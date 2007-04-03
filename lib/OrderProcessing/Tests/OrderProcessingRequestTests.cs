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
      CancelOrderRequest Req = new CancelOrderRequest("", "", "Sandbox", 
        "1234567890", null);
    }

    /// <exclude/>
    [Test, ExpectedException(typeof(ApplicationException))]
    public void InvalidCancelOrderRequest2() {
      CancelOrderRequest Req = new CancelOrderRequest("", "", "Sandbox", 
        "1234567890", "");
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
