using System;
using System.Reflection;
using System.Xml.Serialization;
using GCheckout.Checkout;

namespace GCheckout.AutoGen.Extended {
  
  /// <summary>
  /// This is a class that is designed to extend the New order notification.
  /// </summary>
  [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://checkout.google.com/schema/2")]
  [System.Xml.Serialization.XmlRootAttribute("new-order-notification", Namespace="http://checkout.google.com/schema/2", IsNullable=false)]
  public class NewOrderNotificationExtended : GCheckout.AutoGen.NewOrderNotification {
    
    /// <summary>
    /// Create a new instance of the extended NewOrderNotification
    /// </summary>
    public NewOrderNotificationExtended() {

    }
    
    /// <summary>
    /// Return a readonly wrapped list of the shopping cart items.
    /// </summary>
    public ShoppingCartItem[] Items {
      get {
        if (this.shoppingcart != null && this.shoppingcart.items != null) {
          ShoppingCartItem[] retVal 
            = new ShoppingCartItem[this.shoppingcart.items.Length];

          for (int i = 0; i < retVal.Length; i++) {
            retVal[i] = new ShoppingCartItem(this.shoppingcart.items[i]);   
          }

          return retVal;
        }
      
        return new ShoppingCartItem[] {};
      }
    }

    /// <summary>
    /// Get the Shipping Method.
    /// </summary>
    public string ShippingMethod {
      get {
        string retVal = string.Empty;

        object item = GetShippingItem();

        if (item != null) {

          if (GetField(item, "shippingname") != null) {
            //it exists
            object objVal = GetObjectField(item, "shippingname");

            if (objVal != null)
              return objVal.ToString();
          }
          else {
            throw new ApplicationException(
              string.Format("The Shipping Type is not supported {0}.",
              orderadjustment.shipping.Item.GetType().ToString()));
          }
        }
        return string.Empty;
      }
    }

    /// <summary>
    /// Get the Shipping Cost.
    /// </summary>
    public decimal ShippingCost {
      get {
        object item = GetShippingItem();

        if (item != null) {

          if (GetField(item, "shippingcost") != null) {
            //it exists
            object objVal = GetObjectField(item, "shippingcost");

            if (objVal != null) {
              objVal = GetObjectField(objVal, "Value");
              if (objVal != null) {
                return Decimal.Parse(objVal.ToString());
              }
            }
          }
          else {
            throw new ApplicationException(
              string.Format("The Shipping Type is not supported {0}.",
              orderadjustment.shipping.Item.GetType().ToString()));
          }
        }
        return -1;
      }
    }

    private FieldInfo GetField(object obj, string field) {
      if (obj == null)
        return null;
      return obj.GetType().GetField(field, BindingFlags.Public | BindingFlags.Instance);
    }

    private object GetObjectField(object obj, string field) {
      //we don't care what the object is, just that it supports shipping name.
      FieldInfo pi = GetField(obj, field);
      if (pi != null)
        return pi.GetValue(obj);
      else
        return null;
    }

    private object GetShippingItem() {
      if (orderadjustment != null) {
        if (orderadjustment.shipping != null) {
          if (orderadjustment.shipping.Item != null) {
            return orderadjustment.shipping.Item;
          }
        }
      }
      return null;
    }
  }
}
