using System;
using System.Text;
using NUnit.Framework;
using System.Xml;
using System.IO;
using System.Diagnostics;

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
  }
}
