using System;
using System.Text;
using System.Xml;
using NUnit.Framework;
using GCheckout.Util;

namespace GCheckout.OrderProcessing.Tests
{

  /// <exclude/>
  [TestFixture]
  public class ShipItemsRequestTests
	{
    string originalOrderID = "841171949013218";

    /// <exclude/>
    [Test]
    public void TwoItemsShippedInTwoBoxes() {

      ShipItemBox box;
      string xml;

      Console.WriteLine("Using AddMerchantItemId");
      ShipItemsRequest req = new ShipItemsRequest(
      "1234", "5678", EnvironmentType.Sandbox.ToString(), originalOrderID);
      req.SendEmail = true;
      req.AddMerchantItemId("UPS", "55555555", "A1");
      req.AddMerchantItemId("UPS", "77777777", "B2");
      
      Assert.AreEqual(req.ItemShippingInfo.Length, 2);
      
      xml = Util.EncodeHelper.Utf8BytesToString(req.GetXml());
      VerifyTwoItemsShippingInTwoBoxes(xml, false);

      Console.WriteLine("Using Boxes");

      //now we want to try the same thing with adding boxes and
      //putting items into the boxes.
      req = new ShipItemsRequest(
        "1234", "5678", EnvironmentType.Sandbox.ToString(), originalOrderID);
      req.SendEmail = true;
      box = req.AddBox("UPS", "55555555");
      box.AddMerchantItemID("A1");
      box = req.AddBox("UPS", "77777777");
      box.AddMerchantItemID("B2");

      Assert.AreEqual(req.ItemShippingInfo.Length, 2);
      
      xml = Util.EncodeHelper.Utf8BytesToString(req.GetXml());
      VerifyTwoItemsShippingInTwoBoxes(xml, false);

      Console.WriteLine("Google Items using AddGoogleItemId");

      req = new ShipItemsRequest(
        "1234", "5678", EnvironmentType.Sandbox.ToString(), originalOrderID);
      req.SendEmail = true;
      req.AddMerchantItemId("UPS", "55555555", "123456");
      req.AddMerchantItemId("UPS", "77777777", "7891234");
      
      Assert.AreEqual(req.ItemShippingInfo.Length, 2);
      
      xml = Util.EncodeHelper.Utf8BytesToString(req.GetXml());
      VerifyTwoItemsShippingInTwoBoxes(xml, true);

      Console.WriteLine("Google Items using Boxes");

      //now we want to try the same thing with adding boxes and
      //putting items into the boxes.
      req = new ShipItemsRequest(
        "1234", "5678", EnvironmentType.Sandbox.ToString(), originalOrderID);
      req.SendEmail = true;
      box = req.AddBox("UPS", "55555555");
      box.AddMerchantItemID("123456");
      box = req.AddBox("UPS", "77777777");
      box.AddMerchantItemID("7891234");

      Assert.AreEqual(req.ItemShippingInfo.Length, 2);
      
      xml = Util.EncodeHelper.Utf8BytesToString(req.GetXml());
      VerifyTwoItemsShippingInTwoBoxes(xml, true);

    }

