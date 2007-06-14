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
using System.Configuration;
using System.Xml;

namespace GCheckout.Util {

  /// <summary>
  /// Entry point to obtain the Google Checkout Settings
  /// </summary>
  public class GCheckoutConfigurationHelper {

    private static GCheckoutConfigSection __configSection = null;
    private static bool _initialized = false;
    private static Exception configException = null;

    /// <summary>
    /// The Config Section defined
    /// </summary>
    protected static GCheckoutConfigSection ConfigSection {
      get {
        if (__configSection == null) {
          Initialize();
        }
        return __configSection;
      }
    }

    private static void Initialize() {
      if (!_initialized) {
        try {
          __configSection = 
            ConfigurationSettings.GetConfig("Google/GoogleCheckout")
            as GCheckoutConfigSection;
          _initialized = true;
          configException = null;
        }
        catch (Exception ex) {
          _initialized = false;
          configException = ex;
          System.Diagnostics.Debug.Write(
            "Config Section Missing or error:" + ex.ToString());
          throw;
        }
      }
    }

    /// <summary>
    /// Your numeric Merchant ID. To see it, log in to Google,
    /// select the Settings tab, click the Integration link.
    /// </summary>
    public static long MerchantID {
      get {
        if (ConfigSection != null) {
          return ConfigSection.MerchantID;
        }
        else if (ConfigurationSettings.AppSettings["GoogleMerchantID"] != null)
          return long.Parse(
            ConfigurationSettings.AppSettings["GoogleMerchantID"]);
        else {
          throw new ConfigurationException(
            "Set the 'GoogleMerchantID' key in the config file.");
        }
      }
    }

    /// <summary>
    /// Your alpha-numeric Merchant Key. To see it, log in to
    ///Google, select the Settings tab, click the Integration link.
    /// </summary>
    public static string MerchantKey {
      get {
        if (ConfigSection != null) {
          return ConfigSection.MerchantKey;
        }
        else if (ConfigurationSettings.AppSettings["GoogleMerchantKey"] != null)
          return ConfigurationSettings.AppSettings["GoogleMerchantKey"];
        else {
          throw new ConfigurationException(
            "Set the 'GoogleMerchantKey' key in the config file.");
        }
      }
    }

    /// <summary>
    /// The Current Environment
    /// </summary>
    public static EnvironmentType Environment {
      get {
        if (ConfigSection != null) {
          return ConfigSection.Environment;
        }
        else if (ConfigurationSettings.AppSettings["GoogleEnvironment"]
          != null)
          return (EnvironmentType)Enum.Parse(typeof(EnvironmentType),
            ConfigurationSettings.AppSettings["GoogleEnvironment"], true);
        else {
          throw new ConfigurationException(
            "Set the 'GoogleMerchantKey' key in the config file.");
        }
      }
    }

    /// <summary>
    /// The &lt;platform-id&gt; tag should only be used by eCommerce providers
    /// who make API requests on behalf of a merchant. The tag's value contains
    /// a Google Checkout merchant ID that identifies the eCommerce provider.
    /// </summary>
    /// <remarks>
    /// <seealso href="http://code.google.com/apis/checkout/developer/index.html#tag_platform-id"/>
    /// </remarks>
    public static long PlatformID {
      get {
        if (ConfigSection != null) {
          if (ConfigSection.PlatformID <= 0) {
            throw new ConfigurationException(
              "Set the 'PlatformID' attribute in the config section.");
          }
          return ConfigSection.PlatformID;
        }
        else if (ConfigurationSettings.AppSettings["PlatformID"] != null)
          return long.Parse(
            ConfigurationSettings.AppSettings["PlatformID"]);
        else {
          throw new ConfigurationException(
            "Set the 'PlatformID' key in the config file.");
        }
      }
    }

    /// <summary>
    /// Is Logging Configured
    /// </summary>
    public static bool Logging {
      get {
        if (ConfigSection != null) {
          return ConfigSection.Logging;
        }
        else if (ConfigurationSettings.AppSettings["Logging"] != null)
          return ConfigurationSettings.AppSettings["Logging"] == "true";
        else {
          throw new ConfigurationException(
            "Set the 'Logging' key in the config file.");
        }
      }
    }

    /// <summary>
    /// The Log Directory
    /// </summary>
    public static string LogDirectory {
      get {
        if (ConfigSection != null) {
          return ConfigSection.LogDirectory;
        }
        else if (ConfigurationSettings.AppSettings["LogDirectory"] != null)
          return ConfigurationSettings.AppSettings["LogDirectory"];
        else {
          throw new ConfigurationException(
            "Set the 'LogDirectory' key in the config file.");
        }
      }
    }

    /// <summary>
    /// The currency attribute identifies the unit of currency associated 
    /// with the tag's value. The value of the currency attribute 
    /// should be a three-letter ISO 4217 currency code.
    /// </summary>
    public static string Currency {
      get {
        Initialize();
        if (ConfigSection != null) {
          if (ConfigSection.Currency == null ||
            ConfigSection.Currency == string.Empty) {
            throw new ConfigurationException(
              "Set the 'Currency' key in the config section.");
          }
          return ConfigSection.Currency;
        }
        else if (ConfigurationSettings.AppSettings["Currency"] != null)
          return ConfigurationSettings.AppSettings["Currency"];
        else {
          throw new ConfigurationException(
            "Set the 'Currency' key in the config file.");
        }
      }
    }

    /// <summary>
    /// Create a new Configuration helper.
    /// </summary>
    public GCheckoutConfigurationHelper() {

    }

    /// <summary>
    /// Helper method for retrieving enum values from XmlNode.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="attribute"></param>
    /// <param name="enumType"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    internal static XmlNode GetEnumValue(XmlNode node, string attribute,
      Type enumType, ref int val) {
      XmlNode a = node.Attributes.RemoveNamedItem(attribute);
      if (a == null)
        throw new ConfigurationException("Google Checkout Config Section " + 
          "attribute required: " + attribute);
      if (Enum.IsDefined(enumType, a.Value))
        val = (int)Enum.Parse(enumType, a.Value, true);
      else
        throw new ConfigurationException("Google Checkout Config Section " + 
          "Invalid Enumeration Value: '" + a.Value + "'", a);
      return a;
    }

    /// <summary>
    /// Helper method for retrieving string values from xmlnode.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="attribute"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    internal static XmlNode GetStringValue(XmlNode node, string attribute,
      ref string val) {
      return GetStringValue(node, attribute, true, ref val);
    }

    /// <summary>
    /// Helper method for retrieving string values from xmlnode.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="attribute"></param>
    /// <param name="required"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    internal static XmlNode GetStringValue(XmlNode node, string attribute,
      bool required, ref string val) {
      XmlNode a = node.Attributes.RemoveNamedItem(attribute);
      if (a == null) {
        if (required)
          throw new ConfigurationException("Google Checkout Config Section " + 
            "attribute required: " + attribute);
      }  
      else
        val = a.Value;
      return a;
    }
  }
}
