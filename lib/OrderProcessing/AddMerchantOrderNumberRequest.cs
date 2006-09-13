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
using GCheckout.AutoGen;

namespace GCheckout.OrderProcessing {
  /// <summary>
  /// This class contains methods that construct 
  /// &lt;add-merchant-order-number&gt; API requests.
  /// </summary>
  public class AddMerchantOrderNumberRequest : GCheckoutRequest {
    private string _GoogleOrderNo;
    private string _MerchantOrderNo;

    public AddMerchantOrderNumberRequest(string MerchantID, string MerchantKey,
      string Env, string GoogleOrderNo, string MerchantOrderNo) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _GoogleOrderNo = GoogleOrderNo;
      _MerchantOrderNo = MerchantOrderNo;
    }

    public override byte[] GetXml() {
      AutoGen.AddMerchantOrderNumberRequest Req = 
        new AutoGen.AddMerchantOrderNumberRequest();
      Req.googleordernumber = _GoogleOrderNo;
      Req.merchantordernumber = _MerchantOrderNo;
      return EncodeHelper.Serialize(Req);
    }

  }
}
