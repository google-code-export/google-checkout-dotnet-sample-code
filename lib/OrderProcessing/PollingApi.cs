/*************************************************
 * Copyright (C) 2008 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*************************************************/
/*
 Edit History:
 *  August 2008   Joe Feser joe.feser@joefeser.com
 *  Initial Release.
 * 
*/
using System;
using GCheckout.Util;
using System.Data.OleDb;
using System.Data;
using System.Data.Common;
using System.Text;
using GCheckout.AutoGen;
using GCheckout.Checkout;

namespace GCheckout.OrderProcessing {

  /// <summary>
  /// The Polling API allows you to actively request notifications instead 
  /// of setting up a server that
  /// constantly listens for notification callbacks. Using the Polling API, 
  /// you can retrieve all notifications that are less than
  /// 180 days old and that are at least 60 minutes old.
  /// </summary>
  public class PollingApi {

    /// <summary>Google Checkout Merchant ID</summary>
    protected string _MerchantID;
    /// <summary>Google Checkout Merchant Key</summary>
    protected string _MerchantKey;
    /// <summary>EnvironmentType used to determine where the messages are 
    /// posted (Sandbox, Production)</summary>
    protected EnvironmentType _Environment = EnvironmentType.Unknown;

    string _connectionString;
    DateTime _startDate = DateTime.Now.AddMonths(-3);

    private DataSet PollingDS {
      get {
        DataSet ds = GetMerchantDataset();
        DataTable dt = ds.Tables[0];
        if (dt.Rows.Count == 0) {
          VerifyDataRow();
          ds = GetMerchantDataset();
        }
        return ds;
      }
    }

    /// <summary>
    /// Return the Continue Token if one exists.
    /// </summary>
    public string Token {
      get {
        DataTable dt = PollingDS.Tables[0];
        object retVal = dt.Rows[0]["Token"];
        if (!Convert.IsDBNull(retVal))
          return retVal.ToString();
        return string.Empty;
      }
    }

    /// <summary>
    /// Return the Continue Token if one exists.
    /// </summary>
    public DateTime LastUpdatedDate {
      get {
        DataTable dt = PollingDS.Tables[0];
        object retVal = dt.Rows[0]["LastUpdatedDate"];
        if (!Convert.IsDBNull(retVal))
          return (DateTime)retVal;
        return DateTime.MinValue;
      }
    }

    /// <summary>
    /// The start date of the request
    /// </summary>
    public DateTime StartDate {
      get {
        return _startDate;
      }
      set {
        if (value.AddMonths(6) > DateTime.Now)
          throw new ArgumentException("The date must be less that 6 months ago.");
        _startDate = value;
      }
    }

    /// <summary>
    /// Create a new PollingApi request.
    /// </summary>
    /// <param name="merchantID">Google Checkout Merchant ID</param>
    /// <param name="merchantKey">Google Checkout Merchant Key</param>
    /// <param name="environment">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="connectionString">The connection string to the database</param>
    public PollingApi(string merchantID, string merchantKey,
       string environment, string connectionString) {
      _MerchantID = merchantID;
      _MerchantKey = merchantKey;
      _Environment = StringToEnvironment(environment);
      _connectionString = connectionString;
      VerifyDatabase();
      VerifyDataRow();
    }

    /// <summary>
    /// Create a new PollingApi request.
    /// </summary>
    /// <param name="connectionString">The connection string to the database</param>
    public PollingApi(string connectionString) {
      _MerchantID = GCheckoutConfigurationHelper.MerchantID.ToString();
      _MerchantKey = GCheckoutConfigurationHelper.MerchantKey;
      _Environment = GCheckoutConfigurationHelper.Environment;
      _connectionString = connectionString;
      VerifyDatabase();
      VerifyDataRow();
    }

    /// <summary>
    /// Poll the server and obtain the new messages.
    /// </summary>
    /// <returns></returns>
    public int PollServer() {
      int retVal = 0;

      //if the token is empty, we need to go get a token first.
      if (string.IsNullOrEmpty(Token)) {
        NotificationDataTokenRequest request
          = new NotificationDataTokenRequest(_MerchantID, _MerchantKey,
          _Environment.ToString(), StartDate);

        GCheckoutResponse response = request.Send();

        AutoGen.NotificationDataTokenResponse tokenResponse
          = response.Response as AutoGen.NotificationDataTokenResponse;

        if (tokenResponse != null) {
          SaveToken(tokenResponse.continuetoken);
          QueryForNotifications();
        }
        else {
          throw new ApplicationException(
            "NotificationDataTokenResponse did not return" +
            " a NotificationDataTokenResponse message.");
        }
      }

      return retVal;
    }

    /// <summary>
    /// Query for Notifications until we no longer get a continue token
    /// </summary>
    private void QueryForNotifications() {
      NotificationDataRequest request2
        = new NotificationDataRequest(_MerchantID, _MerchantKey,
        _Environment.ToString(), Token);

      GCheckoutResponse ndr = request2.Send();

      AutoGen.NotificationDataResponse dataResponse
      = ndr.Response as AutoGen.NotificationDataResponse;

      if (dataResponse != null) {
        SaveToken(dataResponse.continuetoken);

        if (dataResponse.notifications != null) {
          if (dataResponse.notifications.Items != null) {
            foreach (object item in dataResponse.notifications.Items) {
              byte[] byteValue = EncodeHelper.Serialize(item);
              string value = EncodeHelper.Utf8BytesToString(byteValue);
              //save the message to the database.
              SaveMessage(value, item.GetType().Name);
            }
          }
        }
        //continue if we have more messages.
        if (dataResponse.hasmorenotifications)
          QueryForNotifications();
      }
      else {
        throw new ApplicationException(
          "NotificationDataRequest did not return" +
          " a NotificationDataResponse message.");
      }
    }

