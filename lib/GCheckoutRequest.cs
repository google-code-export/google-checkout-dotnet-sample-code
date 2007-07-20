/*************************************************
 * Copyright (C) 2006 Google Inc.
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

using System;
using System.IO;
using System.Net;
using GCheckout.Util;

namespace GCheckout {
  /// <summary>
  /// This class contains methods for sending API requests to Google Checkout.
  /// </summary>
  public abstract class GCheckoutRequest {
    /// <summary>Google Checkout Merchant ID</summary>
    protected string _MerchantID;
    /// <summary>Google Checkout Merchant Key</summary>
    protected string _MerchantKey;
    /// <summary>EnvironmentType used to determine where the messages are 
    /// posted (Sandbox, Production)</summary>
    protected EnvironmentType _Environment = EnvironmentType.Unknown;

    /// <summary>Method that is called to produce the Xml message that 
    /// can be posted to Google Checkout.</summary>
    public abstract byte[] GetXml();

    private static string GetAuthorization(string User, string Password) {
      return Convert.ToBase64String(EncodeHelper.StringToUtf8Bytes(
        string.Format("{0}:{1}", User, Password)));
    }

    /// <summary>Convert a String like Sandbox to the 
    /// EnvironmentType enum</summary>
    protected static EnvironmentType StringToEnvironment(string Env) {
      return (EnvironmentType)Enum.Parse(typeof(EnvironmentType), Env, true);
    }

    /// <summary>Send the Message to Google Checkout</summary>
    public GCheckoutResponse Send() {
      CheckSendPreConditions();
      byte[] Data = GetXml();
      // Prepare web request.
      HttpWebRequest myRequest =
        (HttpWebRequest)WebRequest.Create(GetPostUrl());
      myRequest.Method = "POST";
      myRequest.ContentLength = Data.Length;
      myRequest.Headers.Add("Authorization",
        string.Format("Basic {0}",
        GetAuthorization(_MerchantID, _MerchantKey)));
      myRequest.ContentType = "application/xml; charset=UTF-8";
      myRequest.Accept = "application/xml; charset=UTF-8";
      myRequest.KeepAlive = false;

      //determine if we are using a proxy server
      if (GCheckoutConfigurationHelper.UseProxy) {
        Uri proxyUrl = new Uri(GCheckoutConfigurationHelper.ProxyHost);
        WebProxy proxy = new WebProxy(proxyUrl);
        proxy.Credentials = new NetworkCredential(
          GCheckoutConfigurationHelper.ProxyUserName,
          GCheckoutConfigurationHelper.ProxyPassword);
        myRequest.Proxy = proxy;
      }

      // Send the data.
      using (Stream requestStream = myRequest.GetRequestStream()) {
        requestStream.Write(Data, 0, Data.Length);
      }

      // Read the response.
      string responseXml = string.Empty;
      try {
        using (HttpWebResponse myResponse =
          (HttpWebResponse)myRequest.GetResponse()) {
          using (Stream ResponseStream = myResponse.GetResponseStream()) {
            using (StreamReader ResponseStreamReader =
              new StreamReader(ResponseStream)) {
              responseXml = ResponseStreamReader.ReadToEnd();
            }
          }
        }
      }
      catch (WebException WebExcp) {
        if (WebExcp.Response != null) {
          using (HttpWebResponse HttpWResponse =
            (HttpWebResponse)WebExcp.Response) {
            using (StreamReader sr =
              new StreamReader(HttpWResponse.GetResponseStream())) {
              responseXml = sr.ReadToEnd();
            }
          }
        }
      }
      return new GCheckoutResponse(responseXml);
    }

    /// <summary></summary>
    public virtual string GetPostUrl() {
      if (_Environment == EnvironmentType.Sandbox) {
        return string.Format(
          "https://sandbox.google.com/checkout/cws/v2/Merchant/{0}/request",
          _MerchantID);
      }
      else {
        return string.Format(
          "https://checkout.google.com/cws/v2/Merchant/{0}/request",
          _MerchantID);
      }
    }

    private void CheckSendPreConditions() {
      if (_Environment == EnvironmentType.Unknown) {
        throw new ApplicationException(
          "Environment has not been set to Sandbox or Production");
      }
      if (_MerchantID == null) {
        throw new ApplicationException("MerchantID has not been set");
      }
      if (_MerchantKey == null) {
        throw new ApplicationException("MerchantKey has not been set");
      }
    }

    /// <summary>Google Checkout Merchant ID</summary>
    public string MerchantID {
      get {
        return _MerchantID;
      }
      set {
        _MerchantID = value;
      }
    }

    /// <summary>Google Checkout Merchant Key</summary>
    public string MerchantKey {
      get {
        return _MerchantKey;
      }
      set {
        _MerchantKey = value;
      }
    }

    /// <summary>EnvironmentType used to determine where 
    /// the messages are posted (Sandbox, Production)</summary>
    public EnvironmentType Environment {
      get {
        return _Environment;
      }
      set {
        _Environment = value;
      }
    }
  }

  /// <summary>Determine where the messages are posted
  /// (Sandbox, Production)</summary>
  public enum EnvironmentType {
    /// <summary>Use the Sandbox account to post the messages</summary>
    Sandbox = 0,
    /// <summary>Use the Production account to post the messages</summary>
    Production = 1,
    /// <summary>Unknown account.</summary>
    Unknown = 2
  }
}
