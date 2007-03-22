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
using System.Text;
using System.Diagnostics;

namespace GCheckout.Checkout {
  /// <summary>
  /// Class used to create the structure needed by Google Checkout
  /// </summary>
  /// <remarks>
  /// The class also has the ability to send that request to Google or return
  /// the Xml needed to place in the hidden form fields.
  /// </remarks>
  public class CheckoutShoppingCartRequest : GCheckoutRequest {

    internal const string ZIP_CODE_PATTERN_EXCEPTION = 
      "Zip code patterns must be five " +
      "numeric characters, or zero to 4 numeric characters followed by " +
      "a single asterisk as a wildcard character.";

    private ArrayList _Items;
    private AutoGen.TaxTables _TaxTables;
    private AutoGen.MerchantCheckoutFlowSupportShippingmethods
      _ShippingMethods;
    private string _MerchantPrivateData = null;
    private ArrayList _MerchantPrivateDataNodes = new ArrayList();
    private bool _AcceptMerchantCoupons = false;
    private bool _AcceptMerchantGiftCertificates = false;
    private string _MerchantCalculationsUrl = null;
    private string _ContinueShoppingUrl = null;
    private string _EditCartUrl = null;
    private bool _RequestBuyerPhoneNumber = false;
    private DateTime _CartExpiration = DateTime.MinValue;
    private string _Currency = null;
    private string _AnalyticsData = null;
    private long _PlatformID = 0;
    
    private ParameterizedUrls _ParameterizedUrls = new ParameterizedUrls();

    //jf Tax Tables added 3/1/2007
    private AlternateTaxTableCollection _alternateTaxTables = new AlternateTaxTableCollection();


    /// <summary>
    /// This method is called by the <see cref="GCheckoutButton"/> class and
    /// initializes a new instance of the 
    /// <see cref="CheckoutShoppingCartRequest"/> class.
    /// </summary>
    /// <param name="MerchantID">The Google Checkout merchant ID assigned
    /// to a particular merchant.</param>
    /// <param name="MerchantKey">The Google Checkout merchant key assigned
    /// to a particular merchant.</param>
    /// <param name="Env">The environment where a request is being executed. 
    /// Valid values for this parameter are "Sandbox" and "Production".</param>
    /// <param name="Currency">The currency associated with prices in a 
    /// Checkout API request. At this time, the only supported currency value 
    /// is "USD", which corresponds to U.S. dollars.</param>
    /// <param name="CartExpirationMinutes">
    /// The length of time, in minutes, after which the shopping cart will 
    /// expire if it has not been submitted. A value of <b>0</b> indicates 
    /// the cart does not expire.
    /// </param>
    public CheckoutShoppingCartRequest(string MerchantID, string MerchantKey,
      EnvironmentType Env, string Currency, int CartExpirationMinutes) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = Env;
      _Items = new ArrayList();
      _TaxTables = new AutoGen.TaxTables();
      _TaxTables.defaulttaxtable = new AutoGen.DefaultTaxTable();
      _TaxTables.defaulttaxtable.taxrules = new AutoGen.DefaultTaxRule[0];
      _ShippingMethods =
        new AutoGen.MerchantCheckoutFlowSupportShippingmethods();
      _ShippingMethods.Items = new Object[0];
      _Currency = Currency;
      if (CartExpirationMinutes > 0) {
        SetExpirationMinutesFromNow(CartExpirationMinutes);
      }
    }

    /// <summary>
    /// This method adds an item to an order. This method handles items that 
    /// do not have &lt;merchant-private-item-data&gt; XML blocks associated 
    /// with them.
    /// </summary>
    /// <param name="Name">The name of the item. This value corresponds to the 
    /// value of the &lt;item-name&gt; tag in the Checkout API request.</param>
    /// <param name="Description">The description of the item. This value 
    /// corresponds to the value of the &lt;item-description&gt; tag in the 
    /// Checkout API request.</param>
    /// <param name="Price">The price of the item. This value corresponds to 
    /// the value of the &lt;unit-price&gt; tag in the Checkout API 
    /// request.</param>
    /// <param name="Quantity">The number of this item that is included in the 
    /// order. This value corresponds to the value of the &lt;quantity&gt; tag 
    /// in the Checkout API request.</param>
    public void AddItem(string Name, string Description, decimal Price,
      int Quantity) {
      _Items.Add(new ShoppingCartItem(Name, Description, string.Empty, Price, 
        Quantity));
    }

    /// <summary>
    /// This method adds an item to an order. This method handles items that 
    /// do not have &lt;merchant-private-item-data&gt; XML blocks associated 
    /// with them.
    /// </summary>
    /// <param name="Name">The name of the item. This value corresponds to the 
    /// value of the &lt;item-name&gt; tag in the Checkout API request.</param>
    /// <param name="Description">The description of the item. This value 
    /// corresponds to the value of the &lt;item-description&gt; tag in the 
    /// Checkout API request.</param>
    /// <param name="Price">The price of the item. This value corresponds to 
    /// the value of the &lt;unit-price&gt; tag in the Checkout API 
    /// request.</param>
    /// <param name="Quantity">The number of this item that is included in the 
    /// order. This value corresponds to the value of the &lt;quantity&gt; tag 
    /// in the Checkout API request.</param>
    /// <param name="taxTable">The <see cref="AlternateTaxTable"/> 
    /// To assign to the item </param>
    public void AddItem(string Name, string Description, decimal Price,
      int Quantity, AlternateTaxTable taxTable) {
      _alternateTaxTables.VerifyTaxRule(taxTable);
      _Items.Add(new ShoppingCartItem(Name, Description, string.Empty, Price,
        Quantity, taxTable));
    }

    /// <summary>
    /// This method adds an item to an order. This method handles items that 
    /// do not have &lt;merchant-private-item-data&gt; XML blocks associated 
    /// with them.
    /// </summary>
    /// <param name="Name">The name of the item. This value corresponds to the 
    /// value of the &lt;item-name&gt; tag in the Checkout API request.</param>
    /// <param name="Description">The description of the item. This value 
    /// corresponds to the value of the &lt;item-description&gt; tag in the 
    /// Checkout API request.</param>
    /// <param name="MerchantItemID">The Merchant Item Id that uniquely
    /// identifies the product in your system.</param>
    /// <param name="Price">The price of the item. This value corresponds to 
    /// the value of the &lt;unit-price&gt; tag in the Checkout API 
    /// request.</param>
    /// <param name="Quantity">The number of this item that is included in the 
    /// order. This value corresponds to the value of the &lt;quantity&gt; tag 
    /// in the Checkout API request.</param>
    public void AddItem(string Name, string Description, string MerchantItemID,
      decimal Price, int Quantity) {
      _Items.Add(new ShoppingCartItem(Name, Description, MerchantItemID, Price,
        Quantity));
    }

