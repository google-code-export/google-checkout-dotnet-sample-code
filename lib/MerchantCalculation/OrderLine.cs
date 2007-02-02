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
using System.Xml;

namespace GCheckout.MerchantCalculation {
  /// <summary>
  /// This class creates an object for an individual item identified in a 
  /// &lt;merchant-calculation-callback&gt; request. Your class that inherits 
  /// from CallbackRules.cs can access these objects to locate information 
  /// about each item in an order. See the GetTaxResult method in the 
  /// ExampleRules.cs file for an example of how your application might access 
  /// these objects.
  /// </summary>
  public class OrderLine {
    private string _ItemName;
    private string _ItemDescription;
    private int _Quantity;
    private decimal _UnitPrice;
    private string _TaxTableSelector;
    private XmlNode[] _MerchantPrivateItemDataNodes = new XmlNode[] {};

    [Obsolete("MerchantPrivateItemData Is no longer a string, please use MerchantPrivateItemDataNodes")]
    public OrderLine(string ItemName, string ItemDescription, int Quantity,
      decimal UnitPrice, string TaxTableSelector,
      string MerchantPrivateItemData)
    {
      _ItemName = ItemName;
      _ItemDescription = ItemDescription;
      _Quantity = Quantity;
      _UnitPrice = UnitPrice;
      _TaxTableSelector = TaxTableSelector;
      if (MerchantPrivateItemData != null)
        _MerchantPrivateItemDataNodes = new System.Xml.XmlNode[] 
          { Checkout.CheckoutShoppingCartRequest.MakeXmlElement(MerchantPrivateItemData)};
    }

    public OrderLine(string ItemName, string ItemDescription, int Quantity,
      decimal UnitPrice, string TaxTableSelector,
      XmlNode[] MerchantPrivateItemDataNodes) {
      _ItemName = ItemName;
      _ItemDescription = ItemDescription;
      _Quantity = Quantity;
      _UnitPrice = UnitPrice;
      _TaxTableSelector = TaxTableSelector;
      if (MerchantPrivateItemDataNodes != null)
        _MerchantPrivateItemDataNodes = MerchantPrivateItemDataNodes;
    }

    public string ItemName {
      get {
        return _ItemName;
      }
    }

    public string ItemDescription {
      get {
        return _ItemDescription;
      }
    }

    public int Quantity {
      get {
        return _Quantity;
      }
    }

    public decimal UnitPrice {
      get {
        return _UnitPrice;
      }
    }

    public string TaxTableSelector {
      get {
        return _TaxTableSelector;
      }
    }

    [Obsolete("This property must be converted to a XmlNode Array. Only the First XmlNode will be returned.")]
    public string MerchantPrivateItemData {
      get {
        if (_MerchantPrivateItemDataNodes != null
          && _MerchantPrivateItemDataNodes.Length > 0)
          return _MerchantPrivateItemDataNodes[0].OuterXml;
        
        return string.Empty;
      }
    }

    public System.Xml.XmlNode[] MerchantPrivateDataNodes {
      get {
        if (_MerchantPrivateItemDataNodes != null)
          return _MerchantPrivateItemDataNodes;
        else
          return new XmlNode[] {};
      }
    }

  }
}
