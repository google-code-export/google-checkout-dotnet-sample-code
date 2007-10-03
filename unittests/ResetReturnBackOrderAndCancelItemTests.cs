using System;
using System.Text;
using System.Xml;
using NUnit.Framework;
using GCheckout.Util;

namespace GCheckout.OrderProcessing.Tests {

  /// <exclude/>
  [TestFixture]
  public class ResetReturnBackOrderAndCancelItemTests {
    string originalOrderID = "841171949013218";

    /// <exclude/>
    [Test]
    public void VerifyResetReturnBackOrderAndCancelItem() {
      
      string xml;
      ResetItemsShippingInformationRequest req1 =
        new ResetItemsShippingInformationRequest(
        "1234", "5678", EnvironmentType.Sandbox.ToString(), 
        originalOrderID);
      req1.AddMerchantItemId("A1");
      req1.AddMerchantItemId("B1");
      req1.AddMerchantItemId("123456");

      xml = Util.EncodeHelper.Utf8BytesToString(req1.GetXml());
      VerifyMessage(xml, typeof(AutoGen.ResetItemsShippingInformationRequest));


      BackorderItemsRequest req2 =
        new BackorderItemsRequest(
        "1234", "5678", EnvironmentType.Sandbox.ToString(), 
        originalOrderID);
      req2.AddMerchantItemId("A1");
      req2.AddMerchantItemId("B1");
      req2.AddMerchantItemId("123456");

      xml = Util.EncodeHelper.Utf8BytesToString(req2.GetXml());
      VerifyMessage(xml, typeof(AutoGen.BackorderItemsRequest));


      CancelItemsRequest req3 =
        new CancelItemsRequest(
        "1234", "5678", EnvironmentType.Sandbox.ToString(), 
        originalOrderID);
      req3.AddMerchantItemId("A1");
      req3.AddMerchantItemId("B1");
      req3.AddMerchantItemId("123456");

      xml = Util.EncodeHelper.Utf8BytesToString(req3.GetXml());
      VerifyMessage(xml, typeof(AutoGen.CancelItemsRequest));

      ReturnItemsRequest req4 =
        new ReturnItemsRequest(
        "1234", "5678", EnvironmentType.Sandbox.ToString(), 
        originalOrderID);
      req4.AddMerchantItemId("A1");
      req4.AddMerchantItemId("B1");
      req4.AddMerchantItemId("123456");

      xml = Util.EncodeHelper.Utf8BytesToString(req4.GetXml());
      VerifyMessage(xml, typeof(AutoGen.ReturnItemsRequest));


    }

    private void VerifyMessage(string xml, Type theType) {

      //verify the type first
      object obj = Util.EncodeHelper.Deserialize(xml);

      Assert.AreEqual(obj.GetType(), theType);

      StringBuilder sb;
      //XmlNode item;
      XmlNodeList list;
      System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
      XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
      ns.AddNamespace("d", "http://checkout.google.com/schema/2");
      ns.AddNamespace(string.Empty, "http://checkout.google.com/schema/2");

      doc.LoadXml(xml);

      string googleOrderID = doc.DocumentElement.SelectSingleNode(
        "@google-order-number").InnerText;
      Assert.AreEqual(originalOrderID, googleOrderID);

      //ensure we have 3 items

      sb = new StringBuilder();
      sb.Append("/d:");
      sb.Append(Util.EncodeHelper.GetTopElement(xml));
      sb.Append("/d:item-ids/d:item-id");

      list = doc.DocumentElement.SelectNodes(sb.ToString(), ns);
      Assert.AreEqual(list.Count, 3);

      //we should have 2 merchant items
      sb = new StringBuilder();
      sb.Append("/d:");
      sb.Append(Util.EncodeHelper.GetTopElement(xml));
      sb.Append("/d:item-ids/d:item-id/d:merchant-item-id");

      list = doc.DocumentElement.SelectNodes(sb.ToString(), ns);
      Assert.AreEqual(list.Count, 3);

      Console.WriteLine("");       
    }

  }
}
