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
		Req.AddItem("Mars bar", "Packed with peanuts", 0.75, 2)
        Dim Resp As GCheckoutResponse = Req.Send
        If Resp.IsGood Then
            Response.Redirect(Resp.RedirectUrl, true)
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
    <title>Simple cart post</title>
  </head>

  <body>
    This page demonstrates a simple cart post.
    <br />
    <form id="Form1" method="post" runat="server">
      <cc1:GCheckoutButton id="GCheckoutButton1" onclick="PostCartToGoogle" runat="server" />
    </form>
  </body>
</html>
