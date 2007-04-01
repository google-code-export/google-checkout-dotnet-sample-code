/*************************************************
 * Copyright (C) 2006-2007 Google Inc.
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
using System.Web;
using System.Xml;
using System.Configuration;

namespace GCheckout.Util {
  /// <summary>
  /// Summary description for GCheckoutConfigHandler.
  /// </summary>
  public class GCheckoutConfigHandler : IConfigurationSectionHandler {

    /// <summary>
    /// Create a new instance of the Config Handler
    /// </summary>
    public GCheckoutConfigHandler() {

    }

    #region IConfigurationSectionHandler Members

    /// <summary>
    /// Create the application Context
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="configContext"></param>
    /// <param name="section"></param>
    /// <returns></returns>
    public object Create(object parent, object configContext, XmlNode section) {

      GCheckoutConfigSection retVal = new GCheckoutConfigSection();

      string productionMerchantID = string.Empty;
      string productionMerchantKey = string.Empty;
      string sandboxMerchantID = string.Empty;
      string sandboxMerchantKey = string.Empty;
      string currency = string.Empty;
      long platformID = 0;
      int envTemp = 0;
      EnvironmentType environment = EnvironmentType.Unknown;

      bool logging = false;
      string logTemp = string.Empty;
      string platformIDTemp = string.Empty;

      string logDirectory = string.Empty;

      GCheckoutConfigurationHelper.GetStringValue(section, "SandboxMerchantID", ref sandboxMerchantID);
      GCheckoutConfigurationHelper.GetStringValue(section, "SandboxMerchantKey", ref sandboxMerchantKey);
      GCheckoutConfigurationHelper.GetStringValue(section, "ProductionMerchantID", ref productionMerchantID);
      GCheckoutConfigurationHelper.GetStringValue(section, "ProductionMerchantKey", ref productionMerchantKey);
      GCheckoutConfigurationHelper.GetStringValue(section, "Currency", false, ref currency);


      GCheckoutConfigurationHelper.GetEnumValue(section, "Environment", typeof(EnvironmentType), ref envTemp);
      environment = (EnvironmentType) envTemp;

      GCheckoutConfigurationHelper.GetStringValue(section, "Logging", false, ref logTemp);
      if (logTemp != null && logTemp.Length > 0) {
        logging = logTemp.ToLower() == "true";
      }

      //ensure we do not throw an exception.
      try {
        GCheckoutConfigurationHelper.GetStringValue(section, "PlatformID", false, ref platformIDTemp);
        if (platformIDTemp != null && platformIDTemp.Length > 0) {
          platformID = long.Parse(platformIDTemp);
        }
      }
      catch (Exception ex) {
        throw new ConfigurationException("Error Setting PlatformID", ex);
      }

      GCheckoutConfigurationHelper.GetStringValue(section, "LogDirectory", false, ref logDirectory);

      try {
        retVal.ProductionMerchantID = long.Parse(productionMerchantID);
      }
      catch (Exception ex) {
        throw new ConfigurationException("Error Setting ProductionMerchantID", ex);
      }
      retVal.ProductionMerchantKey = productionMerchantKey;

      try {
        retVal.SandboxMerchantID = long.Parse(sandboxMerchantID);
      }
      catch (Exception ex) {
        throw new ConfigurationException("Error Setting SandboxMerchantID", ex);
      }

      retVal.SandboxMerchantKey = sandboxMerchantKey;
      retVal.Environment = environment;
      retVal.Logging = logging;
      retVal.LogDirectory = logDirectory;
      retVal.PlatformID = platformID;
      retVal.Currency = currency;

      return retVal;
    }

    #endregion
  }
}