    private DataSet GetMerchantDataset() {
      DataSet ds = new DataSet();
      using (OleDbConnection conn = new OleDbConnection(_connectionString)) {
        conn.Open();
        string sql = GetSelectSql();

        using (OleDbCommand cmd = new OleDbCommand(sql, conn)) {

          cmd.Parameters.Add(
            new OleDbParameter("GoogleMerchantID", _MerchantID));

          cmd.Parameters.Add(
            new OleDbParameter("GoogleMerchantKey", _MerchantKey));

          cmd.Parameters.Add(
            new OleDbParameter("SandBox",
            _Environment == EnvironmentType.Sandbox));

          using (DataAdapter da = new OleDbDataAdapter(cmd)) {
            da.Fill(ds);
          }
        }
      }
      return ds;
    }

    private bool SaveMessage(string message, string messageType) {
      StringBuilder sb = new StringBuilder();
      sb.Append("INSERT INTO Messages ");
      sb.Append("(Message,");
      sb.Append(" MessageType)");
      sb.Append(" Values (?, ?)");

      using (OleDbConnection conn = new OleDbConnection(_connectionString)) {
        conn.Open();

        using (OleDbCommand update = new OleDbCommand(sb.ToString(), conn)) {
          update.Parameters.Add(
            new OleDbParameter("Message", message));

          update.Parameters.Add(
            new OleDbParameter("MessageType", messageType));

          update.ExecuteNonQuery();
          return true;
        }
      }
    }

    private bool SaveToken(string token) {
      StringBuilder sb = new StringBuilder();
      sb.Append("UPDATE PollingApi ");
      sb.Append("SET Token = ?");
      sb.Append(" WHERE GoogleMerchantID = ?");
      sb.Append(" AND GoogleMerchantKey = ?");
      sb.Append(" AND SandBox = ?");

      using (OleDbConnection conn = new OleDbConnection(_connectionString)) {
        conn.Open();

        using (OleDbCommand update = new OleDbCommand(sb.ToString(), conn)) {
          update.Parameters.Add(
            new OleDbParameter("Token", token));

          update.Parameters.Add(
            new OleDbParameter("GoogleMerchantID", _MerchantID));

          update.Parameters.Add(
            new OleDbParameter("GoogleMerchantKey", _MerchantKey));

          update.Parameters.Add(
            new OleDbParameter("SandBox",
            _Environment == EnvironmentType.Sandbox));

          update.ExecuteNonQuery();
          return true;
        }
      }
    }

    /// <summary>
    /// Verify we have the row in the database as needed.
    /// </summary>
    private void VerifyDataRow() {

      using (OleDbConnection conn = new OleDbConnection(_connectionString)) {
        conn.Open();
        try {
          //see if a row exists

          DataTable dt = GetMerchantDataset().Tables[0];

          if (dt.Rows.Count == 0) {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO PollingApi ");
            sb.Append("(GoogleMerchantID,");
            sb.Append(" GoogleMerchantKey,");
            sb.Append(" SandBox) ");
            sb.Append(" Values (?, ?, ?)");


            using (OleDbCommand update = new OleDbCommand(sb.ToString(), conn)) {
              update.Parameters.Add(
                new OleDbParameter("GoogleMerchantID", _MerchantID));

              update.Parameters.Add(
                new OleDbParameter("GoogleMerchantKey", _MerchantKey));

              update.Parameters.Add(
                new OleDbParameter("SandBox",
                _Environment == EnvironmentType.Sandbox));

              update.ExecuteNonQuery();

            }
          }
        }
        catch (Exception ex) {
          throw new ApplicationException(
            "Error during row verification.", ex);
        }
      }
    }

    private static string GetSelectSql() {
      StringBuilder sb = new StringBuilder();
      sb.Append("Select * from PollingApi ");
      sb.Append("Where GoogleMerchantID = ?");
      sb.Append("and GoogleMerchantKey = ?");
      sb.Append("and SandBox = ?");
      return sb.ToString();
    }

    private void VerifyDatabase() {
      DataSet ds = new DataSet();
      using (OleDbConnection conn = new OleDbConnection(_connectionString)) {
        conn.Open();
        try {
          DataAdapter da = new OleDbDataAdapter(
            "Select * from PollingApi", conn);
          da.Fill(ds);
        }
        catch (Exception ex) {
          throw new ApplicationException(
            "The PollingApi Table does not exist in the database.", ex);
        }
      }

      //we want to verify we have all of the columns needed in the database.

      string[] columns = new string[] { 
        "ID", 
        "GoogleMerchantID", 
        "GoogleMerchantKey", 
        "SandBox", 
        "Token", 
        "Active", 
        "LastUpdatedDate",
        "LastErrorMessage",
        "LastErrorDate" };

      DataTable dt = ds.Tables[0];

      foreach (string item in columns) {
        if (!dt.Columns.Contains(item))
          throw new ApplicationException(
            string.Format(
            "The Column {0} is required in the PollingAPI Table. Please upgrade.", item));
      }
    }

    /// <summary>Convert a String like Sandbox to the 
    /// EnvironmentType enum</summary>
    protected static EnvironmentType StringToEnvironment(string Env) {
      return (EnvironmentType)Enum.Parse(typeof(EnvironmentType), Env, true);
    }

  }
}
