using System;
using System.Text;
using NUnit.Framework;
using System.Xml;
using System.IO;
using System.Diagnostics;
using GCheckout.Util;

namespace GCheckout.Checkout.Tests {

  /// <exclude/>
  [TestFixture()]
  public class CheckoutTests {

    /// <exclude/>
    [Test()]
    public void TestURLParamters() {

      CheckoutShoppingCartRequest request = new CheckoutShoppingCartRequest("123456", "456789", EnvironmentType.Sandbox, "USD", 120);
      ParameterizedUrl url = request.AddParameterizedUrl("http://localhost/default.aspx?url1=test$&url2=false&url3=@@Hello^World", true);

      url = request.AddParameterizedUrl("http://crazyurl.com:8888/crazy dir/default.aspx?url1=test$&url2=false&url3=@@Hello^World", true);

      //Create a second Param
      url = request.AddParameterizedUrl("http://localhost/order.aspx", true);
      url.AddParameter("orderid", UrlParameterType.OrderID);
      url.AddParameter("ordertotal", UrlParameterType.OrderTotal);

      //Now get the XML
      byte[] cart = request.GetXml();

      XmlDocument doc = new XmlDocument();
      XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
      ns.AddNamespace("d", "http://checkout.google.com/schema/2");
      ns.AddNamespace(string.Empty, "http://checkout.google.com/schema/2");

      using (MemoryStream ms = new MemoryStream(cart)) {
        doc.Load(ms);
      }

      XmlNodeList nodeList = doc.SelectNodes("/d:checkout-shopping-cart/d:checkout-flow-support/d:merchant-checkout-flow-support/d:parameterized-urls/d:parameterized-url/d:parameters/d:url-parameter", ns);

      Assert.AreEqual(2, nodeList.Count);

      int index = 0;
      foreach (XmlElement node in nodeList) {
        string name = node.GetAttribute("name");
        string type = node.GetAttribute("type");
        if (index == 0) {
          Assert.AreEqual("orderid", name);
          Assert.AreEqual("order-id", type);
        }
        else {
          Assert.AreEqual("ordertotal", name);
          Assert.AreEqual("order-total", type);        
        }
        index++;
      }

      Debug.WriteLine("");

    }

    /// <exclude/>
    [Test()]
    public void TestAlternateTaxTables() {
      CheckoutShoppingCartRequest request = new CheckoutShoppingCartRequest("123456", "456789", EnvironmentType.Sandbox, "USD", 120);

      //Ensure the factory works as expected
      AlternateTaxTable ohio1 = new AlternateTaxTable("ohio");
      request.AlternateTaxTables.Add(ohio1);
      AlternateTaxTable ohio2 = request.AlternateTaxTables["ohio"];
      AlternateTaxTable ohio3 = new AlternateTaxTable("ohio", false);

      //Ensure that two Tax tables with the same name are not the same reference
      Assert.AreSame(ohio1, ohio2);
      Assert.IsFalse(object.ReferenceEquals(ohio1, ohio3));
      //Assert.AreEqual(ohio1, ohio3);

      //Now add some rules to the item
      ohio1.AddStateTaxRule("OH", .02);

      //Make sure we can add an item to the cart.
      request.AddItem("Item 1", "Cool Candy 1", 2.00M, 1, ohio1);
      try {
        request.AddItem("Item 2", "Cool Candy 2", 2.00M, 1, ohio3);
        Assert.Fail("An exception should have been thrown when we tried to add an item that has a new Tax Reference");
      }
      catch (Exception) {
   
      }

      //Now this should work fine.
      request.AddItem("Item 3", "Cool Candy 3", 2.00M, 1, ohio2);

      //you could create this as an IShoppingCartItem or ShoppingCartItem
      IShoppingCartItem newItem = new ShoppingCartItem("Item 2", "Cool Candy 2", string.Empty, 2.00M, 2);
      //now decide to change your mind on the quantity and price
      newItem.Price = 20;
      newItem.Quantity = 4;
      
      request.AddItem(newItem);

      Assert.AreEqual(1, ohio1.RuleCount);

      DigitalItem emailDigitalItem = new DigitalItem();
      request.AddItem("Email Digital Item", "Cool DigitalItem", 2.00M, 1,  emailDigitalItem);

      DigitalItem urlDigitalItem = new DigitalItem(new Uri("http://www.google.com/download.aspx?myitem=1"), "Url Description for item");
      request.AddItem("Url Digital Item", "Cool Url DigitalItem", 2.00M, 1,  urlDigitalItem);

      DigitalItem keyDigitalItem = new DigitalItem("24-235-sdf-123541-53", "Key Description for item");
      request.AddItem("Url Digital Item", "Cool Url DigitalItem", 2.00M, 1,  keyDigitalItem);

      DigitalItem keyUrlItem = new DigitalItem("24-235-sdf-123541-53","http://www.google.com/download.aspx?myitem=1", "Url/Key Description for item");
      request.AddItem("Url Digital Item", "Cool Url DigitalItem", 2.00M, 1,  keyUrlItem);

      //lets make sure we can add 2 different flat rate shipping amounts

      request.AddFlatRateShippingMethod("UPS Ground", 5);
      request.AddFlatRateShippingMethod("UPS 2 Day Air", 25);

      //lets try adding a Carrier Calculated Shipping Type

      request.AddShippingPackage("main", "Cleveland", "OH", "44114", DeliveryAddressCategory.COMMERCIAL, Packaging.Carrier_Box, 2, 3, 4);
      
      request.AddCarrierCalculatedShippingOption(ShippingType.Fedex_Home_Delivery, 3.99m, CarrierPickup.REGULAR_PICKUP, 1.29m, -2.5);
      request.AddCarrierCalculatedShippingOption(ShippingType.Fedex_Second_Day, 9.99m, CarrierPickup.REGULAR_PICKUP, 2.34m, -24.5);

      //Ensure we are able to create the cart xml

      byte[] cart = request.GetXml();

      //Console.WriteLine(EncodeHelper.Utf8BytesToString(cart));

      //test to see if the item can desialize
      Assert.IsNotNull(GCheckout.Util.EncodeHelper.Deserialize(cart));
    }

