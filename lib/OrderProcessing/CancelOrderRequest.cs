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
  /// This class contains methods that construct &lt;cancel-order&gt; API 
  /// requests.
  /// </summary>
  public class CancelOrderRequest : GCheckoutRequest {
    private string _OrderNo = null;
    private string _Reason = null;
    private string _Comment = null;

    /// <summary>
    /// Create a new &lt;cancel-order&gt; API request message
    /// </summary>
    /// <param name="MerchantID">Google Checkout Merchant ID</param>
    /// <param name="MerchantKey">Google Checkout Merchant Key</param>
    /// <param name="Env">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="OrderNo">The Google Order Number</param>
    /// <param name="Reason">The Reson for canceling the order</param>
    public CancelOrderRequest(string MerchantID, string MerchantKey,
      string Env, string OrderNo, string Reason) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _OrderNo = OrderNo;
      _Reason = Reason;
      CheckCreationPostConditions();
    }

    /// <summary>
    /// Create a new &lt;cancel-order&gt; API request message
    /// </summary>
    /// <param name="MerchantID">Google Checkout Merchant ID</param>
    /// <param name="MerchantKey">Google Checkout Merchant Key</param>
    /// <param name="Env">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="OrderNo">The Google Order Number</param>
    /// <param name="Reason">The Reson for canceling the order</param>
    /// <param name="Comment">A comment to associate to the canceled order</param>
    public CancelOrderRequest(string MerchantID, string MerchantKey,
      string Env, string OrderNo, string Reason, string Comment) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _OrderNo = OrderNo;
      _Reason = Reason;
      _Comment = Comment;
      CheckCreationPostConditions();
    }

    private void CheckCreationPostConditions() {
      if (_Reason == null || _Reason == string.Empty) {
        throw new ApplicationException("Reason cannot be empty");
      }
    }

    /// <summary>Method that is called to produce the Xml message 
    /// that can be posted to Google Checkout.</summary>
    public override byte[] GetXml() {
      AutoGen.CancelOrderRequest Req = new AutoGen.CancelOrderRequest();
      Req.googleordernumber = _OrderNo;
      Req.reason = EncodeHelper.EscapeXmlChars(_Reason);
      if (_Comment != null) {
        Req.comment = _Comment;
      }
      return EncodeHelper.Serialize(Req);
    }

  }
}
