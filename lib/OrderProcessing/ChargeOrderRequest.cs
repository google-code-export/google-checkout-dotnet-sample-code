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
using GCheckout.Util;

namespace GCheckout.OrderProcessing {
  /// <summary>
  /// This class contains methods that construct &lt;charge-order&gt; API 
  /// requests.
  /// </summary>
  public class ChargeOrderRequest : GCheckoutRequest {
    private string _googleOrderNumber = null;
    private string _Currency = null;
    private decimal _Amount = -1;

    /// <summary>
    /// The Google Order Number
    /// </summary>
    public string GoogleOrderNumber {
      get {
        return _googleOrderNumber;
      }
    }

    /// <summary>
    /// Create a new &lt;charge-order&gt; API request message
    /// </summary>
    /// <param name="MerchantID">Google Checkout Merchant ID</param>
    /// <param name="MerchantKey">Google Checkout Merchant Key</param>
    /// <param name="Env">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    public ChargeOrderRequest(string MerchantID, string MerchantKey, 
      string Env, string GoogleOrderNumber) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _googleOrderNumber = GoogleOrderNumber;
    }

    /// <summary>
    /// Create a new &lt;charge-order&gt; API request message
    /// using the configuration settings
    /// </summary>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    public ChargeOrderRequest(string GoogleOrderNumber) {
      _MerchantID = GCheckoutConfigurationHelper.MerchantID.ToString();
      _MerchantKey = GCheckoutConfigurationHelper.MerchantKey;
      _Environment = GCheckoutConfigurationHelper.Environment;
      _googleOrderNumber = GoogleOrderNumber;
    }

    /// <summary>
    /// Create a new &lt;charge-order&gt; API request message
    /// </summary>
    /// <param name="MerchantID">Google Checkout Merchant ID</param>
    /// <param name="MerchantKey">Google Checkout Merchant Key</param>
    /// <param name="Env">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    /// <param name="Currency">The Currency used to charge the order</param>
    /// <param name="Amount">The Amount to charge</param>
    public ChargeOrderRequest(string MerchantID, string MerchantKey, 
      string Env, string GoogleOrderNumber, string Currency, decimal Amount) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _googleOrderNumber = GoogleOrderNumber;
      _Currency = Currency;
      _Amount = Amount;
    }

    /// <summary>
    /// Create a new &lt;charge-order&gt; API request message
    /// using the configuration settings
    /// </summary>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    /// <param name="Currency">The Currency used to charge the order</param>
    /// <param name="Amount">The Amount to charge</param>
    public ChargeOrderRequest(string GoogleOrderNumber, string Currency, 
      decimal Amount) {
      _MerchantID = GCheckoutConfigurationHelper.MerchantID.ToString();
      _MerchantKey = GCheckoutConfigurationHelper.MerchantKey;
      _Environment = GCheckoutConfigurationHelper.Environment;
      _googleOrderNumber = GoogleOrderNumber;
      _Currency = Currency;
      _Amount = Amount;
    }

    /// <summary>Method that is called to produce the Xml message 
    /// that can be posted to Google Checkout.</summary>
    public override byte[] GetXml() {
      AutoGen.ChargeOrderRequest Req = new AutoGen.ChargeOrderRequest();
      Req.googleordernumber = _googleOrderNumber;
      if (_Amount != -1 && _Currency != null) {
        Req.amount = new AutoGen.Money();
        Req.amount.currency = _Currency;
        Req.amount.Value = _Amount;
      }
      return EncodeHelper.Serialize(Req);
    }

  }
}