    /// <summary>
    /// This method adds an item to an order. This method handles items that 
    /// do not have &lt;merchant-private-item-data&gt; XML blocks associated 
    /// with them.
    /// </summary>
    /// <param name="Name">The name of the item. This value corresponds to the 
    /// value of the &lt;item-name&gt; tag in the Checkout API request.</param>
    /// <param name="Description">The description of the item. This value 
    /// corresponds to the value of the &lt;item-description&gt; tag in the 
    /// Checkout API request.</param>
    /// <param name="MerchantItemID">The Merchant Item Id that uniquely
    /// identifies the product in your system.</param>
    /// <param name="Price">The price of the item. This value corresponds to 
    /// the value of the &lt;unit-price&gt; tag in the Checkout API 
    /// request.</param>
    /// <param name="Quantity">The number of this item that is included in the 
    /// order. This value corresponds to the value of the &lt;quantity&gt; tag 
    /// in the Checkout API request.</param>
    /// <param name="taxTable">The <see cref="AlternateTaxTable"/> 
    /// To assign to the item </param>
    public void AddItem(string Name, string Description, string MerchantItemID,
      decimal Price, int Quantity, AlternateTaxTable taxTable) {
      _alternateTaxTables.VerifyTaxRule(taxTable);
      _Items.Add(new ShoppingCartItem(Name, Description, MerchantItemID, Price,
        Quantity, taxTable));
    }

    /// <summary>
    /// This method adds an item to an order. This method handles items that 
    /// have &lt;merchant-private-item-data&gt; XML blocks associated with them.
    /// </summary>
    /// <param name="Name">The name of the item. This value corresponds to the 
    /// value of the &lt;item-name&gt; tag in the Checkout API request.</param>
    /// <param name="Description">The description of the item. This value 
    /// corresponds to the value of the &lt;item-description&gt; tag in the 
    /// Checkout API request.</param>
    /// <param name="Price">The price of the item. This value corresponds to 
    /// the value of the &lt;unit-price&gt; tag in the Checkout API 
    /// request.</param>
    /// <param name="Quantity">The number of this item that is included in the 
    /// order. This value corresponds to the value of the &lt;quantity&gt; tag 
    /// in the Checkout API request.</param>
    /// <param name="MerchantPrivateItemData">An XML block that should be 
    /// associated with the item in the Checkout API request. This value 
    /// corresponds to the value of the value of the 
    /// &lt;merchant-private-item-data&gt; tag in the Checkout API 
    /// request.</param>
    [Obsolete("MerchantPrivateData is now a XmlNode Array. Please use one of the" + 
              " AddItem methods that take in a XmlNode Array.")]
    public void AddItem(string Name, string Description, decimal Price,
      int Quantity, string MerchantPrivateItemData) {
      _Items.Add(new ShoppingCartItem(Name, Description, string.Empty, Price, 
        Quantity, AlternateTaxTable.Empty, MakeXmlElement(MerchantPrivateItemData)));
    }

    /// <summary>
    /// This method adds an item to an order. This method handles items that 
    /// have &lt;merchant-private-item-data&gt; XML blocks associated with them.
    /// </summary>
    /// <param name="Name">The name of the item. This value corresponds to the 
    /// value of the &lt;item-name&gt; tag in the Checkout API request.</param>
    /// <param name="Description">The description of the item. This value 
    /// corresponds to the value of the &lt;item-description&gt; tag in the 
    /// Checkout API request.</param>
    /// <param name="Price">The price of the item. This value corresponds to 
    /// the value of the &lt;unit-price&gt; tag in the Checkout API 
    /// request.</param>
    /// <param name="Quantity">The number of this item that is included in the 
    /// order. This value corresponds to the value of the &lt;quantity&gt; tag 
    /// in the Checkout API request.</param>
    /// <param name="MerchantPrivateItemData">An XML node that should be 
    /// associated with the item in the Checkout API request. This value 
    /// corresponds to the value of the value of the 
    /// &lt;merchant-private-item-data&gt; tag in the Checkout API 
    /// request.</param>
    public void AddItem(string Name, string Description, decimal Price,
      int Quantity, XmlNode MerchantPrivateItemData) {
      _Items.Add(new ShoppingCartItem(Name, Description, string.Empty, Price, 
        Quantity, AlternateTaxTable.Empty, MerchantPrivateItemData));
    }

    /// <summary>
    /// This method adds an item to an order. This method handles items that 
    /// have &lt;merchant-private-item-data&gt; XML blocks associated with them.
    /// </summary>
    /// <param name="Name">The name of the item. This value corresponds to the 
    /// value of the &lt;item-name&gt; tag in the Checkout API request.</param>
    /// <param name="Description">The description of the item. This value 
    /// corresponds to the value of the &lt;item-description&gt; tag in the 
    /// Checkout API request.</param>
    /// <param name="Price">The price of the item. This value corresponds to 
    /// the value of the &lt;unit-price&gt; tag in the Checkout API 
    /// request.</param>
    /// <param name="Quantity">The number of this item that is included in the 
    /// order. This value corresponds to the value of the &lt;quantity&gt; tag 
    /// in the Checkout API request.</param>
    /// <param name="MerchantPrivateItemData">An XML node that should be 
    /// associated with the item in the Checkout API request. This value 
    /// corresponds to the value of the value of the 
    /// &lt;merchant-private-item-data&gt; tag in the Checkout API 
    /// request.</param>
    /// <param name="taxTable">The <see cref="AlternateTaxTable"/> 
    /// To assign to the item </param>
    public void AddItem(string Name, string Description, decimal Price,
      int Quantity, XmlNode MerchantPrivateItemData, 
      AlternateTaxTable taxTable) {
      _alternateTaxTables.VerifyTaxRule(taxTable);
      _Items.Add(new ShoppingCartItem(Name, Description, string.Empty, Price, 
        Quantity, taxTable, MerchantPrivateItemData));
    }

    /// <summary>
    /// This method adds an item to an order. This method handles items that 
    /// have &lt;merchant-private-item-data&gt; XML blocks associated with them.
    /// </summary>
    /// <param name="Name">The name of the item. This value corresponds to the 
    /// value of the &lt;item-name&gt; tag in the Checkout API request.</param>
    /// <param name="Description">The description of the item. This value 
    /// corresponds to the value of the &lt;item-description&gt; tag in the 
    /// Checkout API request.</param>
    /// <param name="MerchantItemID">The Merchant Item Id that uniquely
    /// identifies the product in your system.</param>
    /// <param name="Price">The price of the item. This value corresponds to 
    /// the value of the &lt;unit-price&gt; tag in the Checkout API 
    /// request.</param>
    /// <param name="Quantity">The number of this item that is included in the 
    /// order. This value corresponds to the value of the &lt;quantity&gt; tag 
    /// in the Checkout API request.</param>
    /// <param name="MerchantPrivateItemData">An XML node that should be 
    /// associated with the item in the Checkout API request. This value 
    /// corresponds to the value of the value of the 
    /// &lt;merchant-private-item-data&gt; tag in the Checkout API 
    /// request.</param>
    public void AddItem(string Name, string Description, string MerchantItemID,
      decimal Price, int Quantity, XmlNode MerchantPrivateItemData) {
      _Items.Add(new ShoppingCartItem(Name, Description, MerchantItemID, Price, 
        Quantity, AlternateTaxTable.Empty, MerchantPrivateItemData));
    }

