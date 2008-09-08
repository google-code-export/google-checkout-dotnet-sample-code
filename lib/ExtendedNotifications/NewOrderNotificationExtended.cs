/*************************************************
 * Copyright (C) 2006-2007 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*************************************************/
using System;
using System.Reflection;
using System.Xml.Serialization;
using GCheckout.Checkout;

namespace GCheckout.AutoGen.Extended {

  /// <summary>
  /// This is a class that is designed to extend the New order notification.
  /// </summary>
  [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://checkout.google.com/schema/2")]
  [System.Xml.Serialization.XmlRootAttribute("new-order-notification", Namespace = "http://checkout.google.com/schema/2", IsNullable = false)]
  public class NewOrderNotificationExtended
    : GCheckout.AutoGen.NewOrderNotification {

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

        return new ShoppingCartItem[] { };
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

    /// <summary>
    /// Get the merchant codes associated to the order
    /// </summary>
    public MerchantCode[] MerchantCodes {
      get {
        return MerchantCode.GetMerchantCodes(this);
      }
    }

    /// <summary>
    /// For v2.0 of the Xsd.exe app we must use properties.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    private PropertyInfo GetField(object obj, string field) {
      if (obj == null)
        return null;
      return obj.GetType().GetProperty(field,
        BindingFlags.Public | BindingFlags.Instance);
    }

    private object GetObjectField(object obj, string field) {
      //we don't care what the object is, just that it 
      //supports the field we are looking for.
      PropertyInfo pi = GetField(obj, field);
      if (pi != null)
        return pi.GetValue(obj, null);
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
