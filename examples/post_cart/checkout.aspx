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
		Req.AddItem("Copyright symbol: © <>", "", 0.75m, 2, "<candy-data><sugar-content>1.0 oz</sugar-content><wrapper><color>Brown</color><material>paper</material></wrapper></candy-data>");
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

//		HardcodedShoppingCartRequest Req = new HardcodedShoppingCartRequest("536985267838521", "Ku0RHxdns0e3Ca-Likqe3A", GCheckout.EnvironmentType.Sandbox);
//		GCheckoutResponse Resp = Req.Send();


/*
		
		byte[] xml = Req.GetXml();
		for (int i = 0; i < xml.Length; i++) {
	      	Response.Write(xml[i] + "\t" + Convert.ToChar(xml[i]));
	      	Response.Write("\n");
		}


		Response.Write("Resp.ResponseXml = " + Resp.ResponseXml + "<br>");
		Response.Write("Resp.RedirectUrl = " + Resp.RedirectUrl + "<br>");
		Response.Write("Resp.IsGood = " + Resp.IsGood + "<br>");
		Response.Write("Resp.ErrorMessage = " + Resp.ErrorMessage + "<br>");
*/
//		Response.Redirect(Resp.RedirectUrl, true);




/*
		//Req.AddItem("Mars bar", "Packed with peanuts", 0.75m, 2);
		//Req.AddFlatRateShippingMethod("SuperSaver", 2.50m);
		//Req.AddFlatRateShippingMethod("Standard Ground", 5.00m);
		Req.AddStateTaxRule("CA", 0.0875, true);

		// Loop over all items in the user's shopping cart and call AddItem() for each one.
		//Req.AddItem("Mars bar", "Creamy and nutty", 0.75m, 2);

		Req.AddItem("Mars bar ©", "Creamy and nutty", 0.75m, 2, "<candy-data><sugar-content>1.0 oz</sugar-content><wrapper><color>Brown</color><material>paper</material></wrapper></candy-data>");
		Req.AddItem("Twix bar          å", "Crunchy goodness", 0.85m, 1, "<candy-data><sugar-content>1.1 oz</sugar-content><wrapper><color>Gold</color><material>paper</material></wrapper></candy-data>");
		
		Req.MerchantPrivateData = "<mydata>5</mydata>";

		// Contiguous State.
		ShippingRestrictions ContiguousState = new ShippingRestrictions();
		ContiguousState.AddAllowedCountryArea(GCheckout.AutoGen.USAreas.CONTINENTAL_48);
		Req.AddFlatRateShippingMethod("Standard Shipping (contiguous)", 1, ContiguousState);

		Req.AddStateTaxRule("CA", 0.0875, true);
		//Req.MerchantCalculatedTax = true;

		//Req.AcceptMerchantCoupons = true;
		//Req.MerchantCalculationsUrl = "http://ecom.ccsols.com/ngs/mo/callback/callback.aspx";

		// Non-contiguous State.
		ShippingRestrictions NonContiguousState = new ShippingRestrictions();
		NonContiguousState.AddAllowedStateCode("AK");
		NonContiguousState.AddAllowedStateCode("HI");
		Req.AddFlatRateShippingMethod("Standard Shipping (non-contiguous)", 8, NonContiguousState);
		
		// Territory.
		ShippingRestrictions Territory = new ShippingRestrictions();
		Territory.AddAllowedStateCode("FM");
		Territory.AddAllowedStateCode("AS");
		Territory.AddAllowedStateCode("GU");
		Territory.AddAllowedStateCode("MH");
		Territory.AddAllowedStateCode("MP");
		Territory.AddAllowedStateCode("PW");
		Territory.AddAllowedStateCode("PR");
		Territory.AddAllowedStateCode("RI");
		Territory.AddAllowedStateCode("VI");
		Req.AddFlatRateShippingMethod("Standard Shipping (territory)", 9, Territory);

		Req.AcceptMerchantGiftCertificates = true;
		Req.AcceptMerchantCoupons = true;
		Req.MerchantCalculationsUrl = "http://ecom.ccsols.com/ngs/mo/callback/callback.aspx";
		Req.AddMerchantCalculatedShippingMethod("UPS Ground", 8.30m);
		Req.AddMerchantCalculatedShippingMethod("SuperShip", 2.05m);
		Req.AddStateTaxRule("CA", 0.0875, true);
		Req.MerchantCalculatedTax = true;

		GCheckoutResponse Resp = Req.Send();
		Response.Redirect(Resp.RedirectUrl, true);

		//byte[] xml = Req.GetXml();
		//for (int i = 0; i < xml.Length; i++) {
	    //  	Response.Write(xml[i]);
	    //  	Response.Write(" ");
		//}
      	//Response.Write(EncodeHelper.Utf8BytesToString(Req.GetXml()));
*/
	}
	
</script>
<html>
	<head>
		<title>Shopping Cart</title>
	</head>

	<body>
		This page demonstrates how to transfer a user with a shopping
		cart to Google.
		<p>
		<form id="Form1" method="post" runat="server">
			<cc1:GCheckoutButton id="GCheckoutButton1" onclick="PostCartToGoogle" runat="server" />
		</form>
	</body>
</html>