    /// <summary>
    /// This method adds an item to an order. This method handles items that 
    /// have &lt;merchant-private-item-data&gt; XML blocks associated with them.
    /// </summary>
    /// <param name="Name">The name of the item. This value corresponds to the 
    /// value of the &lt;item-name&gt; tag in the Checkout API request.</param>
    /// <param name="Description">The description of the item. This value 
    /// corresponds to the value of the &lt;item-description&gt; tag in the 
    /// Checkout API request.</param>
    /// <param name="MerchantItemID">The Merchant Item Id that uniquely
    /// identifies the product in your system.</param>
    /// <param name="Price">The price of the item. This value corresponds to 
    /// the value of the &lt;unit-price&gt; tag in the Checkout API 
    /// request.</param>
    /// <param name="Quantity">The number of this item that is included in the 
    /// order. This value corresponds to the value of the &lt;quantity&gt; tag 
    /// in the Checkout API request.</param>
    /// <param name="MerchantPrivateItemData">An XML node that should be 
    /// associated with the item in the Checkout API request. This value 
    /// corresponds to the value of the value of the 
    /// &lt;merchant-private-item-data&gt; tag in the Checkout API 
    /// request.</param>
    /// <param name="taxTable">The <see cref="AlternateTaxTable"/> 
    /// To assign to the item </param>
    public void AddItem(string Name, string Description, string MerchantItemID,
      decimal Price, int Quantity, XmlNode MerchantPrivateItemData, 
      AlternateTaxTable taxTable) {
      _alternateTaxTables.VerifyTaxRule(taxTable);
      _Items.Add(new ShoppingCartItem(Name, Description, MerchantItemID, Price, 
        Quantity, taxTable, MerchantPrivateItemData));
    }

    /// <summary>
    /// This method adds an item to an order. This method handles items that 
    /// have &lt;merchant-private-item-data&gt; XML blocks associated with them.
    /// </summary>
    /// <param name="Name">The name of the item. This value corresponds to the 
    /// value of the &lt;item-name&gt; tag in the Checkout API request.</param>
    /// <param name="Description">The description of the item. This value 
    /// corresponds to the value of the &lt;item-description&gt; tag in the 
    /// Checkout API request.</param>
    /// <param name="MerchantItemID">The Merchant Item Id that uniquely
    /// identifies the product in your system.</param>
    /// <param name="Price">The price of the item. This value corresponds to 
    /// the value of the &lt;unit-price&gt; tag in the Checkout API 
    /// request.</param>
    /// <param name="Quantity">The number of this item that is included in the 
    /// order. This value corresponds to the value of the &lt;quantity&gt; tag 
    /// in the Checkout API request.</param>
    /// <param name="MerchantPrivateItemData">An XML node array that should be 
    /// associated with the item in the Checkout API request. This value 
    /// corresponds to the value of the value of the 
    /// &lt;merchant-private-item-data&gt; tag in the Checkout API 
    /// request.</param>
    /// <param name="taxTable">The <see cref="AlternateTaxTable"/> 
    /// To assign to the item </param>
    public void AddItem(string Name, string Description, string MerchantItemID,
      decimal Price, int Quantity, AlternateTaxTable taxTable,
      params XmlNode[] MerchantPrivateItemData) {
      _alternateTaxTables.VerifyTaxRule(taxTable);
      _Items.Add(new ShoppingCartItem(Name, Description, MerchantItemID, Price, 
        Quantity, taxTable, MerchantPrivateItemData));
    }

    /// <summary>
    /// This method adds an item to an order. This method handles items that 
    /// have &lt;merchant-private-item-data&gt; XML blocks associated with them.
    /// </summary>
    /// <param name="Name">The name of the item. This value corresponds to the 
    /// value of the &lt;item-name&gt; tag in the Checkout API request.</param>
    /// <param name="Description">The description of the item. This value 
    /// corresponds to the value of the &lt;item-description&gt; tag in the 
    /// Checkout API request.</param>
    /// <param name="Price">The price of the item. This value corresponds to 
    /// the value of the &lt;unit-price&gt; tag in the Checkout API 
    /// request.</param>
    /// <param name="Quantity">The number of this item that is included in the 
    /// order. This value corresponds to the value of the &lt;quantity&gt; tag 
    /// in the Checkout API request.</param>
    /// <param name="MerchantPrivateItemData">An array of XML nodes that should be 
    /// associated with the item in the Checkout API request. This value 
    /// corresponds to the value of the value of the 
    /// &lt;merchant-private-item-data&gt; tag in the Checkout API 
    /// request.</param>
    public void AddItem(string Name, string Description, decimal Price,
      int Quantity, params XmlNode[] MerchantPrivateItemData) {
      _Items.Add(new ShoppingCartItem(Name, Description, string.Empty, Price, 
        Quantity, AlternateTaxTable.Empty, MerchantPrivateItemData));
    }

    /// <summary>
    /// This method adds an item to an order. This method handles items that 
    /// have &lt;merchant-private-item-data&gt; XML blocks associated with them.
    /// </summary>
    /// <param name="Name">The name of the item. This value corresponds to the 
    /// value of the &lt;item-name&gt; tag in the Checkout API request.</param>
    /// <param name="Description">The description of the item. This value 
    /// corresponds to the value of the &lt;item-description&gt; tag in the 
    /// Checkout API request.</param>
    /// <param name="MerchantItemID">The Merchant Item Id that uniquely 
    /// identifies the product in your system.</param>
    /// <param name="Price">The price of the item. This value corresponds to 
    /// the value of the &lt;unit-price&gt; tag in the Checkout API 
    /// request.</param>
    /// <param name="Quantity">The number of this item that is included in the 
    /// order. This value corresponds to the value of the &lt;quantity&gt; tag 
    /// in the Checkout API request.</param>
    /// <param name="MerchantPrivateItemData">An array of XML nodes that should be 
    /// associated with the item in the Checkout API request. This value 
    /// corresponds to the value of the value of the 
    /// &lt;merchant-private-item-data&gt; tag in the Checkout API 
    /// request.</param>
    public void AddItem(string Name, string Description, string MerchantItemID,
      decimal Price, int Quantity, params XmlNode[] MerchantPrivateItemData) {
      _Items.Add(new ShoppingCartItem(Name, Description, MerchantItemID, Price,
        Quantity, AlternateTaxTable.Empty, MerchantPrivateItemData));
    }

    /// <summary>
    /// This method adds a flat-rate shipping method to an order. This method 
    /// handles flat-rate shipping methods that do not have shipping 
    /// restrictions.
    /// </summary>
    /// <param name="Name">The name of the shipping method. This value will be 
    /// displayed on the Google Checkout order review page.</param>
    /// <param name="Cost">The cost associated with the shipping method.</param>
    public void AddFlatRateShippingMethod(string Name, decimal Cost) {
      AddFlatRateShippingMethod(Name, Cost, null);
    }

