using System;

namespace GCheckout.Checkout {
  /// <summary>
  /// A Merchant Code Type
  /// </summary>
  public enum MerchantCodeType {

    /// <summary>
    /// Unknown
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// The &lt;gift-certificate-adjustment&gt; tag contains information
    /// about a gift certificate that was applied to an order total.
    /// </summary>
    GiftCertificate,
    
    /// <summary>
    /// The &lt;coupon-adjustment&gt; tag contains information about a
    /// coupon that was applied to an order total.
    /// </summary>
    Coupon
  }
}
