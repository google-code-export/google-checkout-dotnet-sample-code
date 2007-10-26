/*************************************************
 * Copyright (C) 2007 Google Inc.
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
using System.Collections;
using GCheckout.Util;

namespace GCheckout.OrderProcessing
{
	/// <summary>
  /// the &lt;ship-items&gt; command lets you specify shipping information for
  /// one or more items in an order. When you send a &lt;ship-items&gt;
  /// request, the shipping information in that request will be appended to 
  /// the existing shipping records for the specified order.
	/// </summary>
	/// <remarks>
	/// A Box can only contain one Tracking number.
	/// Many Items can fit in a box and one item may
	/// by placed in multiple boxes.
	/// Because of this, There will be two different ways to code this message.
	/// The First way is to Add a Box to the shipment and then add items to it.
	/// The Second way is to just add items to the shipment passing in the
	/// tracking information.
	/// </remarks>
	public class ShipItemsRequest : GCheckoutRequest
	{
    private string _googleOrderNumber = null;
    private bool _sendEmail = false;
    private bool _sendEmailSpecified = false;
    //The list of boxes for the order.
    private Hashtable _boxes = new Hashtable();
    internal Hashtable _items = new Hashtable();

    /// <summary>
    /// The Google Order Number
    /// </summary>
    public string GoogleOrderNumber {
      get {
        return _googleOrderNumber;
      }
    }

    /// <summary>
    /// The &lt;send-email&gt; tag indicates whether Google Checkout 
    /// should email the buyer 
    /// </summary>
    public bool SendEmail {
      get {
        return _sendEmail;
      }
      set {
        _sendEmail = value;
        _sendEmailSpecified = true;
      }
    }

    /// <summary>
    /// The Array of Boxes that contain products.
    /// </summary>
    private ShipItemBox[] Boxes {
      get {
        ShipItemBox[] retVal = new ShipItemBox[_boxes.Count];
        _boxes.CopyTo(retVal, 0);
        return retVal;
      }
    }

    /// <summary>
    /// Return the List of
    /// <see cref="GCheckout.AutoGen.ItemShippingInformation"/>[]
    /// </summary>
    public AutoGen.ItemShippingInformation[] ItemShippingInfo {
      get {
        AutoGen.ItemShippingInformation[] retVal 
          = new AutoGen.ItemShippingInformation[_items.Count];
        _items.Values.CopyTo(retVal, 0);
        return retVal;
      }
    }

    /// <summary>
    /// Create a new &lt;ship-items&gt; API request message using the
    /// Configuration Settings
    /// </summary>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    public ShipItemsRequest(string GoogleOrderNumber) {
      _MerchantID = GCheckoutConfigurationHelper.MerchantID.ToString();
      _MerchantKey = GCheckoutConfigurationHelper.MerchantKey;
      _Environment = GCheckoutConfigurationHelper.Environment;
      _googleOrderNumber = GoogleOrderNumber;
    }
	
    /// <summary>
    /// Create a new &lt;ship-items&gt; API request message
    /// </summary>
    /// <param name="MerchantID">Google Checkout Merchant ID</param>
    /// <param name="MerchantKey">Google Checkout Merchant Key</param>
    /// <param name="Env">A String representation of 
    /// <see cref="EnvironmentType"/></param>
    /// <param name="GoogleOrderNumber">The Google Order Number</param>
    public ShipItemsRequest(string MerchantID, string MerchantKey, 
      string Env, string GoogleOrderNumber) {
      _MerchantID = MerchantID;
      _MerchantKey = MerchantKey;
      _Environment = StringToEnvironment(Env);
      _googleOrderNumber = GoogleOrderNumber;
    }

    /// <summary>
    /// Add a new box to the order that items will be placed in.
    /// </summary>
    /// <param name="carrier">
    /// The &lt;carrier&gt; tag contains the name of the company 
    /// responsible for shipping the item. Valid values for this 
    /// tag are DHL, FedEx, UPS, USPS and Other.
    /// </param>
    /// <param name="trackingNumber">
    /// The &lt;tracking-number&gt; tag contains the shipper's tracking
    /// number that is associated with an order.
    /// </param>
    /// <returns></returns>
    public ShipItemBox AddBox(string carrier, string trackingNumber) {
      if (_boxes.ContainsKey(trackingNumber))
        throw new ApplicationException(
          "You attempted to add a duplicate tracking number.");
      
      ShipItemBox retVal = CreateBox(carrier, trackingNumber);
      _boxes.Add(trackingNumber, retVal);

      return retVal;
    }

    /// <summary>
    /// Add a new item based on a MerchantItemID
    /// </summary>
    /// <param name="carrier">
    /// The &lt;carrier&gt; tag contains the name of the company 
    /// responsible for shipping the item. Valid values for this 
    /// tag are DHL, FedEx, UPS, USPS and Other.
    /// </param>
    /// <param name="trackingNumber">
    /// The &lt;tracking-number&gt; tag contains the shipper's tracking
    /// number that is associated with an order.
    /// </param>
    /// <param name="merchantItemID">
    /// The &lt;merchant-item-id&gt; tag contains a value,
    /// such as a stock keeping unit (SKU), 
    /// that you use to uniquely identify an item.
    /// </param>
    public void AddMerchantItemId(string carrier, string trackingNumber,
      string merchantItemID) {
       
      ShipItemBox box = _boxes[trackingNumber] as ShipItemBox;

      if (box == null) {
        box = CreateBox(carrier, trackingNumber);
        _boxes[trackingNumber] = box;
      }

      box.AddMerchantItemID(merchantItemID);
    }

    /// <summary>Method that is called to produce the Xml message 
    /// that can be posted to Google Checkout.</summary>
    public override byte[] GetXml() {
      //return null;
      AutoGen.ShipItemsRequest retVal = new AutoGen.ShipItemsRequest();
      retVal.googleordernumber = _googleOrderNumber;
      if (_sendEmailSpecified) {
        retVal.sendemail = SendEmail;
        retVal.sendemailSpecified = true;
      }

      retVal.itemshippinginformationlist = ItemShippingInfo;

      return EncodeHelper.Serialize(retVal);

    }

    private ShipItemBox CreateBox(string carrier, string trackingNumber) {
      ShipItemBox retVal = new ShipItemBox(carrier, trackingNumber);
      retVal.Request = this;
      retVal.Carrier = carrier;
      retVal.TrackingNumber = trackingNumber;
      return retVal;
    }

  }
}