    /// <summary>
    /// This method adds a flat-rate shipping method to an order. This method 
    /// handles flat-rate shipping methods that have shipping restrictions.
    /// </summary>
    /// <param name="Name">The name of the shipping method. This value will be 
    /// displayed on the Google Checkout order review page.</param>
    /// <param name="Cost">The cost associated with the shipping method.</param>
    /// <param name="Restrictions">A list of country, state or zip code areas 
    /// where the shipping method is either available or unavailable.</param>
    public void AddFlatRateShippingMethod(string Name, decimal Cost,
      ShippingRestrictions Restrictions) {
      AutoGen.FlatRateShipping Method = new AutoGen.FlatRateShipping();
      Method.name = Name;
      Method.price = new AutoGen.FlatRateShippingPrice();
      Method.price.currency = _Currency;
      Method.price.Value = Cost;
      if (Restrictions != null) {
        Method.shippingrestrictions = Restrictions.XmlRestrictions;
      }
      AddNewShippingMethod(Method);
    }

    /// <summary>
    /// This method adds a merchant-calculated shipping method to an order. 
    /// This method handles merchant-calculated shipping methods that do not 
    /// have shipping restrictions.
    /// </summary>
    /// <param name="Name">The name of the shipping method. This value will be 
    /// displayed on the Google Checkout order review page.</param>
    /// <param name="DefaultCost">The default cost associated with the shipping 
    /// method. This value is the amount that Gogle Checkout will charge for 
    /// shipping if the merchant calculation callback request fails.</param>
    public void AddMerchantCalculatedShippingMethod(string Name,
      decimal DefaultCost) {
      AddMerchantCalculatedShippingMethod(Name, DefaultCost, null);
    }

    /// <summary>
    /// This method adds a merchant-calculated shipping method to an order. 
    /// This method handles merchant-calculated shipping methods that have 
    /// shipping restrictions.
    /// </summary>
    /// <param name="Name">The name of the shipping method. This value will be 
    /// displayed on the Google Checkout order review page.</param>
    /// <param name="DefaultCost">The default cost associated with the shipping 
    /// method. This value is the amount that Gogle Checkout will charge for 
    /// shipping if the merchant calculation callback request fails.</param>
    /// <param name="Restrictions">A list of country, state or zip code areas 
    /// where the shipping method is either available or unavailable.</param>
    public void AddMerchantCalculatedShippingMethod(string Name,
      decimal DefaultCost, ShippingRestrictions Restrictions) {
      AutoGen.MerchantCalculatedShipping Method =
        new AutoGen.MerchantCalculatedShipping();
      Method.name = Name;
      Method.price = new AutoGen.MerchantCalculatedShippingPrice();
      Method.price.currency = _Currency;
      Method.price.Value = DefaultCost;
      if (Restrictions != null) {
        Method.shippingrestrictions = Restrictions.XmlRestrictions;
      }
      AddNewShippingMethod(Method);
    }

    /// <summary>
    /// This method adds an instore-pickup shipping option to an order.
    /// </summary>
    /// <param name="Name">The name of the shipping method. This value will be 
    /// displayed on the Google Checkout order review page.</param>
    /// <param name="Cost">The cost associated with the shipping method.</param>
    public void AddPickupShippingMethod(string Name, decimal Cost) {
      AutoGen.Pickup Method = new AutoGen.Pickup();
      Method.name = Name;
      Method.price = new AutoGen.PickupPrice();
      Method.price.currency = _Currency;
      Method.price.Value = Cost;
      AddNewShippingMethod(Method);
    }

    /// <summary>
    /// Adds the new shipping method.
    /// </summary>
    /// <param name="NewShippingMethod">The new shipping method.</param>
    private void AddNewShippingMethod(Object NewShippingMethod) {
      Object[] NewMethods = new Object[_ShippingMethods.Items.Length + 1];
      for (int i = 0; i < _ShippingMethods.Items.Length; i++) {
        NewMethods[i] = _ShippingMethods.Items[i];
      }
      NewMethods[NewMethods.Length - 1] = NewShippingMethod;
      _ShippingMethods.Items = NewMethods;
    }

    /// <summary>
    /// This method adds a tax rule associated with a zip code pattern.
    /// </summary>
    /// <param name="ZipPattern">The zip pattern.</param>
    /// <param name="TaxRate">The tax rate associated with a tax rule. Tax rates 
    /// are expressed as decimal values. For example, a value of 0.0825 
    /// specifies a tax rate of 8.25%.</param>
    /// <param name="ShippingTaxed">
    /// If this parameter has a value of <b>true</b>, then shipping costs will
    /// be taxed for items that use the associated tax rule.
    /// </param>
    public void AddZipTaxRule(string ZipPattern, double TaxRate,
      bool ShippingTaxed) {
      if (!IsValidZipPattern(ZipPattern)) {
        throw new ApplicationException(ZIP_CODE_PATTERN_EXCEPTION);
      }
      AutoGen.DefaultTaxRule Rule = new AutoGen.DefaultTaxRule();
      Rule.rate = TaxRate;
      Rule.shippingtaxedSpecified = true;
      Rule.shippingtaxed = ShippingTaxed;
      Rule.taxarea = new AutoGen.DefaultTaxRuleTaxarea();
      AutoGen.USZipArea Area = new AutoGen.USZipArea();
      Rule.taxarea.Item = Area;
      Area.zippattern = ZipPattern;
      AddNewTaxRule(Rule);
    }

    /// <summary>
    /// Add an already <see cref="System.Web.HttpUtility.UrlEncode(string)"/> Url
    /// </summary>
    /// <param name="url">The UrlEncoded &lt;parameterized-url&gt; to add to the collection</param>
    public ParameterizedUrl AddParameterizedUrl(string url) {
      return AddParameterizedUrl(url, false);
    }

    /// <summary>
    /// Add a Third Party Tracking URL.
    /// </summary>
    /// <param name="url">The &lt;parameterized-url&gt; to add to the collection</param>
    /// <param name="urlEncode">true if you need the url to be <see cref="System.Web.HttpUtility.UrlEncode(string)"/></param>
    /// <returns>A new <see cref="ParameterizedUrl" /></returns>
    public ParameterizedUrl AddParameterizedUrl(string url, bool urlEncode) {
      ParameterizedUrl retVal = ParameterizedUrls.AddUrl(url, urlEncode);
      return retVal;
    }

    /// <summary>
    /// Add a Third Party Tracking URL.
    /// </summary>
    /// <param name="url">The &lt;parameterized-url&gt; object to add to the collection</param>
    /// <returns>A new <see cref="ParameterizedUrl" /></returns>
    public void AddParameterizedUrl(ParameterizedUrl url) {
      _ParameterizedUrls.AddUrl(url);
    }

