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
  /// This class contains methods that construct &lt;refund-order&gt; API 
  /// requests.
  /// </summary>
  public class RefundOrderRequest : GCheckoutRequest {
    private string _googleOrderNumber = null;
    private string _Reason = null;
    private string _Currency = null;
    private decimal _Amount = -1;
    private string _Comment = null;

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
      string Env, string GoogleOrderNumber, string Reason) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _googleOrderNumber = GoogleOrderNumber;
      _Reason = Reason;
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
      decimal Amount) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _googleOrderNumber = GoogleOrderNumber;
      _Reason = Reason;
      _Currency = Currency;
      _Amount = Amount;
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
      string Env, string GoogleOrderNumber, string Reason, string Comment) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _googleOrderNumber = GoogleOrderNumber;
      _Reason = Reason;
      _Comment = Comment;
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
      decimal Amount, string Comment) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _googleOrderNumber = GoogleOrderNumber;
      _Reason = Reason;
      _Currency = Currency;
      _Amount = Amount;
      _Comment = Comment;
    }

    /// <summary>Method that is called to produce the Xml message
    ///  that can be posted to Google Checkout.</summary>
    public override byte[] GetXml() {
      AutoGen.RefundOrderRequest Req = new AutoGen.RefundOrderRequest();
      Req.googleordernumber = _googleOrderNumber;
      Req.reason = EncodeHelper.EscapeXmlChars(_Reason);
      if (_Amount != -1 && _Currency != null) {
        Req.amount = new AutoGen.Money();
        Req.amount.currency = _Currency;
        Req.amount.Value = _Amount;
      }
      if (_Comment != null) {
        Req.comment = _Comment;
      }
      return EncodeHelper.Serialize(Req);
    }

  }
}