    /// <exclude/>
    [Test()]
    public void RequestInitialAuthDetails() {
      byte[] Xml;
      AutoGen.CheckoutShoppingCart Cart;
      CheckoutShoppingCartRequest Req = new CheckoutShoppingCartRequest
        ("123", "456", EnvironmentType.Sandbox, "USD", 0);
      Req.AddItem("Mars bar", "", 0.75m, 2);
      Req.AddFlatRateShippingMethod("USPS", 4.30m);
      // Check the <order-processing-support> tag is not there by default.
      Xml = Req.GetXml();
      Cart = (AutoGen.CheckoutShoppingCart) EncodeHelper.Deserialize(Xml);
      Assert.IsNull(Cart.orderprocessingsupport);
      // Set RequestInitialAuthDetails and check that the XML changes.
      Req.RequestInitialAuthDetails = true;
      Xml = Req.GetXml();
      Cart = (AutoGen.CheckoutShoppingCart) EncodeHelper.Deserialize(Xml);
      Assert.IsNotNull(Cart.orderprocessingsupport);
      Assert.IsTrue(Cart.orderprocessingsupport.requestinitialauthdetails);
      Assert.IsTrue(Cart.orderprocessingsupport.requestinitialauthdetailsSpecified);
    }

    /// <exclude/>
    [Test()]
    public void CartExpiration() {
      byte[] Xml;
      AutoGen.CheckoutShoppingCart Cart;
      CheckoutShoppingCartRequest Req = new CheckoutShoppingCartRequest
        ("123", "456", EnvironmentType.Sandbox, "USD", 0);
      Req.AddItem("Mars bar", "", 0.75m, 2);
      Req.AddFlatRateShippingMethod("USPS", 4.30m);
      Req.SetExpirationMinutesFromNow(10);
      Xml = Req.GetXml();
      Cart = (AutoGen.CheckoutShoppingCart) EncodeHelper.Deserialize(Xml);
      DateTime Exp = Cart.shoppingcart.cartexpiration.gooduntildate;
      Exp = Exp.ToLocalTime();
      TimeSpan T = Exp.Subtract(DateTime.Now);
      Assert.IsTrue(T.TotalSeconds <= 600);
      Assert.IsTrue(T.TotalSeconds > 595);
    }

  }
}
