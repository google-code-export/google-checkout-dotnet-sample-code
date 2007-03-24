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

    /// <summary>
    /// Obsolete method
    /// </summary>
    /// <param name="ItemName">Identifies the name of an item</param>
    /// <param name="ItemDescription">Contains a description of an item</param>
    /// <param name="Quantity">Identifies how many units of a particular item
    ///  are included in the customer's shopping cart.</param>
    /// <param name="UnitPrice">Identifies the cost of the item. 
    /// This tag has one required attribute, which identifies the
    /// currency of the price.</param>
    /// <param name="TaxTableSelector">identifies an alternate tax table that 
    /// should be used to calculate tax for a particular item. 
    /// The value of the &lt;tax-table-selector&gt; tag should correspond to the 
    /// value of the name attribute of an alternate-tax-table</param>
    /// <param name="MerchantPrivateItemData"></param>
    [Obsolete("MerchantPrivateItemData Is no longer a string, please use MerchantPrivateItemDataNodes")]
    public OrderLine(string ItemName, string ItemDescription, int Quantity,
      decimal UnitPrice, string TaxTableSelector,
      string MerchantPrivateItemData) {
      _ItemName = ItemName;
      _ItemDescription = ItemDescription;
      _Quantity = Quantity;
      _UnitPrice = UnitPrice;
      _TaxTableSelector = TaxTableSelector;
      if (MerchantPrivateItemData != null)
        _MerchantPrivateItemDataNodes = new System.Xml.XmlNode[] { 
            Checkout.CheckoutShoppingCartRequest.MakeXmlElement(MerchantPrivateItemData)};
    }

    /// <summary>
    /// Contains information about an individual item
    ///  listed in the customer's shopping cart
    /// </summary>
    /// <param name="ItemName">Identifies the name of an item</param>
    /// <param name="ItemDescription">Contains a description of an item</param>
    /// <param name="Quantity">Identifies how many units of a particular item
    ///  are included in the customer's shopping cart.</param>
    /// <param name="UnitPrice">Identifies the cost of the item. 
    /// This tag has one required attribute, which identifies the
    /// currency of the price.</param>
    /// <param name="TaxTableSelector">identifies an alternate tax table that 
    /// should be used to calculate tax for a particular item. 
    /// The value of the &lt;tax-table-selector&gt; tag should correspond to the 
    /// value of the name attribute of an alternate-tax-table</param>
    /// <param name="MerchantPrivateItemDataNodes">tag contains any well-formed 
    /// XML sequence that should accompany an individual item in an order. 
    /// Google Checkout will return this XML in the 
    /// &lt;merchant-calculation-callback&gt;
    ///  and the &lt;new-order-notification&gt; for the order.</param>
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

    /// <summary>
    /// Identifies the name of an item
    /// </summary>
    public string ItemName {
      get {
        return _ItemName;
      }
    }

    /// <summary>
    /// Contains a description of an item
    /// </summary>
    public string ItemDescription {
      get {
        return _ItemDescription;
      }
    }

    /// <summary>
    /// Identifies how many units of a particular item
    ///  are included in the customer's shopping cart
    /// </summary>
    public int Quantity {
      get {
        return _Quantity;
      }
    }

    /// <summary>
    /// Identifies the cost of the item. 
    /// This tag has one required attribute, which identifies the
    /// currency of the price
    /// </summary>
    public decimal UnitPrice {
      get {
        return _UnitPrice;
      }
    }

    /// <summary>
    /// identifies an alternate tax table that 
    /// should be used to calculate tax for a particular item. 
    /// The value of the &lt;tax-table-selector&gt; tag should correspond to the 
    /// value of the name attribute of an alternate-tax-table
    /// </summary>
    public string TaxTableSelector {
      get {
        return _TaxTableSelector;
      }
    }

    /// <summary>
    /// Obsolete
    /// </summary>
    [Obsolete("This property must be converted to a XmlNode Array. Only the First XmlNode will be returned.")]
    public string MerchantPrivateItemData {
      get {
        if (_MerchantPrivateItemDataNodes != null
          && _MerchantPrivateItemDataNodes.Length > 0)
          return _MerchantPrivateItemDataNodes[0].OuterXml;
        
        return string.Empty;
      }
    }

    /// <summary>
    /// An Array of <see cref="System.Xml.XmlNode" /> for the Merchant Private Data
    /// </summary>
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
