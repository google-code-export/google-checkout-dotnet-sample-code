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

    private AutoGen.DeliverOrderRequest ParseDeliverOrderRequest(byte[] Xml) {
      Type T = typeof(AutoGen.DeliverOrderRequest);
      string Xml2 = EncodeHelper.Utf8BytesToString(Xml);
      return (AutoGen.DeliverOrderRequest) EncodeHelper.Deserialize(Xml2, T);
    }


	}
}
