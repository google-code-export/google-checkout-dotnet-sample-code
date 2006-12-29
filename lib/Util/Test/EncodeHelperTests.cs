using System;
using System.Text;
using NUnit.Framework;

namespace GCheckout.Util.Test
{
  /// <exclude/>
  [TestFixture()]
  public class EncodeHelperTests
	{
    /// <exclude/>
    [Test()]
    public void StringToUtf8BytesAndBack() {
      string Orig = "abc€Œ™©®åëñöÿ!\"#$%&'()*+,-./שּׁзγəˆỊ₪€₧ﻷ";
      Assert.AreEqual(Orig,
        EncodeHelper.Utf8BytesToString(EncodeHelper.StringToUtf8Bytes(Orig)));
    }

    /// <exclude/>
    [Test()]
    public void GetTopElement_OK() {
      byte[] B = EncodeHelper.Serialize(CreateNewOrderNotification());
      Assert.AreEqual("new-order-notification", EncodeHelper.GetTopElement(B));
      string Xml = EncodeHelper.Utf8BytesToString(B);
      Assert.AreEqual("new-order-notification", 
        EncodeHelper.GetTopElement(Xml));
    }

    /// <exclude/>
    [Test()]
    public void GetGoogleOrderNumber_OK() {
      byte[] B = EncodeHelper.Serialize(CreateNewOrderNotification());
      Assert.AreEqual("841171949013218", EncodeHelper.GetGoogleOrderNumber(B));
    }

    /// <exclude/>
    [Test()]
    public void GetGoogleOrderNumber_NotFound() {
      AutoGen.NewOrderNotification N = new AutoGen.NewOrderNotification();
      byte[] B = EncodeHelper.Serialize(N);
      // Check that B does not contain a "google-order-number" element.
      String Xml = EncodeHelper.Utf8BytesToString(B);
      Assert.IsTrue(Xml.IndexOf("google-order-number") == -1);
      // Check that GetGoogleOrderNumber() returns an empty string if there is
      // no "google-order-number" element.
      Assert.AreEqual("", EncodeHelper.GetGoogleOrderNumber(B));
    }

    /// <exclude/>
    [Test()]
    public void SerializeAndDeserialize() {
      AutoGen.NewOrderNotification N1 = CreateNewOrderNotification();
      byte[] B = EncodeHelper.Serialize(N1);
      AutoGen.NewOrderNotification N2 = (AutoGen.NewOrderNotification) 
        EncodeHelper.Deserialize(B, typeof(AutoGen.NewOrderNotification));
      Assert.AreEqual(N1.googleordernumber, N2.googleordernumber);
      Assert.AreEqual(N1.buyerid, N2.buyerid);
      Assert.AreEqual(N1.serialnumber, N2.serialnumber);
      Assert.AreEqual(N1.financialorderstate, N2.financialorderstate);
      Assert.AreEqual(N1.timestamp, N2.timestamp);
      Assert.AreEqual(N1.shoppingcart.items.Length, 
        N2.shoppingcart.items.Length);
      Assert.AreEqual(N1.shoppingcart.items[0].itemname, 
        N2.shoppingcart.items[0].itemname);
      Assert.AreEqual(N1.shoppingcart.items[0].itemdescription, 
        N2.shoppingcart.items[0].itemdescription);
      Assert.AreEqual(N1.shoppingcart.items[0].quantity, 
        N2.shoppingcart.items[0].quantity);
      Assert.AreEqual(N1.shoppingcart.items[0].unitprice.currency, 
        N2.shoppingcart.items[0].unitprice.currency);
      Assert.AreEqual(N1.shoppingcart.items[0].unitprice.Value, 
        N2.shoppingcart.items[0].unitprice.Value);
      Assert.AreEqual(N1.shoppingcart.items[1].itemname, 
        N2.shoppingcart.items[1].itemname);
      Assert.AreEqual(N1.shoppingcart.items[1].itemdescription, 
        N2.shoppingcart.items[1].itemdescription);
      Assert.AreEqual(N1.shoppingcart.items[1].quantity, 
        N2.shoppingcart.items[1].quantity);
      Assert.AreEqual(N1.shoppingcart.items[1].unitprice.currency, 
        N2.shoppingcart.items[1].unitprice.currency);
      Assert.AreEqual(N1.shoppingcart.items[1].unitprice.Value, 
        N2.shoppingcart.items[1].unitprice.Value);
    }

    /// <exclude/>
    [Test()]
    public void EncodeXmlChars() {
      string In = "$25 > $20 & $100 < $200 <<&&>>";
      string Expected = "$25 &#x3e; $20 &#x26; $100 &#x3c; $200 &#x3c;&#x3c;" +
        "&#x26;&#x26;&#x3e;&#x3e;";
      Assert.AreEqual(Expected, EncodeHelper.EscapeXmlChars(In));
    }

    /// <exclude/>
    private AutoGen.NewOrderNotification CreateNewOrderNotification() {
      AutoGen.NewOrderNotification N1 = new AutoGen.NewOrderNotification();
      N1.googleordernumber = "841171949013218";
      N1.buyerid = 379653;
      N1.serialnumber = "fc8n593wfhfoc8nwot8";
      N1.timestamp = DateTime.Now;
      N1.shoppingcart = new AutoGen.ShoppingCart();
      N1.shoppingcart.items = new AutoGen.Item[2];
      N1.shoppingcart.items[0] = new AutoGen.Item();
      N1.shoppingcart.items[0].itemname = "Vanilla Coffee Syrup";
      N1.shoppingcart.items[0].itemdescription = "From Espresso House";
      N1.shoppingcart.items[0].quantity = 10;
      N1.shoppingcart.items[0].unitprice = new AutoGen.Money();
      N1.shoppingcart.items[0].unitprice.currency = "USD";
      N1.shoppingcart.items[0].unitprice.Value = 5.05m;
      N1.shoppingcart.items[1] = new AutoGen.Item();
      N1.shoppingcart.items[1].itemname = "Nescafé Cappuccino ©";
      N1.shoppingcart.items[1].itemdescription = "שּׁзγəˆỊ₪€₧ﻷ";
      N1.shoppingcart.items[1].quantity = 2;
      N1.shoppingcart.items[1].unitprice = new AutoGen.Money();
      N1.shoppingcart.items[1].unitprice.currency = "SEK";
      N1.shoppingcart.items[1].unitprice.Value = 23.50m;
      return N1;
    }

	}
}
