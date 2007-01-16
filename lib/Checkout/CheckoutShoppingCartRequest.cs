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
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;
using GCheckout.Util;

namespace GCheckout.Checkout {
  public class CheckoutShoppingCartRequest : GCheckoutRequest {
    private ArrayList _Items;
    private AutoGen.TaxTables _TaxTables;
    private AutoGen.MerchantCheckoutFlowSupportShippingmethods
      _ShippingMethods;
    private string _MerchantPrivateData = null;
    private bool _AcceptMerchantCoupons = false;
    private bool _AcceptMerchantGiftCertificates = false;
    private string _MerchantCalculationsUrl = null;
    private string _ContinueShoppingUrl = null;
    private string _EditCartUrl = null;
    private bool _RequestBuyerPhoneNumber = false;
    private DateTime _CartExpiration = DateTime.MinValue;
    private string _Currency = null;

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
      _Items.Add(new ShoppingCartItem(Name, Description, Price, Quantity));
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
    public void AddItem(string Name, string Description, decimal Price,
      int Quantity, string MerchantPrivateItemData) {
      _Items.Add(new ShoppingCartItem(Name, Description, Price, Quantity,
        MerchantPrivateItemData));
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
      Method.price = new AutoGen.Money();
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
      Method.price = new AutoGen.Money();
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
      Method.price = new AutoGen.Money();
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
        throw new ApplicationException("Zip code patterns must be five " +
          "numeric characters, or zero to 4 numeric characters followed by " +
          "a single asterisk as a wildcard character.");
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
        if (MyItem.MerchantPrivateItemData != null) {
          MyCart.shoppingcart.items[i].merchantprivateitemdata =
            MakeXmlElement(MyItem.MerchantPrivateItemData);
        }
      }

      // Add the &lt;merchant-private-data&gt; element to the API request.
      if (_MerchantPrivateData != null) {
        MyCart.shoppingcart.merchantprivatedata =
          MakeXmlElement(_MerchantPrivateData);
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
    private static XmlElement MakeXmlElement(string Xml) {
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
    public string MerchantPrivateData {
      get {
        return _MerchantPrivateData;
      }
      set {
        _MerchantPrivateData = value;
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

    private class ShoppingCartItem {
      public string Name = "";
      public string Description = "";
      public decimal Price = 0.0m;
      public int Quantity = 0;
      public string MerchantPrivateItemData = null;

      /// <summary>
      /// This method initializes a new instance of the 
      /// <see cref="ShoppingCartItem"/> class, which creates an object
      /// corresponding to an individual item in an order. This method 
      /// is used for items that do not have an associated
      /// &lt;merchant-private-item-data&gt; XML block.
      /// </summary>
      /// <param name="InName">The name of the item.</param>
      /// <param name="InDescription">A description of the item.</param>
      /// <param name="InPrice">The price of the item, per item.</param>
      /// <param name="InQuantity">The number of the item that is included in the order.</param>
      public ShoppingCartItem(string InName, string InDescription,
        decimal InPrice, int InQuantity)
        : this(InName, InDescription, InPrice, InQuantity, null) {
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
      /// <param name="InPrice">The price of the item, per item.</param>
      /// <param name="InQuantity">The number of the item that is included in the order.</param>
      /// <param name="InMerchantPrivateItemData">The merchant private
      /// item data associated with the item.</param>
      public ShoppingCartItem(string InName, string InDescription,
        decimal InPrice, int InQuantity, string InMerchantPrivateItemData) {
        Name = InName;
        Description = InDescription;
        Price = InPrice;
        Quantity = InQuantity;
        MerchantPrivateItemData = InMerchantPrivateItemData;
      }
    }
  }
}
