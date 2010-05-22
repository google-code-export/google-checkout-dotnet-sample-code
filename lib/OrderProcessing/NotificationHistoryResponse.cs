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
 *  9-7-2008   Joe Feser joe.feser@joefeser.com
 *  Initial Release.
 * 
*/
using System;
using System.Collections.Generic;
using System.Text;
using GCheckout.Util;

namespace GCheckout.OrderProcessing {

  /// <summary>
  /// Wrapper object for the NotificationHistoryResponse
  /// </summary>
  public class NotificationHistoryResponse : GCheckoutResponse {

    private AutoGen.NotificationHistoryResponse _response;
    private List<AutoGen.NotificationHistoryResponse> _additionalResponses
      = new List<GCheckout.AutoGen.NotificationHistoryResponse>();

    private List<string> _invalidOrderNumbers;
    private List<object> _notificationResponses;

    /// <summary>
    /// Parse the Xml for the correct message type.
    /// </summary>
    /// <param name="responseXml">The Xml to parse</param>
    public NotificationHistoryResponse(string responseXml)
      : base(responseXml) {

    }

    /// <summary>
    /// Does the request contain more responses?
    /// </summary>
    public bool HasMoreResults {
      get {
        return !string.IsNullOrEmpty(NextPageToken);
      }
    }

    /// <summary>
    /// Return the list of Order Numbers that are not valid.
    /// </summary>
    public List<string> InvalidOrderNumbers {
      get {
        if (_invalidOrderNumbers == null) {
          _invalidOrderNumbers = new List<string>();
          if (_response.invalidordernumbers != null) {
            _invalidOrderNumbers.AddRange(_response.invalidordernumbers);
          }
        }
        return _invalidOrderNumbers;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Google returned an error code or not.
    /// </summary>
    /// <value>
    /// <c>true</c> if the response did not indicate an error; 
    /// otherwise, <c>false</c>.
    /// </value>
    public override bool IsGood {
      get {
        return (base.IsGood || _response != null);
      }
    }

    /// <summary>
    /// The nextpagetoken if additional response exist?
    /// </summary>
    public string NextPageToken {
      get {
        //we want to make sure we only show the nextpage token if it really needs 
        //to be requested.
        if (_additionalResponses.Count > 0) {
          var item = _additionalResponses[_additionalResponses.Count - 1];
          if (item != null) {
            return item.nextpagetoken;
          }
          return null;
        }
        if (_response != null) {
          return _response.nextpagetoken;
        }
        return null;
      }
    }

    /// <summary>
    /// Obtain the list of Notification Responses.
    /// </summary>
    public List<object> NotificationResponses {
      get {
        if (_notificationResponses == null) {
          _notificationResponses = new List<object>();
          if (_response != null && _response.notifications != null) {
            _notificationResponses.AddRange(_response.notifications.Items);
          }
        }
        if (_additionalResponses != null) {
          foreach (AutoGen.NotificationHistoryResponse item
            in _additionalResponses) {
            if (item != null && item.notifications != null) {
              _notificationResponses.AddRange(item.notifications.Items);
            }
          }
        }
        return _notificationResponses;
      }
    }

    /// <summary>
    /// Return the responses of type AuthorizationNotifications
    /// </summary>
    /// <returns></returns>
    public List<AutoGen.AuthorizationAmountNotification>
      AuthorizationNotifications() {
      return GetMessagesOfType<AutoGen.AuthorizationAmountNotification>();
    }

    /// <summary>
    /// Return the responses of type ChargeAmountNotification
    /// </summary>
    /// <returns></returns>
    public List<AutoGen.ChargeAmountNotification>
      ChargeAmountNotifications() {
      return GetMessagesOfType<AutoGen.ChargeAmountNotification>();
    }

    /// <summary>
    /// Return the responses of type ChargebackAmountNotification
    /// </summary>
    /// <returns></returns>
    public List<AutoGen.ChargebackAmountNotification>
      ChargebackAmountNotifications() {
      return GetMessagesOfType<AutoGen.ChargebackAmountNotification>();
    }

    /// <summary>
    /// Return the responses of type NewOrderNotification
    /// </summary>
    /// <returns></returns>
    public List<AutoGen.NewOrderNotification>
      NewOrderNotifications() {
      return GetMessagesOfType<AutoGen.NewOrderNotification>();
    }

    /// <summary>
    /// Return the responses of type OrderStateChangeNotification
    /// </summary>
    /// <returns></returns>
    public List<AutoGen.OrderStateChangeNotification>
      OrderStateChangeNotifications() {
      return GetMessagesOfType<AutoGen.OrderStateChangeNotification>();
    }

    /// <summary>
    /// Return the responses of type RefundAmountNotification
    /// </summary>
    /// <returns></returns>
    public List<AutoGen.RefundAmountNotification>
      RefundAmountNotifications() {
      return GetMessagesOfType<AutoGen.RefundAmountNotification>();
    }

    /// <summary>
    /// Return the responses of type RiskInformationNotification
    /// </summary>
    /// <returns></returns>
    public List<AutoGen.RiskInformationNotification>
      RiskInformationNotifications() {
      return GetMessagesOfType<AutoGen.RiskInformationNotification>();
    }

    /// <summary>
    /// Parse the Message for a notification data token response message.
    /// </summary>
    /// <returns></returns>
    protected override bool ParseMessage() {
      try {
        if (ResponseXml.IndexOf("<notification-history-response") > -1) {
          _response = (AutoGen.NotificationHistoryResponse)
            EncodeHelper.Deserialize(ResponseXml,
            typeof(AutoGen.NotificationHistoryResponse));
          return true;
        }
      }
      catch {
        //let it continue
      }

      return false;
    }

    /// <summary>
    /// The Response that was returned from the server.
    /// </summary>
    public override object Response {
      get {
        if (_response != null)
          return _response;
        return base.Response;
      }
    }

    /// <summary>
    /// Add an additional response.
    /// </summary>
    /// <remarks>This is used when processing the nextpagetoken.
    /// We are able to combine multiple messages together.</remarks>
    /// <param name="response">The response to add to this message</param>
    public void AddAdditionalResponse(NotificationHistoryResponse response) {
      AutoGen.NotificationHistoryResponse theResponse
        = response.Response as AutoGen.NotificationHistoryResponse;
      if (theResponse != null) {
        _additionalResponses.Add(theResponse);
        _notificationResponses = null;
      }
      else {
        //TODO wrap the error in a specific error to allow people to obtain the message that blew up.
        throw new NotificationHistoryException(response, response.NextPageToken, null);
      }
    }

    /// <summary>
    /// Super hot function used to reduce the list down to 
    /// return just the message types needed
    /// </summary>
    /// <typeparam name="T">The Type to return</typeparam>
    /// <returns></returns>
    private List<T> GetMessagesOfType<T>() where T : class {
      List<T> retVal = new List<T>();

      foreach (object item in NotificationResponses) {
        if (item.GetType() == typeof(T)) {
          retVal.Add(item as T);
        }
      }

      return retVal;
    }

  }
}
