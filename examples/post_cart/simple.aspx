<%@ Import Namespace="GCheckout.Checkout" %>
<%@ Import Namespace="GCheckout.Util" %>
<%@ Register TagPrefix="cc1" Namespace="GCheckout.Checkout" Assembly="GCheckout" %>
<script runat="server" language="c#">

  public void Page_Load(Object sender, EventArgs e)
  {
    // If the items in the cart aren't eligible for Google Checkout, uncomment line below.
    //GCheckoutButton1.Enabled = false;
  }

  private void PostCartToGoogle(object sender, System.Web.UI.ImageClickEventArgs e)
  {
    CheckoutShoppingCartRequest Req = GCheckoutButton1.CreateRequest();
    Req.AddItem("Mars bar", "Packed with peanuts", 0.75m, 2);
	
	//lets make sure we can add 2 different flat rate shipping amounts
	Req.AddFlatRateShippingMethod("UPS Ground", 5);
	Req.AddFlatRateShippingMethod("UPS 2 Day Air", 25);

	//lets try adding a Carrier Calculated Shipping Type

	//The first thing that needs to be done for carrier calculated shipping is we must set the FOB address.
	Req.AddShippingPackage("main", "Cleveland", "OH", "44114", DeliveryAddressCategory.COMMERCIAL, 2, 3, 4);

	//The next thing we will do is add a Fedex Home Package.
	//We will set the default to 3.99, the Pickup to Regular Pickup, the additional fixed charge to 1.29 and the discount to 2.5%
	Req.AddCarrierCalculatedShippingOption(ShippingType.Fedex_Home_Delivery, 3.99m, CarrierPickup.REGULAR_PICKUP, 1.29m, -2.5);
	Req.AddCarrierCalculatedShippingOption(ShippingType.Fedex_Second_Day, 9.99m, CarrierPickup.REGULAR_PICKUP, 2.34m, -24.5);
	
	GCheckoutResponse Resp = Req.Send();
    if (Resp.IsGood)
    {
      Response.Redirect(Resp.RedirectUrl, true);
    }
    else
    {
      Response.Write("Resp.ResponseXml = " + Resp.ResponseXml + "<br>");
      Response.Write("Resp.RedirectUrl = " + Resp.RedirectUrl + "<br>");
      Response.Write("Resp.IsGood = " + Resp.IsGood + "<br>");
      Response.Write("Resp.ErrorMessage = " + Resp.ErrorMessage + "<br>");
    }
  }

</script>
<html>
  <head>
    <title>Simple cart post</title>
  </head>

  <body>
    This page demonstrates a simple cart post.
    <p>
    <form id="Form1" method="post" runat="server">
      <cc1:GCheckoutButton id="GCheckoutButton1" onclick="PostCartToGoogle" runat="server" />
    </form>
  </body>
</html>
