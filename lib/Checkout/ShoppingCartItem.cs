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
/*
 Edit History:
 *  August 2008   Joe Feser joe.feser@joefeser.com
 *  Initial Release.
 * 
*/
using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;
using GCheckout.Util;
using GCheckout.Checkout;

namespace GCheckout.Checkout {

  /// <summary>
  /// A wrapper containing information about an individual item listed 
  /// in the customer's shopping cart
  /// </summary>
  public class ShoppingCartItem : IShoppingCartItem, ICloneable {

    private string _name;
    private string _description;
    private decimal _price;
    private int _quantity;
    private string _merchantItemID;
    private XmlNode[] _merchantPrivateItemDataNodes = new XmlNode[] {};
    private AlternateTaxTable _taxTable;
    private DigitalItem _digitalContent;
    private double _itemWeight;
    //this is only used by the callback method and should not be used by the cart
    private string _taxtableselector = string.Empty;
    //we want the for the table table information.
    private GCheckout.AutoGen.Item _notificationItem;

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
    /// A legacy method of allowing the private data be set with a string
    /// </summary>
    public string MerchantPrivateItemData {
      get {
        foreach (XmlNode node in MerchantPrivateItemDataNodes) {
          if (node.Name == CheckoutShoppingCartRequest.MERCHANT_DATA_HIDDEN)
            return node.InnerXml;
        }
        //if we get this far and we have one node, just return it
        if (MerchantPrivateItemDataNodes.Length == 1)
          return MerchantPrivateItemDataNodes[0].OuterXml;

        return null;
      }
      set {
        if (value == null)
          value = string.Empty;

        XmlNode merchantNode = null;

        foreach (XmlNode node in MerchantPrivateItemDataNodes) {
          if (node.Name == CheckoutShoppingCartRequest.MERCHANT_DATA_HIDDEN) {
            merchantNode = node;
            break;
          }
        }

        //we must delete the data
        if (value == string.Empty) {
          //we are not going to remove the node. we are just going to set it
          if (merchantNode != null){
            merchantNode.InnerXml = string.Empty;
          }
        }
        else {
          if (merchantNode == null) {
            //we need to copy the array and then add this item to it.
            merchantNode = CheckoutShoppingCartRequest.MakeXmlElement(value);
            //now put the node in the array.
            XmlNode[] nodes 
              = new XmlNode[MerchantPrivateItemDataNodes.Length + 1];
            MerchantPrivateItemDataNodes.CopyTo(nodes, 0);
            nodes[nodes.Length - 1] = merchantNode;
            _merchantPrivateItemDataNodes = nodes;
          }
          else {
            merchantNode.InnerXml = value;   
          }
        }
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
        if (value == null)
          value = new XmlNode[] {};

        //see if there is a private node set as a string first
        string mpd = null;
        foreach (XmlNode node in MerchantPrivateItemDataNodes) {
          if (node.Name == CheckoutShoppingCartRequest.MERCHANT_DATA_HIDDEN)
            mpd = node.InnerXml;
        }

        _merchantPrivateItemDataNodes = value;

        if (mpd != null && mpd.Length > 0) {
          MerchantPrivateItemData = mpd;   
        }
      }
    }

    /// <summary>
    /// Identifies an alternate tax table that should be
    /// used to calculate tax for a particular item.
    /// </summary>
    public virtual AlternateTaxTable TaxTable {
      get {
        if (_notificationItem != null)
          throw new NotImplementedException("This Item was built from a " + 
          "new-order-notification and has no knowledge of a tax table. " +
          "Please call 'TaxTableSelector' to read the tax-table-selector property.");
        if (_taxTable == null)
          _taxTable = new AlternateTaxTable();
        return _taxTable;
      }
      set {
        _taxTable = value;
      }
    }

    /// <summary>
    /// Property used only when this object was built from an 
    /// <seealso cref="GCheckout.AutoGen.Item"/> object.
    /// </summary>
    public virtual string TaxTableSelector {
      get {
        if (_notificationItem == null)
          throw new NotImplementedException("This Item was not built from a " + 
            "new-order-notification. " +
            "Please call 'TaxTable' to read the AlternateTaxTable for a " +
            "GCheckoutRequest.");
        if (_notificationItem.taxtableselector == null)
          _notificationItem.taxtableselector= string.Empty;
        return _notificationItem.taxtableselector;
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

    /// <summary>
    /// The &lt;item-weight&gt; tag specifies the weight of an 
    /// individual item in the customer's shopping cart.
    /// </summary>
    public virtual double Weight {
      get {
        return _itemWeight;
      }
      set {
        if (value < 0)
          throw new 
            ArgumentOutOfRangeException("The value must be 0 or larger");
        _itemWeight = value; 
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
        this.MerchantPrivateItemDataNodes 
          = theItem.merchantprivateitemdata.Any;
      else
        this.MerchantPrivateItemDataNodes = new XmlNode[] {};
      this.Name = theItem.itemname;
      this.Price = theItem.unitprice.Value;
      this.Quantity = theItem.quantity;
      _taxtableselector = theItem.taxtableselector;
      _notificationItem = theItem;
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
      string merchantPrivateItemData) {
      Name = name;
      Description = description;
      if (merchantItemID == string.Empty)
        merchantItemID = null;
      MerchantItemID = merchantItemID;
      Price = price;
      Quantity = quantity;
      MerchantPrivateItemData = merchantPrivateItemData;
      TaxTable = taxTable;
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
      if (merchantItemID == string.Empty)
        merchantItemID = null;
      MerchantItemID = merchantItemID;
      Price = price;
      Quantity = quantity;
      MerchantPrivateItemDataNodes = merchantPrivateItemData;
      TaxTable = taxTable;
    }

    #region ICloneable Members

    /// <summary>
    /// Clone the item
    /// </summary>
    /// <returns></returns>
    public object Clone() {
      ShoppingCartItem retVal = this.MemberwiseClone() as ShoppingCartItem;

      //clone the xml
      XmlNode[] nodes = new XmlNode[this.MerchantPrivateItemDataNodes.Length];
      
      for (int i = 0; i < nodes.Length; i++) {
        nodes[i] = MerchantPrivateItemDataNodes[i].Clone();
      }

      retVal.MerchantPrivateItemDataNodes = nodes;

      //clone the digital item
      if (DigitalContent != null)
        retVal.DigitalContent = DigitalContent.Clone() as DigitalItem;

      return retVal;
    }

    #endregion

  }
}