    /// <summary>
    /// This method verifies that a given zip code pattern is valid. Zip code 
    /// patterns may be five-digit numbers or they may be one- to four-digit 
    /// numbers followed by an asterisk.
    /// </summary>
    /// <param name="ZipPattern">This parameter contains the zip code pattern 
    /// that is being evaluated.</param>
    /// <returns>
    ///   This method returns <b>true</b> if the provided zip code pattern
    ///   is valid, meaning it is either a series of five digits or it is 
    ///   a series of zero to four digits followed by an asterisk. If the 
    ///   zip code pattern is not valid, this method returns <b>false</b>.
    /// </returns>
    public static bool IsValidZipPattern(string ZipPattern) {
      Regex r = new Regex("^((\\d{0,4}\\*)|(\\d{5}))$");
      Match m = r.Match(ZipPattern);
      return m.Success;
    }

    /// <summary>
    /// This method adds a tax rule associated with a particular state.
    /// </summary>
    /// <param name="StateCode">This parameter contains a two-letter U.S. state 
    /// code associated with a tax rule.</param>
    /// <param name="TaxRate">The tax rate associated with a tax rule. Tax 
    /// rates are expressed as decimal values. For example, a value of 0.0825 
    /// specifies a tax rate of 8.25%.</param>
    /// <param name="ShippingTaxed">
    /// If this parameter has a value of <b>true</b>, then shipping costs will
    /// be taxed for items that use the associated tax rule.
    /// </param>
    public void AddStateTaxRule(string StateCode, double TaxRate,
      bool ShippingTaxed) {
      AutoGen.DefaultTaxRule Rule = new AutoGen.DefaultTaxRule();
      Rule.rate = TaxRate;
      Rule.shippingtaxedSpecified = true;
      Rule.shippingtaxed = ShippingTaxed;
      Rule.taxarea = new AutoGen.DefaultTaxRuleTaxarea();
      AutoGen.USStateArea Area = new AutoGen.USStateArea();
      Rule.taxarea.Item = Area;
      Area.state = StateCode;
      AddNewTaxRule(Rule);
    }

    /// <summary>
    /// Adds the country tax rule.
    /// This method adds a tax rule associated with a particular state.
    /// </summary>
    /// <param name="Area">The area.</param>
    /// <param name="TaxRate">The tax rate associated with a tax rule. Tax 
    /// rates are expressed as decimal values. For example, a value of 0.0825 
    /// specifies a tax rate of 8.25%.</param>
    /// <param name="ShippingTaxed">
    /// If this parameter has a value of <b>true</b>, then shipping costs will
    /// be taxed for items that use the associated tax rule.
    /// </param>
    /// <example>
    /// <code>
    ///   // We assume Req is a CheckoutShoppingCartRequest object.
    ///   // Charge the 50 states 8% tax and do not tax shipping.
    ///   Req.AddCountryTaxRule(AutoGen.USAreas.FULL_50_STATES, 0.08, false);
    ///   // Charge the 48 continental states 5% tax and do tax shipping.
    ///   Req.AddCountryTaxRule(AutoGen.USAreas.CONTINENTAL_48, 0.05, true);
    ///   // Charge all states (incl territories) 9% tax, don't tax shipping.
    ///   Req.AddCountryTaxRule(AutoGen.USAreas.ALL, 0.09, false);
    /// </code>
    /// </example>
    public void AddCountryTaxRule(AutoGen.USAreas Area, double TaxRate,
      bool ShippingTaxed) {
      AutoGen.DefaultTaxRule Rule = new AutoGen.DefaultTaxRule();
      Rule.rate = TaxRate;
      Rule.shippingtaxedSpecified = true;
      Rule.shippingtaxed = ShippingTaxed;
      Rule.taxarea = new AutoGen.DefaultTaxRuleTaxarea();
      AutoGen.USCountryArea ThisArea = new AutoGen.USCountryArea();
      Rule.taxarea.Item = ThisArea;
      ThisArea.countryarea = Area;
      AddNewTaxRule(Rule);
    }

    /// <summary>
    /// This method adds a new tax rule to the &lt;default-tax-table&gt;.
    /// This method is called by the methods that create the XML blocks
    /// for flat-rate shipping, merchant-calculated shipping and instore-pickup
    /// shipping methods.
    /// </summary>
    /// <param name="NewRule">This parameter contains an object representing
    /// a default tax rule.</param>
    private void AddNewTaxRule(AutoGen.DefaultTaxRule NewRule) {
      AutoGen.DefaultTaxTable NewTable = new AutoGen.DefaultTaxTable();
      NewTable.taxrules =
        new AutoGen.DefaultTaxRule
        [_TaxTables.defaulttaxtable.taxrules.Length + 1];
      for (int i = 0; i < NewTable.taxrules.Length - 1; i++) {
        NewTable.taxrules[i] = _TaxTables.defaulttaxtable.taxrules[i];
      }
      NewTable.taxrules[NewTable.taxrules.Length - 1] = NewRule;
      _TaxTables.defaulttaxtable = NewTable;
    }

    /// <summary>
    /// This property adds a XmlNode to the 
    /// &lt;merchant-private-data&gt; element.
    /// </summary>
    /// <value>The &lt;merchant-private-data&gt; element.</value>
    public void AddMerchantPrivateDataNode(XmlNode node) {
      if (node == null)
        throw new ArgumentNullException("node");

      _MerchantPrivateDataNodes.Add(node);
    }

