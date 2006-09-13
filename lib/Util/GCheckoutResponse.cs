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
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using GCheckout.Util;

namespace GCheckout.Util {
  /// <summary>
  /// This request contains methods that handle a &lt;checkout-redirect&gt; 
  /// response from Google Checkout and capture the URL to which a customer 
  /// should be redirected to complete the checkout process.
  /// </summary>
  public class GCheckoutResponse {
    private AutoGen.RequestReceivedResponse _GoodResponse = null;
    private AutoGen.ErrorResponse _ErrorResponse = null;
    private AutoGen.CheckoutRedirect _CheckoutRedirectResponse = null;
    private string _Xml;

    /// <summary>
    /// Creates a new instance of the <see cref="GCheckoutResponse"/> class.
    /// </summary>
    /// <param name="ResponseXml">The XML returned from Google.</param>
    public GCheckoutResponse(string ResponseXml) {
      _Xml = ResponseXml;
      try {
        _GoodResponse = (AutoGen.RequestReceivedResponse) 
          EncodeHelper.Deserialize(ResponseXml, 
          typeof(AutoGen.RequestReceivedResponse));
      }
      catch {
        try {
          _ErrorResponse = (AutoGen.ErrorResponse) 
            EncodeHelper.Deserialize(ResponseXml, 
            typeof(AutoGen.ErrorResponse));
        }
        catch {
          _CheckoutRedirectResponse = (AutoGen.CheckoutRedirect) 
            EncodeHelper.Deserialize(ResponseXml, 
            typeof(AutoGen.CheckoutRedirect));
        }
      }
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current 
    /// GCheckoutResponse. Intended for debugging only.
    /// </summary>
    /// <returns>
    /// A human-readable <see cref="T:System.String"/> that represents the 
    /// current GCheckoutResponse. Note that the format of the string may 
    /// change in future releases.
    /// </returns>
    public override string ToString() {
      return string.Format("[GCheckoutResponse -- IsGood: '{0}', " +
      "SerialNo: '{1}', ErrorMsg: '{2}', RedirectUrl: '{3}']", 
        IsGood, SerialNumber, ErrorMessage, RedirectUrl);
    }

    /// <summary>
    /// Gets a value indicating whether Google returned an error code or not.
    /// </summary>
    /// <value>
    /// <c>true</c> if the response did not indicate an error; 
    /// otherwise, <c>false</c>.
    /// </value>
    public bool IsGood {
      get {
        return (_GoodResponse != null || _CheckoutRedirectResponse != null);
      }
    }

    /// <summary>
    /// Gets the serial number. Google attaches a unique serial number to
    /// every response.
    /// </summary>
    /// <value>
    /// The serial number, for example 58ea39d3-025b-4d52-a697-418f0be74bf9.
    /// </value>
    public string SerialNumber {
      get {
        if (_GoodResponse != null) {
          return _GoodResponse.serialnumber;
        }
        if (_ErrorResponse != null) {
          return _ErrorResponse.serialnumber;
        }
        if (_CheckoutRedirectResponse != null) {
          return _CheckoutRedirectResponse.serialnumber;
        }
        throw new ApplicationException("All three responses are null; " +
          "something's wrong!");
      }
    }

    /// <summary>
    /// If Google responded with an error (IsGood = false) then this 
    /// property will contain the human-readable error message.
    /// </summary>
    /// <value>
    /// The error message returned by Google, or an empty string if
    /// there was no error.
    /// </value>
    public string ErrorMessage {
      get {
        if (_ErrorResponse != null) {
          return _ErrorResponse.errormessage;
        }
        else {
          return "";
        }
      }
    }

    /// <summary>
    /// If Google indicated a redirect URL in the response, this property
    /// will contain the URL string.
    /// </summary>
    /// <value>
    /// The redirect URL, or the empty string if Google didn't send a redirect
    /// URL.
    /// </value>
    public string RedirectUrl {
      get {
        if (_CheckoutRedirectResponse != null) {
          return _CheckoutRedirectResponse.redirecturl.Replace("&amp;", "&");
        }
        else {
          return "";
        }
      }
    }

    /// <summary>
    /// Gets the response XML sent by Google.
    /// </summary>
    /// <value>The response XML sent by Google.</value>
    public string ResponseXml {
      get {
        return _Xml;
      }
    }

  }
}