    private void VerifyTwoItemsShippingInTwoBoxes(string xml, bool googleItems) {
      XmlNode item;
      XmlNode childItem;
      XmlNodeList list;
      StringBuilder sb;
      
      System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
      XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
      ns.AddNamespace("d", "http://checkout.google.com/schema/2");
      ns.AddNamespace(string.Empty, "http://checkout.google.com/schema/2");

      doc.LoadXml(xml);

      string googleOrderID = doc.DocumentElement.SelectSingleNode(
        "@google-order-number").InnerText;
      Assert.AreEqual(originalOrderID, googleOrderID);

      sb = new StringBuilder();
      sb.Append("/d:ship-items/d:item-shipping-information-list");
      sb.Append("/d:item-shipping-information");
      if (googleItems)
        sb.Append("[d:item-id/d:merchant-item-id = '123456']");
      else
        sb.Append("[d:item-id/d:merchant-item-id = 'A1']");


      item = doc.SelectSingleNode(sb.ToString(), ns);
      Assert.IsNotNull(item);

      //Try to obtain only one tracking node
      sb = new StringBuilder("d:tracking-data-list/d:tracking-data");
      list = item.SelectNodes(sb.ToString(), ns);
      Assert.AreEqual(list.Count, 1);

      //now try for the 55555555 tracking number
      sb.Append("[d:tracking-number = '55555555']");
      childItem = item.SelectSingleNode(sb.ToString(), ns);
      Assert.IsNotNull(childItem);

      sb = new StringBuilder();
      sb.Append("/d:ship-items/d:item-shipping-information-list");
      sb.Append("/d:item-shipping-information");
      if (googleItems)
        sb.Append("[d:item-id/d:merchant-item-id = '7891234']");
      else
        sb.Append("[d:item-id/d:merchant-item-id = 'B2']");

      item = doc.SelectSingleNode(sb.ToString(), ns);
      Assert.IsNotNull(item);

      //Try to obtain only one tracking node
      sb = new StringBuilder("d:tracking-data-list/d:tracking-data");
      list = item.SelectNodes(sb.ToString(), ns);
      Assert.AreEqual(list.Count, 1);

      //now try for the 55555555 tracking number
      sb.Append("[d:tracking-number = '77777777']");
      childItem = item.SelectSingleNode(sb.ToString(), ns);
      Assert.IsNotNull(childItem);    
    }

    /// <exclude/>
    [Test]
    public void TwoItemsShipInTheSameBox() {
      ShipItemBox box;
      string xml;

      Console.WriteLine("Using AddMerchantItemId");
      ShipItemsRequest req = new ShipItemsRequest(
        "1234", "5678", EnvironmentType.Sandbox.ToString(), originalOrderID);
      req.SendEmail = true;
      req.AddMerchantItemId("UPS", "55555555", "A1");
      req.AddMerchantItemId("UPS", "55555555", "B2");
      
      Assert.AreEqual(req.ItemShippingInfo.Length, 2);
      xml = Util.EncodeHelper.Utf8BytesToString(req.GetXml());

      Console.WriteLine("Using Boxes");

      //now we want to try the same thing with adding boxes and
      //putting items into the boxes.
      req = new ShipItemsRequest(
        "1234", "5678", EnvironmentType.Sandbox.ToString(), originalOrderID);
      req.SendEmail = true;
      box = req.AddBox("UPS", "55555555");
      box.AddMerchantItemID("A1");
      box.AddMerchantItemID("B2");

      Assert.AreEqual(req.ItemShippingInfo.Length, 2);
      
      xml = Util.EncodeHelper.Utf8BytesToString(req.GetXml());

    }

    /// <exclude/>
    [Test]
    public void OneItemShipsInTwoBoxes() {
      ShipItemBox box;
      string xml;

      Console.WriteLine("Using AddMerchantItemId");
      ShipItemsRequest req = new ShipItemsRequest(
        "1234", "5678", EnvironmentType.Sandbox.ToString(), originalOrderID);
      req.SendEmail = true;
      req.AddMerchantItemId("UPS", "55555555", "A1");
      req.AddMerchantItemId("UPS", "77777777", "A1");
      
      Assert.AreEqual(req.ItemShippingInfo.Length, 1);
      xml = Util.EncodeHelper.Utf8BytesToString(req.GetXml());

      Console.WriteLine("Using Boxes");

      //now we want to try the same thing with adding boxes and
      //putting items into the boxes.
      req = new ShipItemsRequest(
        "1234", "5678", EnvironmentType.Sandbox.ToString(), originalOrderID);
      req.SendEmail = true;
      box = req.AddBox("UPS", "55555555");
      box.AddMerchantItemID("A1");
      box = req.AddBox("UPS", "77777777");
      box.AddMerchantItemID("A1");

      Assert.AreEqual(req.ItemShippingInfo.Length, 1);
      
      xml = Util.EncodeHelper.Utf8BytesToString(req.GetXml());
      
    }

	}
}
