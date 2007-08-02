<%@ Page Language="VB" %>
<%@ Import Namespace="GCheckout.Checkout" %>
<%@ Import Namespace="GCheckout.Util" %>
<%@ Register TagPrefix="cc1" Namespace="GCheckout.Checkout" Assembly="GCheckout" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
	Public Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		' If the items in the cart aren't eligible for Google Checkout, uncomment line below.
		'GCheckoutButton1.Enabled = false;
	End Sub
    
	Private Sub PostCartToGoogle(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
		Dim Req As CheckoutShoppingCartRequest = GCheckoutButton1.CreateRequest
		' First we are going to create a multi node merchant-private data.
		' We are going to treat it like a name value pair so we can store multiple values.
		' You can do this in many different ways, including creating many XmlDocuments.
		Dim doc As System.Xml.XmlDocument = New System.Xml.XmlDocument
		doc.LoadXml("<root />")
		Dim element As System.Xml.XmlElement = doc.CreateElement("more")
		element.InnerText = "We need to know this"
		doc.DocumentElement.AppendChild(element)
		Req.AddMerchantPrivateDataNode(doc.DocumentElement)
		' Add a "shopper-id" node.
		Dim tempDoc As System.Xml.XmlDocument = New System.Xml.XmlDocument
		Dim tempNode As System.Xml.XmlNode = tempDoc.CreateElement("shopper-id")
		tempNode.InnerText = "1234567890"
		Req.AddMerchantPrivateDataNode(tempNode)
		' Add a "cart-id" node.
		Dim tempNode2 As System.Xml.XmlNode = tempDoc.CreateElement("cart-id")
		tempNode2.InnerText = "0987654321"
		Req.AddMerchantPrivateDataNode(tempNode2)
		' We just created this structure on the order level:
		' <merchant-private-data>
		'   <root xmlns="">
		'     <more>We need to know this</more>
		'   </root>
		'   <shopper-id xmlns="">1234567890</shopper-id>
		'   <cart-id xmlns="">0987654321</cart-id>
		' </merchant-private-data>
		' Now we are going to add a few products.
		Req.AddItem("Candy Bar", "Big Candy bar that costs $1.00", "arx-112", 1, 1)
		' Now we are going to create multiple xml nodes that can be passed into the next item.
		tempNode = tempDoc.CreateElement("supplier-id")
		tempNode.InnerText = "ABC Candy Company"
		Req.AddItem("Fat Free Candy Bar", "Candy bar that looks like paper pulp", "candy-paper", 0.45, 2, tempNode)
		tempNode = tempDoc.CreateElement("supplier-id")
		tempNode.InnerText = "ABC Candy Company"
		tempNode2 = tempDoc.CreateElement("quantity-on-hand")
		tempNode2.InnerText = "45"
		Req.AddItem("Flat Candy", "Candy run over by the truck", "candy-flat", 0.02, 200, tempNode, tempNode2)
		Dim Resp As GCheckoutResponse = Req.Send
		If Resp.IsGood Then
			Response.Redirect(Resp.RedirectUrl, True)
		Else
			Response.Write("Resp.ResponseXml = " & Resp.ResponseXml & "<br>")
			Response.Write("Resp.RedirectUrl = " & Resp.RedirectUrl & "<br>")
			Response.Write("Resp.IsGood = " & Resp.IsGood & "<br>")
			Response.Write("Resp.ErrorMessage = " & Resp.ErrorMessage & "<br>")
		End If
	End Sub


</script>

<html>
  <head>
    <title>Cart post with merchant private data</title>
  </head>

  <body>
    This page demonstrates a cart post with merchant private data.
    <br />
    <form id="Form1" method="post" runat="server">
      <cc1:GCheckoutButton id="GCheckoutButton1" onclick="PostCartToGoogle" runat="server" />
    </form>
  </body>
</html>