    /// <summary>
    /// This method generates the XML for a Checkout API request. This method
    /// also calls the <b>CheckPreConditions</b> method, which verifies that
    /// if the API request indicates that the merchant will calculate tax and
    /// shipping costs, that the input data for those calculations is included
    /// in the request.
    /// </summary>
    /// <returns>This method returns the XML for a Checkout API request.
    /// </returns>
    public override byte[] GetXml() {

      // Verify that if the API request calls for merchant calculations, the 
      // input data for those calculations is included in the request.
      //
      CheckPreConditions();

      AutoGen.CheckoutShoppingCart MyCart = new AutoGen.CheckoutShoppingCart();
      MyCart.shoppingcart = new AutoGen.ShoppingCart();

      // Add the Shopping cart expiration element.
      if (_CartExpiration != DateTime.MinValue) {
        MyCart.shoppingcart.cartexpiration = new AutoGen.CartExpiration();
        MyCart.shoppingcart.cartexpiration.gooduntildate = _CartExpiration;
      }

      // Add the items in the shopping cart to the API request.
      MyCart.shoppingcart.items = new AutoGen.Item[_Items.Count];
      for (int i = 0; i < _Items.Count; i++) {
        ShoppingCartItem MyItem = (ShoppingCartItem)_Items[i];
        MyCart.shoppingcart.items[i] = new AutoGen.Item();
        MyCart.shoppingcart.items[i].itemname =
          EncodeHelper.EscapeXmlChars(MyItem.Name);
        MyCart.shoppingcart.items[i].itemdescription =
          EncodeHelper.EscapeXmlChars(MyItem.Description);
        MyCart.shoppingcart.items[i].quantity = MyItem.Quantity;
        MyCart.shoppingcart.items[i].unitprice = new AutoGen.Money();
        MyCart.shoppingcart.items[i].unitprice.currency = _Currency;
        MyCart.shoppingcart.items[i].unitprice.Value = MyItem.Price;
        
        if (MyItem.MerchantItemID != null && MyItem.MerchantItemID.Length > 0) {
         MyCart.shoppingcart.items[i].merchantitemid = MyItem.MerchantItemID;  
        }

        if (MyItem.MerchantPrivateItemDataNodes != null
          && MyItem.MerchantPrivateItemDataNodes.Length > 0) {
          AutoGen.anyMultiple any = new AutoGen.anyMultiple();

          any.Any = MyItem.MerchantPrivateItemDataNodes;
          MyCart.shoppingcart.items[i].merchantprivateitemdata = any;
        }

        if (MyItem.TaxTable != null && 
          MyItem.TaxTable != AlternateTaxTable.Empty) {
          MyCart.shoppingcart.items[i].taxtableselector = MyItem.TaxTable.Name; 
        }
      }

      //because we are merging the nodes, create a new ArrayList
      ArrayList copiedMerchantPrivateData = new ArrayList();

      // Add the &lt;merchant-private-data&gt; element to the API request.
      if (_MerchantPrivateData != null) {
        copiedMerchantPrivateData.Add(MakeXmlElement(_MerchantPrivateData));
      }

      if (_MerchantPrivateDataNodes != null && _MerchantPrivateDataNodes.Count > 0) {
        for (int i = 0; i < _MerchantPrivateDataNodes.Count; i++)
          copiedMerchantPrivateData.Add(_MerchantPrivateDataNodes[i]);
      }

      if (copiedMerchantPrivateData.Count > 0) {
        AutoGen.anyMultiple any = new AutoGen.anyMultiple();

        System.Xml.XmlNode[] nodes =
          new System.Xml.XmlNode[copiedMerchantPrivateData.Count];
        copiedMerchantPrivateData.CopyTo(nodes);
        any.Any = nodes;

        MyCart.shoppingcart.merchantprivatedata = any;
      }

      // Add the &lt;continue-shopping-url&gt; element to the API request.
      MyCart.checkoutflowsupport =
        new AutoGen.CheckoutShoppingCartCheckoutflowsupport();
      MyCart.checkoutflowsupport.Item =
        new AutoGen.MerchantCheckoutFlowSupport();
      if (_ContinueShoppingUrl != null) {
        MyCart.checkoutflowsupport.Item.continueshoppingurl =
          _ContinueShoppingUrl;
      }

      if (_AnalyticsData != null) {
        MyCart.checkoutflowsupport.Item.analyticsdata = _AnalyticsData;
      }

      if (_PlatformID != 0) {
        MyCart.checkoutflowsupport.Item.platformid = _PlatformID;
      }

      // Add the &lt;edit-cart-url&gt; element to the API request.
      if (_EditCartUrl != null) {
        MyCart.checkoutflowsupport.Item.editcarturl = _EditCartUrl;
      }

      // Add the &lt;request-buyer-phone-number&gt; element to the API request.
      if (_RequestBuyerPhoneNumber) {
        MyCart.checkoutflowsupport.Item.requestbuyerphonenumber = true;
        MyCart.checkoutflowsupport.Item.requestbuyerphonenumberSpecified =
          true;
      }

      // Add the shipping methods to the API request.
      MyCart.checkoutflowsupport.Item.shippingmethods = _ShippingMethods;

      //jf Tax Tables added 3/1/2007
      if (_alternateTaxTables != null) {
        _alternateTaxTables.AppendToRequest(_TaxTables);   
      }

      // Add the tax tables to the API request.
      if (_TaxTables != null) {
        MyCart.checkoutflowsupport.Item.taxtables = _TaxTables;
      }

      // Add the merchant calculations URL to the API request.
      if (MerchantCalculationsUrl != null) {
        MyCart.checkoutflowsupport.Item.merchantcalculations =
          new AutoGen.MerchantCalculations();
        MyCart.checkoutflowsupport.Item.merchantcalculations.
          merchantcalculationsurl = MerchantCalculationsUrl;
      }

      // Indicate whether the merchant accepts coupons and gift certificates.
      if (_AcceptMerchantCoupons) {
        MyCart.checkoutflowsupport.Item.merchantcalculations.
          acceptmerchantcoupons = true;
        MyCart.checkoutflowsupport.Item.merchantcalculations.
          acceptmerchantcouponsSpecified = true;
      }
      if (_AcceptMerchantGiftCertificates) {
        MyCart.checkoutflowsupport.Item.merchantcalculations.
          acceptgiftcertificates = true;
        MyCart.checkoutflowsupport.Item.merchantcalculations.
          acceptgiftcertificatesSpecified = true;
      }

      //See if we have any ParameterizedUrl that need to be added to the message.
      if (_ParameterizedUrls.Urls.Length > 0) {
        GCheckout.AutoGen.ParameterizedUrl[] destUrls
          = new GCheckout.AutoGen.ParameterizedUrl[_ParameterizedUrls.Urls.Length];
        Debug.WriteLine("<ParameterizedUrls>");
        for (int i = 0; i < _ParameterizedUrls.Urls.Length; i++) {
          Debug.WriteLine("<ParameterizedUrl>");
          ParameterizedUrl fromUrl = _ParameterizedUrls.Urls[i];
          GCheckout.AutoGen.ParameterizedUrl toUrl
            = new GCheckout.AutoGen.ParameterizedUrl();
          toUrl.url = fromUrl.URL;

          //ok now we have to see if params exist, and if they do we need to look that array and build it out.
          if (fromUrl.Params.Length > 0) {
            GCheckout.AutoGen.UrlParameter[] destparams
              = new GCheckout.AutoGen.UrlParameter[fromUrl.Params.Length];
            for (int j = 0; j < fromUrl.Params.Length; j++) {
              UrlParamter fromParam = fromUrl.Params[j];
              GCheckout.AutoGen.UrlParameter toParam
              = new GCheckout.AutoGen.UrlParameter();
              toParam.name = fromParam.Name;
              //Use the reflection code to get the string value of the enum.
              toParam.type = fromParam.SerializedValue;
              Debug.WriteLine("<parameterized-url url=\"" + toUrl.url + "\" />");
              destparams[j] = toParam;
            }
            toUrl.parameters = destparams;
          }

          Debug.WriteLine("<parameterized-url url=\"" + toUrl.url + "\">");
          destUrls[i] = toUrl; //add the url to the array.

          MyCart.checkoutflowsupport.Item.parameterizedurls = destUrls;
          Debug.WriteLine("</ParameterizedUrl>");
        }
        Debug.WriteLine("</ParameterizedUrls>");
      }

      // Call the EncodeHelper.Serialize method to generate the XML for
      // the Checkout API request.
      return EncodeHelper.Serialize(MyCart);
    }

