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
/*
 Edit History:
 *  3-14-2009   Joe Feser joe.feser@joefeser.com
 *  We no longer allow people to pass in fractional amounts. All numbers are trimmed to $x.xx
 * 
*/
using System;
using GCheckout.Util;

namespace GCheckout.OrderProcessing {
  
  /// <summary>
  /// This class contains methods that construct &lt;refund-order&gt; API 
  /// requests.
  /// </summary>
  public class RefundOrderRequest : OrderProcessingBase {
    private string _reason;
    private string _currency = GCheckoutConfigurationHelper.Currency;
    private decimal _amount = decimal.MinValue;
    private string _comment;

    /// <summary>
    /// Create a new &lt;refund-order&gt; API request message
    /// </summary>
    /// <param name="MerchantID">Google Checkout Merchant ID</param>
    /// <param name="MerchantKey">Google Checkout Merchant Key</param>
    /// <param name="Env">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    /// <param name="Reason">The Reason for the refund</param>
    public RefundOrderRequest(string MerchantID, string MerchantKey,
      string Env, string GoogleOrderNumber, string Reason)
      : base(MerchantID, MerchantKey, Env, GoogleOrderNumber) {
      _reason = Reason;
      _amount = -1;
    }

    /// <summary>
    /// Create a new &lt;refund-order&gt; API request message
    /// using the configuration settings
    /// </summary>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    /// <param name="Reason">The Reason for the refund</param>
    public RefundOrderRequest(string GoogleOrderNumber, string Reason)
      : base(GoogleOrderNumber) {
      _reason = Reason;
      _amount = -1;
    }

    /// <summary>
    /// Create a new &lt;refund-order&gt; API request message
    /// </summary>
    /// <param name="MerchantID">Google Checkout Merchant ID</param>
    /// <param name="MerchantKey">Google Checkout Merchant Key</param>
    /// <param name="Env">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    /// <param name="Reason">The Reason for the refund</param>
    /// <param name="Currency">The Currency used to charge the order</param>
    /// <param name="Amount">The Amount to charge</param>
    public RefundOrderRequest(string MerchantID, string MerchantKey,
      string Env, string GoogleOrderNumber, string Reason, string Currency,
      decimal Amount)
      : base(MerchantID, MerchantKey, Env, GoogleOrderNumber) {
      _reason = Reason;
      _currency = Currency;
      _amount = Math.Round(Amount, 2); //fix for sending in fractional cents
    }

    /// <summary>
    /// Create a new &lt;refund-order&gt; API request message
    /// using the configuration settings
    /// </summary>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    /// <param name="Reason">The Reason for the refund</param>
    /// <param name="Currency">The Currency used to charge the order</param>
    /// <param name="Amount">The Amount to charge</param>
    public RefundOrderRequest(string GoogleOrderNumber, string Reason,
      string Currency, decimal Amount) : base(GoogleOrderNumber) {
      _reason = Reason;
      _currency = Currency;
      _amount = Math.Round(Amount, 2); //fix for sending in fractional cents
    }

    /// <summary>
    /// Create a new &lt;refund-order&gt; API request message
    /// </summary>
    /// <param name="MerchantID">Google Checkout Merchant ID</param>
    /// <param name="MerchantKey">Google Checkout Merchant Key</param>
    /// <param name="Env">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    /// <param name="Reason">The Reason for the refund</param>
    /// <param name="Comment">A comment to append to the order</param>
    public RefundOrderRequest(string MerchantID, string MerchantKey,
      string Env, string GoogleOrderNumber, string Reason, string Comment)
      : base(MerchantID, MerchantKey, Env, GoogleOrderNumber) {
      _reason = Reason;
      _comment = Comment;
    }

    /// <summary>
    /// Create a new &lt;refund-order&gt; API request message
    /// using the configuration settings
    /// </summary>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    /// <param name="Reason">The Reason for the refund</param>
    /// <param name="Comment">A comment to append to the order</param>
    public RefundOrderRequest(string GoogleOrderNumber, 
      string Reason, string Comment) : base(GoogleOrderNumber) {
      _reason = Reason;
      _comment = Comment;
    }

    /// <summary>
    /// Create a new &lt;refund-order&gt; API request message
    /// </summary>
    /// <param name="MerchantID">Google Checkout Merchant ID</param>
    /// <param name="MerchantKey">Google Checkout Merchant Key</param>
    /// <param name="Env">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    /// <param name="Reason">The Reason for the refund</param>
    /// <param name="Currency">The Currency used to charge the order</param>
    /// <param name="Amount">The Amount to charge</param>
    /// <param name="Comment">A comment to append to the order</param>
    public RefundOrderRequest(string MerchantID, string MerchantKey,
      string Env, string GoogleOrderNumber, string Reason, string Currency,
      decimal Amount, string Comment)
      : base(MerchantID, MerchantKey, Env, GoogleOrderNumber) {
      _reason = Reason;
      _currency = Currency;
      _amount = Math.Round(Amount, 2); //fix for sending in fractional cents
      _comment = Comment;
    }

    /// <summary>
    /// Create a new &lt;refund-order&gt; API request message
    /// using the configuration settings
    /// </summary>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    /// <param name="Reason">The Reason for the refund</param>
    /// <param name="Currency">The Currency used to charge the order</param>
    /// <param name="Amount">The Amount to charge</param>
    /// <param name="Comment">A comment to append to the order</param>
    public RefundOrderRequest(string GoogleOrderNumber, string Reason, 
      string Currency, decimal Amount, string Comment)
      : base(GoogleOrderNumber) {
      _reason = Reason;
      _currency = Currency;
      _amount = Math.Round(Amount, 2); //fix for sending in fractional cents
      _comment = Comment;
    }

    /// <summary>Method that is called to produce the Xml message
    ///  that can be posted to Google Checkout.</summary>
    public override byte[] GetXml() {
      AutoGen.RefundOrderRequest Req = new AutoGen.RefundOrderRequest();
      Req.googleordernumber = GoogleOrderNumber;
      Req.reason = EncodeHelper.EscapeXmlChars(_reason);
      if (_amount > decimal.MinValue && _currency != null) {
        Req.amount = new AutoGen.Money();
        Req.amount.currency = _currency;
        Req.amount.Value = _amount; //already checked.
      }
      if (_comment != null) {
        Req.comment = _comment;
      }
      return EncodeHelper.Serialize(Req);
    }
  }
}
