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
  /// This class contains methods that construct &lt;process-order&gt; API 
  /// requests.
  /// </summary>
  public class ProcessOrderRequest : GCheckoutRequest {
    private string _googleOrderNumber = null;

    /// <summary>
    /// Create a new &lt;process-order&gt; API request message
    /// </summary>
    /// <param name="MerchantID">Google Checkout Merchant ID</param>
    /// <param name="MerchantKey">Google Checkout Merchant Key</param>
    /// <param name="Env">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    public ProcessOrderRequest(string MerchantID, string MerchantKey, 
      string Env, string GoogleOrderNumber) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _googleOrderNumber = GoogleOrderNumber;
    }

    /// <summary>
    /// Create a new &lt;process-order&gt; API request message
    /// using the configuration settings
    /// </summary>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    public ProcessOrderRequest(string GoogleOrderNumber) {
      _MerchantID = GCheckoutConfigurationHelper.MerchantID.ToString();
      _MerchantKey = GCheckoutConfigurationHelper.MerchantKey;
      _Environment = GCheckoutConfigurationHelper.Environment;
      _googleOrderNumber = GoogleOrderNumber;
    }

    /// <summary>Method that is called to produce the Xml message 
    /// that can be posted to Google Checkout.</summary>
    public override byte[] GetXml() {
      AutoGen.ProcessOrderRequest Req = new AutoGen.ProcessOrderRequest();
      Req.googleordernumber = _googleOrderNumber;
      return EncodeHelper.Serialize(Req);
    }

  }
}
