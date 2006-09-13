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
  /// This class contains methods that construct &lt;add-tracking-data&gt; 
  /// API requests.
  /// </summary>
  public class AddTrackingDataRequest : GCheckoutRequest {
    private string _OrderNo;
    private string _Carrier;
    private string _TrackingNo;

    public AddTrackingDataRequest(string MerchantID, string MerchantKey, 
      string Env, string OrderNo, string Carrier, string TrackingNo) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _OrderNo = OrderNo;
      _Carrier = Carrier;
      _TrackingNo = TrackingNo;
    }

    public override byte[] GetXml() {
      AutoGen.AddTrackingDataRequest Req = 
        new AutoGen.AddTrackingDataRequest();
      Req.googleordernumber = _OrderNo;
      Req.trackingdata = new AutoGen.TrackingData();
      Req.trackingdata.carrier = _Carrier;
      Req.trackingdata.trackingnumber = _TrackingNo;
      return EncodeHelper.Serialize(Req);
    }

  }
}
