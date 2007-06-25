using System;
using NUnit.Framework;
using GCheckout.Util;

namespace GCheckout.OrderProcessing.Tests {
  /// <exclude/>
  [TestFixture]
  public class OrderProcessingRequestTests {

    /// <exclude/>
    [Test]
    public void DeliverOrderRequest() {
      DeliverOrderRequest Req;
      AutoGen.DeliverOrderRequest D;
      // Test the first constructor.
      Req = new DeliverOrderRequest("", "", "Sandbox", "1234567890");
      D = ParseDeliverOrderRequest(Req.GetXml());
      Assert.AreEqual("1234567890", D.googleordernumber);
      Assert.AreEqual(false, D.sendemailSpecified);
      Assert.AreEqual(null, D.trackingdata);
      // Test the second constructor.
      Req = new DeliverOrderRequest("", "", "Sandbox", "1234567890", "UPS", 
        "1234", false);
      D = ParseDeliverOrderRequest(Req.GetXml());
      Assert.AreEqual("1234567890", D.googleordernumber);
      Assert.AreEqual(true, D.sendemailSpecified);
      Assert.AreEqual(false, D.sendemail);
      Assert.AreEqual("UPS", D.trackingdata.carrier);
      Assert.AreEqual("1234", D.trackingdata.trackingnumber);
      // Test the third constructor.
      Req = new DeliverOrderRequest("", "", "Sandbox", "1234567890", "UPS", 
        "1234");
      D = ParseDeliverOrderRequest(Req.GetXml());
      Assert.AreEqual("1234567890", D.googleordernumber);
      Assert.AreEqual(false, D.sendemailSpecified);
      Assert.AreEqual("UPS", D.trackingdata.carrier);
      Assert.AreEqual("1234", D.trackingdata.trackingnumber);
      // Test the fourth constructor.
      Req = new DeliverOrderRequest("", "", "Sandbox", "1234567890", false);
      D = ParseDeliverOrderRequest(Req.GetXml());
      Assert.AreEqual("1234567890", D.googleordernumber);
      Assert.AreEqual(true, D.sendemailSpecified);
      Assert.AreEqual(false, D.sendemail);
      Assert.AreEqual(null, D.trackingdata);
    }

    /// <exclude/>
    [Test]
    public void CancelOrderRequest() {
      CancelOrderRequest Req;
      AutoGen.CancelOrderRequest D;
      // Test the first constructor.
      Req = new CancelOrderRequest("", "", "Sandbox", "1234567890", 
        "Wrong size");
      D = ParseCancelOrderRequest(Req.GetXml());
      Assert.AreEqual("1234567890", D.googleordernumber);
      Assert.AreEqual(null, D.comment);
      Assert.AreEqual("Wrong size", D.reason);
      // Test the second constructor.
      Req = new CancelOrderRequest("", "", "Sandbox", "1234567890", 
        "Wrong size", "Buyer called to say the T-shirt was the wrong size.");
      D = ParseCancelOrderRequest(Req.GetXml());
      Assert.AreEqual("1234567890", D.googleordernumber);
      Assert.AreEqual("Buyer called to say the T-shirt was the wrong size.", 
        D.comment);
      Assert.AreEqual("Wrong size", D.reason);
    }

    /// <exclude/>
    [Test, ExpectedException(typeof(ApplicationException))]
    public void InvalidCancelOrderRequest1() {
      // Reason is not allowed to be null.
      CancelOrderRequest Req = new CancelOrderRequest("", "", "Sandbox", 
        "1234567890", null);
    }

    /// <exclude/>
    [Test, ExpectedException(typeof(ApplicationException))]
    public void InvalidCancelOrderRequest2() {
      // Reason is not allowed to be an empty string.
      CancelOrderRequest Req = new CancelOrderRequest("", "", "Sandbox", 
        "1234567890", "");
    }

    /// <exclude/>
    [Test]
    public void ChargeOrderRequest() {
      ChargeOrderRequest Req;
      AutoGen.ChargeOrderRequest D;
      // Test the first constructor.
      Req = new ChargeOrderRequest("", "", "Sandbox", "1234567890");
      D = (AutoGen.ChargeOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual("1234567890", D.googleordernumber);
      Assert.AreEqual(null, D.amount);
      // Test the second constructor.
      Req = new ChargeOrderRequest("", "", "Sandbox", "5354645", "GBP", 10.2m);
      D = (AutoGen.ChargeOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual("5354645", D.googleordernumber);
      Assert.AreEqual("GBP", D.amount.currency);
      Assert.AreEqual(10.2m, D.amount.Value);
    }

    /// <exclude/>
    [Test]
    public void RefundOrderRequest() {
      RefundOrderRequest Req;
      AutoGen.RefundOrderRequest D;
      // Test the first constructor.
      Req = new RefundOrderRequest("", "", "Sandbox", "1234567890", 
        "Not delivered in time");
      D = (AutoGen.RefundOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual("1234567890", D.googleordernumber);
      Assert.AreEqual(null, D.amount);
      Assert.AreEqual(null, D.comment);
      Assert.AreEqual("Not delivered in time", D.reason);
      // Test the second constructor.
      Req = new RefundOrderRequest("", "", "Sandbox", "1234567890", 
        "Not delivered in time", "GBP", 100.98m);
      D = (AutoGen.RefundOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual("1234567890", D.googleordernumber);
      Assert.AreEqual("GBP", D.amount.currency);
      Assert.AreEqual(100.98m, D.amount.Value);
      Assert.AreEqual(null, D.comment);
      Assert.AreEqual("Not delivered in time", D.reason);
      // Test the third constructor.
      Req = new RefundOrderRequest("", "", "Sandbox", "1234567890", 
        "Not delivered in time", "Sorry about that");
      D = (AutoGen.RefundOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual("1234567890", D.googleordernumber);
      Assert.AreEqual(null, D.amount);
      Assert.AreEqual("Sorry about that", D.comment);
      Assert.AreEqual("Not delivered in time", D.reason);
      // Test the fourth constructor.
      Req = new RefundOrderRequest("", "", "Sandbox", "1234567890", 
        "Not delivered in time", "USD", 100.99m, "Sorry about that");
      D = (AutoGen.RefundOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual("1234567890", D.googleordernumber);
      Assert.AreEqual("USD", D.amount.currency);
      Assert.AreEqual(100.99m, D.amount.Value);
      Assert.AreEqual("Sorry about that", D.comment);
      Assert.AreEqual("Not delivered in time", D.reason);
    }

    private AutoGen.DeliverOrderRequest ParseDeliverOrderRequest(byte[] Xml) {
      object testVal = null;

      Type T = typeof(AutoGen.DeliverOrderRequest);

      testVal = EncodeHelper.Deserialize(Xml);
      Assert.IsNotNull(testVal);
      Assert.AreEqual(testVal.GetType(), T);  
      
      string Xml2 = EncodeHelper.Utf8BytesToString(Xml);

      //we want to test the generic Deserialize Method first.
      testVal = EncodeHelper.Deserialize(Xml2);
      Assert.IsNotNull(testVal);
      Assert.AreEqual(testVal.GetType(), T);
      
      return (AutoGen.DeliverOrderRequest) EncodeHelper.Deserialize(Xml2, T);
    }

    private AutoGen.CancelOrderRequest ParseCancelOrderRequest(byte[] Xml) {
      Type T = typeof(AutoGen.CancelOrderRequest);
      string Xml2 = EncodeHelper.Utf8BytesToString(Xml);
      return (AutoGen.CancelOrderRequest) EncodeHelper.Deserialize(Xml2, T);
    }
	}
}
