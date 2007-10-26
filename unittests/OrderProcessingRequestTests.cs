using System;
using NUnit.Framework;
using GCheckout.Util;

namespace GCheckout.OrderProcessing.Tests {
  /// <exclude/>
  [TestFixture]
  public class OrderProcessingRequestTests {

    public const string ORDER_NUMBER = "1234567890";

    /// <exclude/>
    [Test]
    public void DeliverOrderRequest() {
      DeliverOrderRequest Req;
      DeliverOrderRequest Req2;
      AutoGen.DeliverOrderRequest D;

      // Test the first constructor.
      Req = new DeliverOrderRequest("", "", "Sandbox", ORDER_NUMBER);
      D = ParseDeliverOrderRequest(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual(false, D.sendemailSpecified);
      Assert.AreEqual(null, D.trackingdata);

      Req2 = new DeliverOrderRequest(ORDER_NUMBER);
      D = ParseDeliverOrderRequest(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual(false, D.sendemailSpecified);
      Assert.AreEqual(null, D.trackingdata);

      // Test the second constructor.
      Req = new DeliverOrderRequest("", "", "Sandbox", ORDER_NUMBER, "UPS", 
        "1234", false);
      D = ParseDeliverOrderRequest(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual(true, D.sendemailSpecified);
      Assert.AreEqual(false, D.sendemail);
      Assert.AreEqual("UPS", D.trackingdata.carrier);
      Assert.AreEqual("1234", D.trackingdata.trackingnumber);

      Req2 = new DeliverOrderRequest(ORDER_NUMBER, "UPS", "1234", false);
      D = ParseDeliverOrderRequest(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual(true, D.sendemailSpecified);
      Assert.AreEqual(false, D.sendemail);
      Assert.AreEqual("UPS", D.trackingdata.carrier);
      Assert.AreEqual("1234", D.trackingdata.trackingnumber);

      // Test the third constructor.
      Req = new DeliverOrderRequest("", "", "Sandbox", ORDER_NUMBER, "UPS", 
        "1234");
      D = ParseDeliverOrderRequest(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual(false, D.sendemailSpecified);
      Assert.AreEqual("UPS", D.trackingdata.carrier);
      Assert.AreEqual("1234", D.trackingdata.trackingnumber);

      Req2 = new DeliverOrderRequest(ORDER_NUMBER, "UPS", "1234");
      D = ParseDeliverOrderRequest(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual(false, D.sendemailSpecified);
      Assert.AreEqual("UPS", D.trackingdata.carrier);
      Assert.AreEqual("1234", D.trackingdata.trackingnumber);

      // Test the fourth constructor.
      Req = new DeliverOrderRequest("", "", "Sandbox", ORDER_NUMBER, false);
      D = ParseDeliverOrderRequest(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual(true, D.sendemailSpecified);
      Assert.AreEqual(false, D.sendemail);
      Assert.AreEqual(null, D.trackingdata);

      // Test the 8th constructor.
      Req2 = new DeliverOrderRequest(ORDER_NUMBER, false);
      D = ParseDeliverOrderRequest(Req.GetXml());
      Assert.AreEqual(Req2.GoogleOrderNumber, D.googleordernumber);
      Assert.AreEqual(true, D.sendemailSpecified);
      Assert.AreEqual(false, D.sendemail);
      Assert.AreEqual(null, D.trackingdata);
    }

    /// <exclude/>
    [Test]
    public void AuthorizeOrderRequestTests() {
      AuthorizeOrderRequest req = new AuthorizeOrderRequest(ORDER_NUMBER);
      AutoGen.AuthorizeOrderRequest post
        = EncodeHelper.Deserialize(req.GetXml()) as AutoGen.AuthorizeOrderRequest;
      Assert.AreEqual(req.GoogleOrderNumber, post.googleordernumber);

      req = new AuthorizeOrderRequest("", "", "Sandbox", ORDER_NUMBER);
      post
        = EncodeHelper.Deserialize(req.GetXml()) as AutoGen.AuthorizeOrderRequest;
      Assert.AreEqual(req.GoogleOrderNumber, post.googleordernumber);
    }

    /// <exclude/>
    [Test]
    public void SendBuyerMessageRequestTests() {
      SendBuyerMessageRequest req = new SendBuyerMessageRequest(ORDER_NUMBER, "Message", true);
      AutoGen.SendBuyerMessageRequest post = EncodeHelper.Deserialize(req.GetXml()) as AutoGen.SendBuyerMessageRequest;

      Assert.AreEqual(req.GoogleOrderNumber, post.googleordernumber);
      Assert.AreEqual(req.Message, post.message);
      Assert.AreEqual(req.SendEmail, true);

      req = new SendBuyerMessageRequest("", "", "Sandbox", ORDER_NUMBER, "Message", true);
      post = EncodeHelper.Deserialize(req.GetXml()) as AutoGen.SendBuyerMessageRequest;

      Assert.AreEqual(req.GoogleOrderNumber, post.googleordernumber);
      Assert.AreEqual(req.Message, post.message);
      Assert.AreEqual(req.SendEmail, post.sendemail);

      req = new SendBuyerMessageRequest(ORDER_NUMBER, "Message");
      post = EncodeHelper.Deserialize(req.GetXml()) as AutoGen.SendBuyerMessageRequest;

      Assert.AreEqual(req.GoogleOrderNumber, post.googleordernumber);
      Assert.AreEqual(req.Message, post.message);
      Assert.AreEqual(req.SendEmail, post.sendemail);

      req = new SendBuyerMessageRequest("", "", "Sandbox", ORDER_NUMBER, "Message");
      post = EncodeHelper.Deserialize(req.GetXml()) as AutoGen.SendBuyerMessageRequest;

      Assert.AreEqual(req.GoogleOrderNumber, post.googleordernumber);
      Assert.AreEqual(req.Message, post.message);
      Assert.AreEqual(req.SendEmail, post.sendemail, "Send Email");
    }

    /// <exclude/>
    [Test]
    public void CancelOrderRequest() {
      CancelOrderRequest Req;
      AutoGen.CancelOrderRequest D;
      // Test the first constructor.
      Req = new CancelOrderRequest("", "", "Sandbox", ORDER_NUMBER, 
        "Wrong size");
      D = ParseCancelOrderRequest(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual(null, D.comment);
      Assert.AreEqual("Wrong size", D.reason);
      Assert.AreEqual(Req.GoogleOrderNumber, D.googleordernumber);

      // Test the second constructor.
      Req = new CancelOrderRequest("", "", "Sandbox", ORDER_NUMBER, 
        "Wrong size", "Buyer called to say the T-shirt was the wrong size.");
      D = ParseCancelOrderRequest(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual("Buyer called to say the T-shirt was the wrong size.", 
        D.comment);
      Assert.AreEqual("Wrong size", D.reason);

      Req = new CancelOrderRequest(ORDER_NUMBER, "Wrong size");
      D = ParseCancelOrderRequest(Req.GetXml());

      Req = new CancelOrderRequest(ORDER_NUMBER, "Wrong size", "Test Comment");
      D = ParseCancelOrderRequest(Req.GetXml());

    }

    /// <exclude/>
    [Test, ExpectedException(typeof(ApplicationException))]
    public void InvalidCancelOrderRequest1() {
      // Reason is not allowed to be null.
      CancelOrderRequest Req = new CancelOrderRequest("", "", "Sandbox", 
        ORDER_NUMBER, null);
    }

    /// <exclude/>
    [Test, ExpectedException(typeof(ApplicationException))]
    public void InvalidCancelOrderRequest2() {
      // Reason is not allowed to be an empty string.
      CancelOrderRequest Req = new CancelOrderRequest("", "", "Sandbox", 
        ORDER_NUMBER, "");
    }

    /// <exclude/>
    [Test]
    public void ChargeOrderRequest() {
      ChargeOrderRequest Req;
      AutoGen.ChargeOrderRequest D;

      // Test the first constructor.
      Req = new ChargeOrderRequest("", "", "Sandbox", ORDER_NUMBER);
      D = (AutoGen.ChargeOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual(null, D.amount);
      
      // Test the second constructor.
      Req = new ChargeOrderRequest("", "", "Sandbox", "5354645", "GBP", 10.2m);
      D = (AutoGen.ChargeOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual("5354645", D.googleordernumber);
      Assert.AreEqual("GBP", D.amount.currency);
      Assert.AreEqual(10.2m, D.amount.Value);
    
      Req = new ChargeOrderRequest(ORDER_NUMBER);
      D = (AutoGen.ChargeOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual(Req.GoogleOrderNumber, D.googleordernumber);
 
      Req = new ChargeOrderRequest(ORDER_NUMBER, "USD", 12.95m);
      D = (AutoGen.ChargeOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual(Req.GoogleOrderNumber, D.googleordernumber);

    }

    /// <exclude/>
    [Test]
    public void ProcessOrderRequest() {
      byte[] xml;
      ProcessOrderRequest req = new ProcessOrderRequest(ORDER_NUMBER);
      xml = req.GetXml();

      AutoGen.ProcessOrderRequest post
        = EncodeHelper.Deserialize(xml) as AutoGen.ProcessOrderRequest;

      Assert.AreEqual(req.GoogleOrderNumber, post.googleordernumber);

      req = new ProcessOrderRequest("", "", "Sandbox", ORDER_NUMBER);
      xml =  req.GetXml();

    
    }

    /// <exclude/>
    [Test]
    public void RefundOrderRequest() {
      RefundOrderRequest Req;
      AutoGen.RefundOrderRequest D;
      // Test the first constructor.
      Req = new RefundOrderRequest("", "", "Sandbox", ORDER_NUMBER, 
        "Not delivered in time");
      D = (AutoGen.RefundOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual(null, D.amount);
      Assert.AreEqual(null, D.comment);
      Assert.AreEqual("Not delivered in time", D.reason);
      Assert.AreEqual(Req.GoogleOrderNumber, D.googleordernumber);

      // Test the second constructor.
      Req = new RefundOrderRequest("", "", "Sandbox", ORDER_NUMBER, 
        "Not delivered in time", "GBP", 100.98m);
      D = (AutoGen.RefundOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual("GBP", D.amount.currency);
      Assert.AreEqual(100.98m, D.amount.Value);
      Assert.AreEqual(null, D.comment);
      Assert.AreEqual("Not delivered in time", D.reason);

      // Test the third constructor.
      Req = new RefundOrderRequest("", "", "Sandbox", ORDER_NUMBER, 
        "Not delivered in time", "Sorry about that");
      D = (AutoGen.RefundOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual(0, D.amount.Value);
      Assert.AreEqual("Sorry about that", D.comment);
      Assert.AreEqual("Not delivered in time", D.reason);

      // Test the fourth constructor.
      Req = new RefundOrderRequest("", "", "Sandbox", ORDER_NUMBER, 
        "Not delivered in time", "USD", 100.99m, "Sorry about that");
      D = (AutoGen.RefundOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual("USD", D.amount.currency);
      Assert.AreEqual(100.99m, D.amount.Value);
      Assert.AreEqual("Sorry about that", D.comment);
      Assert.AreEqual("Not delivered in time", D.reason);

      // Test the fifth constructor.
      Req = new RefundOrderRequest(ORDER_NUMBER, "Not delivered in time");
      D = (AutoGen.RefundOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual(null, D.comment);
      Assert.AreEqual("Not delivered in time", D.reason);

      // Test the sixth constructor.
      Req = new RefundOrderRequest(ORDER_NUMBER, 
        "Not delivered in time", "USD", 100.99m);
      D = (AutoGen.RefundOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual(100.99m, D.amount.Value);
      Assert.AreEqual(null, D.comment);
      Assert.AreEqual("Not delivered in time", D.reason);

      // Test the seventh constructor.
      Req = new RefundOrderRequest(ORDER_NUMBER, 
        "Not delivered in time", "Sorry about that");
      D = (AutoGen.RefundOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual(0, D.amount.Value);
      Assert.AreEqual("Sorry about that", D.comment);
      Assert.AreEqual("Not delivered in time", D.reason);

      // Test the eighth constructor.
      Req = new RefundOrderRequest(ORDER_NUMBER, 
        "Not delivered in time", "USD", 100.99m, "Sorry about that");
      D = (AutoGen.RefundOrderRequest) EncodeHelper.Deserialize(Req.GetXml());
      Assert.AreEqual(ORDER_NUMBER, D.googleordernumber);
      Assert.AreEqual(100.99m, D.amount.Value);
      Assert.AreEqual("Sorry about that", D.comment);
      Assert.AreEqual("Not delivered in time", D.reason);

    }

    /// <exclude/>
    [Test()]
    public void PostUrl() {
      DeliverOrderRequest Req;
      // Sandbox.
      Req = new DeliverOrderRequest("123", "456", "Sandbox", "789", false);
      Assert.AreEqual("https://sandbox.google.com/checkout/api/checkout/v2/request/Merchant/123", Req.GetPostUrl());
      Req = new DeliverOrderRequest("123", "456", "Production", "789", false);
      Assert.AreEqual("https://checkout.google.com/api/checkout/v2/request/Merchant/123", Req.GetPostUrl());
    }

    /// <exclude/>
    [Test()]
    public void AddTrackingDataRequestTests() {
      AddTrackingDataRequest req
        = new AddTrackingDataRequest("123", "456", "SandBox", 
        ORDER_NUMBER, "UPS", "Z1234567890");
      Assert.AreEqual(ORDER_NUMBER, req.GoogleOrderNumber);
      Assert.AreEqual("UPS", req.Carrier);
      Assert.AreEqual("Z1234567890", req.TrackingNo);
      req.GetXml();

      req = new AddTrackingDataRequest(ORDER_NUMBER, "UPS", "Z1234567890");
      Assert.AreEqual("UPS", req.Carrier);
      Assert.AreEqual("Z1234567890", req.TrackingNo);
      req.GetXml();
    }

    /// <exclude/>
    [Test()]
    public void ArchiveOrderRequestTests() {
      ArchiveOrderRequest req = new ArchiveOrderRequest(ORDER_NUMBER);
      ArchiveOrderRequest req2 
        = EncodeHelper.Deserialize(req.GetXml()) as ArchiveOrderRequest;

      Assert.AreEqual(ORDER_NUMBER, req.GoogleOrderNumber);

      req = new ArchiveOrderRequest("123", "456", "SandBox",ORDER_NUMBER);
      Assert.AreEqual(ORDER_NUMBER, req.GoogleOrderNumber);

    }

    /// <exclude/>
    [Test()]
    public void TestAddMerchantOrderNumberRequest() {
      AddMerchantOrderNumberRequest req = new AddMerchantOrderNumberRequest(ORDER_NUMBER, "ABCDEFGHIJ");
      AutoGen.AddMerchantOrderNumberRequest D 
        = EncodeHelper.Deserialize(req.GetXml()) as AutoGen.AddMerchantOrderNumberRequest;

      Assert.AreEqual(req.GoogleOrderNumber, D.googleordernumber);
      Assert.AreEqual("ABCDEFGHIJ", D.merchantordernumber);

      req = new AddMerchantOrderNumberRequest("123", "456", "SandBox", ORDER_NUMBER, "ABCDEFGHIJ");
      D = EncodeHelper.Deserialize(req.GetXml()) as AutoGen.AddMerchantOrderNumberRequest;

      Assert.AreEqual(req.GoogleOrderNumber, D.googleordernumber);
      Assert.AreEqual("ABCDEFGHIJ", D.merchantordernumber);
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
