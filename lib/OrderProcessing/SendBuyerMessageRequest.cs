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
  /// This class contains methods that construct &lt;send-buyer-message&gt; 
  /// API requests.
  /// </summary>
  public class SendBuyerMessageRequest : GCheckoutRequest {
    private string _OrderNo = null;
    private string _Message = null;
    private bool _SendEmail = true;

    public SendBuyerMessageRequest(string MerchantID, string MerchantKey, 
      string Env, string OrderNo, string Message) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _OrderNo = OrderNo;
      _Message = Message;
    }

    public SendBuyerMessageRequest(string MerchantID, string MerchantKey, 
      string Env, string OrderNo, string Message, bool SendEmail) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _OrderNo = OrderNo;
      _Message = Message;
      _SendEmail = SendEmail;
    }

    public override byte[] GetXml() {
      AutoGen.SendBuyerMessageRequest Req = 
        new AutoGen.SendBuyerMessageRequest();
      Req.googleordernumber = _OrderNo;
      Req.message = _Message;
      Req.sendemail = _SendEmail;
      Req.sendemailSpecified = true;
      return EncodeHelper.Serialize(Req);
    }

  }
}