    /// <summary>
    /// This method is used to perform several checks that verify the integrity
    /// of the information in a Checkout API XML request. This method will 
    /// throw an exception if the request does not pass any of these tests.
    /// </summary>
    private void CheckPreConditions() {
      // 1. If the request indicates that tax will be calculated by the 
      // merchant, the request must contain at least one default tax rule.
      if (_TaxTables.merchantcalculated &&
        _TaxTables.defaulttaxtable.taxrules.Length == 0) {
        throw new ApplicationException("If you set " +
          "MerchantCalculatedTax=true, you must add at least one tax rule.");
      }
      // 2. If the request indicates that tax will be calculated by the
      // merchant, the request must specify a merchant-calculations-url.
      if (_TaxTables.merchantcalculated && _MerchantCalculationsUrl == null) {
        throw new ApplicationException("If you set " +
          "MerchantCalculatedTax=true, you must set MerchantCalculationsUrl.");
      }
      // 3. If the request indicates that the merchant accepts coupons, the
      // request must also specify a merchant-calculations-url.
      if (_AcceptMerchantCoupons && _MerchantCalculationsUrl == null) {
        throw new ApplicationException("If you set " +
          "AcceptMerchantCoupons=true, you must set MerchantCalculationsUrl.");
      }
      // 4. If the request indicates that the merchant accepts gift 
      // certificates, the request must also specify a 
      // merchant-calculations-url.
      if (_AcceptMerchantGiftCertificates && _MerchantCalculationsUrl == null) {
        throw new ApplicationException("If you set " +
          "AcceptMerchantGiftCertificates=true, you must set " +
          "MerchantCalculationsUrl.");
      }
    }

    /// <summary>
    /// This method is used to construct the &lt;merchant-private-data&gt; and 
    /// &lt;merchant-private-item-data&gt; XML elements. Both of these elements
    /// contain freeform XML blocks that are specified by the merchant.
    /// </summary>
    /// <param name="Xml">The XML.</param>
    /// <returns>This method returns the element value for either the 
    /// &lt;merchant-private-data&gt; or the &lt;merchant-private-item-data&gt; 
    /// XML element.</returns>
    internal static XmlElement MakeXmlElement(string Xml) {
      XmlDocument Doc = new XmlDocument();
      XmlElement El = Doc.CreateElement("q");
      El.InnerXml = Xml;
      return (XmlElement)El.FirstChild;
    }

    /// <summary>
    /// This method sets the value of the &lt;good-until-date&gt; using the
    /// value of the <b>CartExpirationMinutes</b> parameter. This method 
    /// converts that value into Coordinated Universal Time (UTC).
    /// </summary>
    /// <param name="ExpirationMinutesFromNow">
    /// The length of time, in minutes, after which the shopping cart should
    /// expire. This property contains the value of the 
    /// <b>CartExpirationMinutes</b> property.
    /// </param>
    public void SetExpirationMinutesFromNow(int ExpirationMinutesFromNow) {
      CartExpiration = DateTime.UtcNow.AddMinutes(ExpirationMinutesFromNow);
    }

    /// <summary>
    /// This property sets or retrieves a value that indicates whether the 
    /// merchant is responsible for calculating taxes for the default
    /// tax table.
    /// </summary>
    /// <value>
    ///   The value of this property should be <b>true</b> if the merchant
    ///   will calculate taxes for the order. Otherwise, this value should be
    ///   <b>false</b>. The value should only be <b>true</b> if the merchant
    ///   has implemented the Merchant Calculations API.
    /// </value>
    public bool MerchantCalculatedTax {
      get {
        return _TaxTables.merchantcalculated;
      }
      set {
        _TaxTables.merchantcalculated = value;
        _TaxTables.merchantcalculatedSpecified = true;
      }
    }

    /// <summary>
    /// This property sets or retrieves a value that indicates whether the 
    /// merchant accepts coupons. If this value is set to <b>true</b>, the
    /// Google Checkout order confirmation page will display a text field 
    /// where the customer can enter a coupon code.
    /// </summary>
    /// <value>
    ///   This value of this property is a Boolean value that indicates
    ///   whether the merchant accepts coupons. This value should only be 
    ///   set to <b>true</b> if the merchant has implemented the Merchant 
    ///   Calculations API.
    /// </value>
    public bool AcceptMerchantCoupons {
      get {
        return _AcceptMerchantCoupons;
      }
      set {
        _AcceptMerchantCoupons = value;
      }
    }

    /// <summary>
    /// This property sets or retrieves a value that indicates whether the 
    /// merchant accepts gift certificates. If this value is set to 
    /// <b>true</b>, the Google Checkout order confirmation page will 
    /// display a text field where the customer can enter a gift certificate 
    /// code.
    /// </summary>
    /// <value>
    ///   This value of this property is a Boolean value that indicates
    ///   whether the merchant accepts gift certificates. This value should 
    ///   only be set to <b>true</b> if the merchant has implemented the 
    ///   Merchant Calculations API.
    /// </value>
    public bool AcceptMerchantGiftCertificates {
      get {
        return _AcceptMerchantGiftCertificates;
      }
      set {
        _AcceptMerchantGiftCertificates = value;
      }
    }

    /// <summary>
    /// This property sets or retrieves the value of the 
    /// &lt;merchant-calculations-url&gt; element. This value is the URL to 
    /// which Google Checkout will send &lt;merchant-calculation-callback&gt;
    /// requests. This property is only relevant for merchants who are
    /// implementing the Merchant Calculations API.
    /// </summary>
    /// <value>The &lt;merchant-calculations-url&gt; element value.</value>
    public string MerchantCalculationsUrl {
      get {
        return _MerchantCalculationsUrl;
      }
      set {
        _MerchantCalculationsUrl = value;
      }
    }

    /// <summary>
    /// This property sets or retrieves the value of the 
    /// &lt;merchant-private-data&gt; element.
    /// </summary>
    /// <value>The &lt;merchant-private-data&gt; element value.</value>
    [Obsolete("merchant-private-data is now a XmlNode array. please use the AddMerchantPrivateDataNode method.")]
    public string MerchantPrivateData {
      get {
        return _MerchantPrivateData;
      }
      set {
        _MerchantPrivateData = value;
      }
    }

    /// <summary>
    /// This property retrieves the value of the 
    /// &lt;merchant-private-data&gt; element.
    /// </summary>
    /// <value>The &lt;merchant-private-data&gt; element value.</value>
    public System.Xml.XmlNode[] MerchantPrivateDataNodes {
      get {
        System.Xml.XmlNode[] nodes =
          new System.Xml.XmlNode[_MerchantPrivateDataNodes.Count];
        _MerchantPrivateDataNodes.CopyTo(nodes);
        return nodes;
      }
    }

