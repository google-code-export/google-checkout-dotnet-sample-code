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
  /// This class contains methods that construct &lt;deliver-order&gt; API 
  /// requests.
  /// </summary>
  public class DeliverOrderRequest : GCheckoutRequest {
    private string _OrderNo;
    private string _Carrier = null;
    private string _TrackingNo = null;
    private bool _SendEmail;

    /// <summary>
    /// Create a new &lt;deliver-order&gt; API request
    /// </summary>
    /// <param name="MerchantID">Google Checkout Merchant ID</param>
    /// <param name="MerchantKey">Google Checkout Merchant Key</param>
    /// <param name="Env">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="OrderNo">The Google Order Number</param>
    public DeliverOrderRequest(string MerchantID, string MerchantKey, 
      string Env, string OrderNo) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _OrderNo = OrderNo;
    }

    /// <summary>
    /// Create a new &lt;deliver-order&gt; API request message
    /// </summary>
    /// <param name="MerchantID">Google Checkout Merchant ID</param>
    /// <param name="MerchantKey">Google Checkout Merchant Key</param>
    /// <param name="Env">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="OrderNo">The Google Order Number</param>
    /// <param name="Carrier">The Carrier the package was shipped with</param>
    /// <param name="TrackingNo">The Tracking Number for the order</param>
    /// <param name="SendEmail">Send an email to the buyer</param>
    public DeliverOrderRequest(string MerchantID, string MerchantKey, 
      string Env, string OrderNo, string Carrier, string TrackingNo,
      bool SendEmail) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _OrderNo = OrderNo;
      _Carrier = Carrier;
      _TrackingNo = TrackingNo;
      _SendEmail = SendEmail;
    }

    /// <summary>
    /// Create a new &lt;deliver-order&gt; API request message
    /// </summary>
    /// <param name="MerchantID">Google Checkout Merchant ID</param>
    /// <param name="MerchantKey">Google Checkout Merchant Key</param>
    /// <param name="Env">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="OrderNo">The Google Order Number</param>
    /// <param name="Carrier">The Carrier the package was shipped with</param>
    /// <param name="TrackingNo">The Tracking Number for the order</param>
    public DeliverOrderRequest(string MerchantID, string MerchantKey, 
      string Env, string OrderNo, string Carrier, string TrackingNo) 
    {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _OrderNo = OrderNo;
      _Carrier = Carrier;
      _TrackingNo = TrackingNo;
    }

    /// <summary>
    /// Create a new &lt;deliver-order&gt; API request message
    /// </summary>
    /// <param name="MerchantID">Google Checkout Merchant ID</param>
    /// <param name="MerchantKey">Google Checkout Merchant Key</param>
    /// <param name="Env">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="OrderNo">The Google Order Number</param>
    /// <param name="SendEmail">Send an email to the buyer</param>
    public DeliverOrderRequest(string MerchantID, string MerchantKey, 
      string Env, string OrderNo, bool SendEmail)
    {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _OrderNo = OrderNo;
      _SendEmail = SendEmail;
    }

    /// <summary>Method that is called to produce the Xml message
    ///  that can be posted to Google Checkout.</summary>
    public override byte[] GetXml() 
    {
      AutoGen.DeliverOrderRequest Req = new AutoGen.DeliverOrderRequest();
      Req.googleordernumber = _OrderNo;
      if (_Carrier != null && _TrackingNo != null) 
      {
        Req.trackingdata = new AutoGen.TrackingData();
        Req.trackingdata.carrier = _Carrier;
        Req.trackingdata.trackingnumber = _TrackingNo;
      }
      if (_SendEmail) {
        Req.sendemail = _SendEmail;
      }
      return EncodeHelper.Serialize(Req);
    }
  }
}
