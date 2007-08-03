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
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;
using GCheckout.Util;

namespace GCheckout.Checkout {

  /// <summary>
  /// A wrapper containing information about an individual item listed 
  /// in the customer's shopping cart
  /// </summary>
  public class ShoppingCartItem : IShoppingCartItem {

    private string _name;
    private string _description;
    private decimal _price = 0.0m;
    private int _quantity = 0;
    private string _merchantItemID;
    private XmlNode[] _merchantPrivateItemDataNodes;
    private AlternateTaxTable _taxTable = null;
    private DigitalItem _digitalContent = null;

    /// <summary>
    /// Identifies the name of an item
    /// </summary>
    public virtual string Name {
      get {
        return _name;
      }
      set {
        _name = EncodeHelper.CleanString(value);
      }
    }

    /// <summary>
    /// Contains a description of an item
    /// </summary>
    public virtual string Description {
      get {
        return _description;
      }
      set {
        _description = EncodeHelper.CleanString(value);
      }
    }

    /// <summary>
    /// The cost of the item. This tag has one required attribute, 
    /// which identifies the currency of the price
    /// </summary>
    public virtual decimal Price {
      get {
        return _price;
      }
      set {
        _price = value;
      }
    }

    /// <summary>
    /// Identifies how many units of a particular item are 
    /// included in the customer's shopping cart.
    /// </summary>
    public virtual int Quantity {
      get {
        return _quantity;
      }
      set {
        _quantity = value;
      }
    }

    /// <summary>
    /// Contains a value, such as a stock keeping unit (SKU), 
    /// that you use to uniquely identify an item.
    /// </summary>
    /// <remarks>Google Checkout will include this value in 
    /// the merchant calculation 
    /// callbacks and the new order notification for the order.</remarks>
    public virtual string MerchantItemID {
      get {
        return _merchantItemID;
      }
      set {
        _merchantItemID = EncodeHelper.CleanString(value);
      }
    }

    /// <summary>
    /// Contains any well-formed XML sequence that should 
    /// accompany an individual item in an order.
    /// </summary>
    public virtual XmlNode[] MerchantPrivateItemDataNodes {
      get {
        return _merchantPrivateItemDataNodes;
      }
      set {
        _merchantPrivateItemDataNodes = value;
      }
    }

    /// <summary>
    /// Identifies an alternate tax table that should be
    /// used to calculate tax for a particular item.
    /// </summary>
    public virtual AlternateTaxTable TaxTable {
      get {
        return _taxTable;
      }
      set {
        _taxTable = value;
      }
    }

    /// <summary>
    /// Contains information relating to digital
    /// delivery of an item.
    /// </summary>
    public virtual DigitalItem DigitalContent {
      get {
        return _digitalContent;
      }
      set {
        _digitalContent = value;
      }
    }

    string _taxtableselector = string.Empty;

    /// <summary>
    /// Property used to just return the name of the Tax Table
    /// </summary>
    public virtual string TaxTableName {
      get {
        if (_taxTable != null && _taxTable.Name != null)
          return _taxTable.Name;
        if (_taxtableselector != null && _taxtableselector.Length > 0)
          return _taxtableselector;
        return string.Empty;
      } 
    }

    /// <summary>
    /// Initialize a new instance of the 
    /// <see cref="ShoppingCartItem"/> class, which creates an object
    /// corresponding to an individual item in an order. This method 
    /// is used for items that do not have an associated
    /// &lt;merchant-private-item-data&gt; XML block.
    /// </summary>
    public ShoppingCartItem() {
       
    }

    /// <summary>
    /// ctor used by the extended notification class
    /// </summary>
    /// <param name="theItem"></param>
    internal ShoppingCartItem(GCheckout.AutoGen.Item theItem) {
      if (theItem.digitalcontent != null)
        this.DigitalContent = new DigitalItem(theItem.digitalcontent);
      this.Description = theItem.itemdescription;
      this.MerchantItemID = theItem.merchantitemid;
      if (theItem.merchantprivateitemdata != null)
        this.MerchantPrivateItemDataNodes = theItem.merchantprivateitemdata.Any;
      else
        this.MerchantPrivateItemDataNodes = new XmlNode[] {};
      this.Name = theItem.itemname;
      this.Price = theItem.unitprice.Value;
      this.Quantity = theItem.quantity;
      _taxtableselector = theItem.taxtableselector;
      //this.TaxTable = theItem.taxtableselector;
    }

    /// <summary>
    /// Initialize a new instance of the 
    /// <see cref="ShoppingCartItem"/> class, which creates an object
    /// corresponding to an individual item in an order. This method 
    /// is used for items that do not have an associated
    /// &lt;merchant-private-item-data&gt; XML block.
    /// </summary>
    /// <param name="name">The name of the item.</param>
    /// <param name="description">A description of the item.</param>
    /// <param name="merchantItemID">The Merchant Item Id that uniquely 
    /// identifies the product in your system. (optional)</param>
    /// <param name="price">The price of the item, per item.</param>
    /// <param name="quantity">The number of the item that is 
    /// included in the order.</param>
    public ShoppingCartItem(string name, string description,
      string merchantItemID, decimal price, int quantity)
      : this(name, description, merchantItemID, price,
      quantity, AlternateTaxTable.Empty, new XmlNode[] {}) {
    }

    /// <summary>
    /// Initialize a new instance of the 
    /// <see cref="ShoppingCartItem"/> class, which creates an object
    /// corresponding to an individual item in an order. This method 
    /// is used for items that do have an associated
    /// &lt;merchant-private-item-data&gt; XML block.
    /// </summary>
    /// <param name="name">The name of the item.</param>
    /// <param name="description">A description of the item.</param>
    /// <param name="merchantItemID">The Merchant Item Id that uniquely 
    /// identifies the product in your system. (optional)</param>
    /// <param name="price">The price of the item, per item.</param>
    /// <param name="quantity">The number of the item that is 
    /// included in the order.</param>
    /// <param name="merchantPrivateItemData">The merchant private
    /// item data associated with the item.</param>
    /// <param name="taxTable">The <see cref="AlternateTaxTable"/> 
    /// To assign to the item </param>
    public ShoppingCartItem(string name, string description,
      string merchantItemID, decimal price, int quantity,
      AlternateTaxTable taxTable,
      params XmlNode[] merchantPrivateItemData) {
      Name = name;
      Description = description;
      if (MerchantItemID == string.Empty)
        MerchantItemID = null;
      MerchantItemID = merchantItemID;
      Price = price;
      Quantity = quantity;
      MerchantPrivateItemDataNodes = merchantPrivateItemData;
      TaxTable = taxTable;
    }
  }
}