    /// <summary>
    /// This property sets or retrieves the value of the 
    /// &lt;continue-shopping-url&gt; element. Google Checkout will display 
    /// a link to this URL on the page that the customer sees after completing 
    /// her purchase.
    /// </summary>
    /// <value>The &lt;continue-shopping-url&gt; element value.</value>
    public string ContinueShoppingUrl {
      get {
        return _ContinueShoppingUrl;
      }
      set {
        _ContinueShoppingUrl = value;
      }
    }

    /// <summary>
    /// This property sets or retrieves the value of the 
    /// &lt;edit-cart-url&gt; element. Google Checkout will display 
    /// a link to this URL on the Google Checkout order confirmation page.
    /// The customer can click this link to edit the shopping cart contents
    /// before completing a purchase.
    /// </summary>
    /// <value>The &lt;edit-cart-url&gt; element value.</value>
    public string EditCartUrl {
      get {
        return _EditCartUrl;
      }
      set {
        _EditCartUrl = value;
      }
    }

    /// <summary>
    /// This property sets or retrieves the value of the 
    /// &lt;request-buyer-phone-number&gt; element. If this value is true,
    /// the buyer must enter a phone number to complete a purchase.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the Google should send the buyer's phone number
    ///   to the merchant, otherwise <c>false</c>.
    /// </value>
    public bool RequestBuyerPhoneNumber {
      get {
        return _RequestBuyerPhoneNumber;
      }
      set {
        _RequestBuyerPhoneNumber = value;
      }
    }

    /// <summary>
    /// This property sets or retrieves the value of the 
    /// &lt;good-until-date&gt; element.
    /// </summary>
    /// <value>The cart expiration.</value>
    public DateTime CartExpiration {
      get {
        return _CartExpiration;
      }
      set {
        _CartExpiration = value;
      }
    }

    /// <summary>
    /// This property sets or retrieves the value of the 
    /// &lt;analytics-data&gt; element. Google Analytics uses this
    /// to Track Google Checkout Orders.
    /// </summary>
    /// <remarks>
    /// Please read http://code.google.com/apis/checkout/developer/checkout_analytics_integration.html"
    /// for more information.
    /// </remarks>
    /// <value>The &lt;analytics-data&gt; element value.</value>
    public string AnalyticsData {
      get {
        return _AnalyticsData;
      }
      set {
        _AnalyticsData = value;
      }
    }

    /// <summary>
    /// This property sets or retrieves the value of the 
    /// &lt;platform-id&gt; element.
    /// </summary>
    /// <remarks>
    /// The &lt;platform-id&gt; tag should only be used by eCommerce providers
    /// who make API requests on behalf of a merchant. The tag's value contains
    /// a Google Checkout merchant ID that identifies the eCommerce provider.
    /// </remarks>
    /// <value>The &lt;analytics-data&gt; element value.</value>
    public long PlatformID {
      get {
        return _PlatformID;
      }
      set {
        _PlatformID = value;
      }
    }

    /// <summary>
    /// The &lt;parameterized-urls&gt; tag
    /// </summary>
    /// <remarks>
    /// It contains information about all of the
    /// web beacons that the merchant wants to add to the Google Checkout order 
    /// confirmation page. This tag encapsulates a list of one or more
    /// &lt;parameterized-url&gt; (<see cref="ParameterizedUrl"/>) tags.
    /// See
    /// http://code.google.com/apis/checkout/developer/checkout_pixel_tracking.html
    /// For additional information on Third-Party Conversion Tracking
    /// </remarks>
    public ParameterizedUrls ParameterizedUrls {
      get {
        return _ParameterizedUrls;
      }
    }

    /// <summary>
    /// The Alternate Tax Tables container
    /// </summary>
    /// <remarks>
    /// All of the Alternate Tax tables are added to this Container.
    /// A Factory Method is also provided to create a Tax table and return it.
    /// </remarks>
    public AlternateTaxTableCollection AlternateTaxTables {
      get {
        return _alternateTaxTables; 
      }
    }

    /// <summary></summary>
    public override string GetPostUrl() 
    {
      if (_Environment == EnvironmentType.Sandbox) 
      {
        return string.Format(
          "https://sandbox.google.com/checkout/cws/v2/Merchant/{0}/merchantCheckout",
          _MerchantID);
      }
      else 
      {
        return string.Format(
          "https://checkout.google.com/cws/v2/Merchant/{0}/merchantCheckout",
          _MerchantID);
      }
    }

    private class ShoppingCartItem 
    {
      public string Name;
      public string Description;
      public decimal Price = 0.0m;
      public int Quantity = 0;
      public string MerchantItemID;
      public XmlNode[] MerchantPrivateItemDataNodes;
      public AlternateTaxTable TaxTable = null;

      /// <summary>
      /// This method initializes a new instance of the 
      /// <see cref="ShoppingCartItem"/> class, which creates an object
      /// corresponding to an individual item in an order. This method 
      /// is used for items that do not have an associated
      /// &lt;merchant-private-item-data&gt; XML block.
      /// </summary>
      /// <param name="InName">The name of the item.</param>
      /// <param name="InDescription">A description of the item.</param>
      /// <param name="InMerchantItemID">The Merchant Item Id that uniquely 
      /// identifies the product in your system. (optional)</param>
      /// <param name="InPrice">The price of the item, per item.</param>
      /// <param name="InQuantity">The number of the item that is 
      /// included in the order.</param>
      public ShoppingCartItem(string InName, string InDescription,
        string InMerchantItemID, decimal InPrice, int InQuantity)
        : this(InName, InDescription, InMerchantItemID, InPrice,
        InQuantity, AlternateTaxTable.Empty, new XmlNode[] {}) {
      }

      /// <summary>
      /// This method initializes a new instance of the 
      /// <see cref="ShoppingCartItem"/> class, which creates an object
      /// corresponding to an individual item in an order. This method 
      /// is used for items that do have an associated
      /// &lt;merchant-private-item-data&gt; XML block.
      /// </summary>
      /// <param name="InName">The name of the item.</param>
      /// <param name="InDescription">A description of the item.</param>
      /// <param name="InMerchantItemID">The Merchant Item Id that uniquely 
      /// identifies the product in your system. (optional)</param>
      /// <param name="InPrice">The price of the item, per item.</param>
      /// <param name="InQuantity">The number of the item that is 
      /// included in the order.</param>
      /// <param name="InMerchantPrivateItemData">The merchant private
      /// item data associated with the item.</param>
      /// <param name="taxTable">The <see cref="AlternateTaxTable"/> 
      /// To assign to the item </param>
      public ShoppingCartItem(string InName, string InDescription,
        string InMerchantItemID, decimal InPrice, int InQuantity,
        AlternateTaxTable taxTable,
        params XmlNode[] InMerchantPrivateItemData) {
        Name = InName;
        Description = InDescription;
        if (MerchantItemID == string.Empty)
          MerchantItemID = null;
        MerchantItemID = InMerchantItemID;
        Price = InPrice;
        Quantity = InQuantity;
        MerchantPrivateItemDataNodes = InMerchantPrivateItemData;        
        TaxTable = taxTable;
      }
    }
  }
}
