/*************************************************
 * Copyright (C) 2010 Google Inc.
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
 *  5-22-2010   Joe Feser joe.feser@joefeser.com
 *  Initial Release.
 * 
*/
using System;
using System.Collections.Generic;
using System.Text;
using GCheckout.Util;

namespace GCheckout.OrderProcessing {

  /// <summary>
  /// OrderSummaryRequest
  /// </summary>
  public class OrderSummaryRequest : GCheckoutRequest {

    /// <summary>
    /// The Google Order Numbers for the request
    /// </summary>
    public List<string> GoogleOrderNumbers {
      get;
      private set;
    }

    /// <summary>
    /// Create a new Instance of OrderSummaryRequest
    /// </summary>
    /// <param name="merchantID">Google Checkout Merchant ID</param>
    /// <param name="merchantKey">Google Checkout Merchant Key</param>
    /// <param name="environment">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    public OrderSummaryRequest(string merchantID,
      string merchantKey, string environment) {
      _MerchantID = merchantID;
      _MerchantKey = merchantKey;
      _Environment = StringToEnvironment(environment);
      GoogleOrderNumbers = new List<string>();
    }

    /// <summary>
    /// Create a new Instance of OrderSummaryRequest
    /// </summary>
    public OrderSummaryRequest() {
      _MerchantID = GCheckoutConfigurationHelper.MerchantID.ToString();
      _MerchantKey = GCheckoutConfigurationHelper.MerchantKey;
      _Environment = GCheckoutConfigurationHelper.Environment;
      GoogleOrderNumbers = new List<string>();
    }

    /// <summary>
    /// Create a new Instance of OrderSummaryRequest
    /// </summary>
    /// <param name="merchantID">Google Checkout Merchant ID</param>
    /// <param name="merchantKey">Google Checkout Merchant Key</param>
    /// <param name="environment">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="googleOrderNumber">The single order to request</param>
    public OrderSummaryRequest(string merchantID,
      string merchantKey, string environment, string googleOrderNumber) {
      _MerchantID = merchantID;
      _MerchantKey = merchantKey;
      _Environment = StringToEnvironment(environment);
      GoogleOrderNumbers = new List<string>();
      GoogleOrderNumbers.Add(googleOrderNumber);
    }

    /// <summary>
    /// Create a new Instance of OrderSummaryRequest
    /// </summary>
    /// <param name="googleOrderNumber">The single order to request</param>
    public OrderSummaryRequest(string googleOrderNumber) {
      _MerchantID = GCheckoutConfigurationHelper.MerchantID.ToString();
      _MerchantKey = GCheckoutConfigurationHelper.MerchantKey;
      _Environment = GCheckoutConfigurationHelper.Environment;
      GoogleOrderNumbers = new List<string>();
      GoogleOrderNumbers.Add(googleOrderNumber);
    }

    /// <summary>
    /// Create a new Instance of OrderSummaryRequest
    /// </summary>
    /// <param name="merchantID">Google Checkout Merchant ID</param>
    /// <param name="merchantKey">Google Checkout Merchant Key</param>
    /// <param name="environment">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="googleOrderNumbers">The orders to request</param>
    public OrderSummaryRequest(string merchantID,
      string merchantKey, string environment, List<string> googleOrderNumbers) {
      _MerchantID = merchantID;
      _MerchantKey = merchantKey;
      _Environment = StringToEnvironment(environment);
      if (googleOrderNumbers == null) {
        throw new ArgumentNullException("googleOrderNumbers");
      }
      GoogleOrderNumbers = new List<string>(googleOrderNumbers);
    }

    /// <summary>
    /// Create a new Instance of OrderSummaryRequest
    /// </summary>
    /// <param name="googleOrderNumbers">The orders to request</param>
    public OrderSummaryRequest(List<string> googleOrderNumbers) {
      _MerchantID = GCheckoutConfigurationHelper.MerchantID.ToString();
      _MerchantKey = GCheckoutConfigurationHelper.MerchantKey;
      _Environment = GCheckoutConfigurationHelper.Environment;
      if (googleOrderNumbers == null) {
        throw new ArgumentNullException("googleOrderNumbers");
      }
      GoogleOrderNumbers = new List<string>(googleOrderNumbers);
    }

    /// <summary>
    /// Return the parsed response from the service
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    protected override GCheckoutResponse ParseResponse(string response) {
      try {
        return new OrderSummaryResponse(response);
      }
      catch (Exception) {
        return null;
      }
    }

    /// <summary>
    /// Get the Xml For the message
    /// </summary>
    /// <returns></returns>
    public override byte[] GetXml() {
      var retVal = new AutoGen.OrderSummaryRequest();
      retVal.ordernumbers = GoogleOrderNumbers.ToArray();
      return EncodeHelper.Serialize(retVal);
    }

  }
}
