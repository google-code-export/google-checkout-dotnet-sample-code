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

    public CancelOrderRequest(string MerchantID, string MerchantKey, 
      string Env, string OrderNo, string Reason) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _OrderNo = OrderNo;
      _Reason = Reason;
    }

    public CancelOrderRequest(string MerchantID, string MerchantKey, 
      string Env, string OrderNo, string Reason, string Comment) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _OrderNo = OrderNo;
      _Reason = Reason;
      _Comment = Comment;
    }

    public override byte[] GetXml() {
      AutoGen.CancelOrderRequest Req = new AutoGen.CancelOrderRequest();
      Req.googleordernumber = _OrderNo;
      Req.reason = _Reason;
      if (_Comment != null) {
        Req.comment = _Comment;
      }
      return EncodeHelper.Serialize(Req);
    }

  }
}
