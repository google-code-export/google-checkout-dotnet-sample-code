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
    private string _OrderNo = null;
    private string _Reason = null;
    private string _Currency = null;
    private decimal _Amount = -1;
    private string _Comment = null;

    public RefundOrderRequest(string MerchantID, string MerchantKey, 
      string Env, string OrderNo, string Reason) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _OrderNo = OrderNo;
      _Reason = Reason;
    }

    public RefundOrderRequest(string MerchantID, string MerchantKey, 
      string Env, string OrderNo, string Reason, string Currency, 
      decimal Amount, string Comment) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _OrderNo = OrderNo;
      _Reason = Reason;
      _Currency = Currency;
      _Amount = Amount;
      _Comment = Comment;
    }

    public override byte[] GetXml() {
      AutoGen.RefundOrderRequest Req = new AutoGen.RefundOrderRequest();
      Req.googleordernumber = _OrderNo;
      Req.reason = _Reason;
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
