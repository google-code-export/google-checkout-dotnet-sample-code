/*************************************************
 * Copyright (C) 2006 Google Inc.
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

namespace GCheckout.MerchantCalculation {
  /// <summary>
  /// This class contains a sample class that inherits from CallbackRules.cs. 
  /// This class demonstrates how you could subclass CallbackRules.cs to 
  /// define your own merchant-calculated shipping, tax and coupon options.
  /// </summary>
  public class ExampleRules : CallbackRules {
    private const string SAVE10 = "SAVE10";
    private const string SAVE20 = "SAVE20";
    private const string GIFTCERT = "GIFTCERT";

    public override MerchantCodeResult GetMerchantCodeResult(Order ThisOrder, 
      AnonymousAddress Address, string MerchantCode) {
      MerchantCodeResult RetVal = new MerchantCodeResult();
      if (MerchantCode.ToUpper() == SAVE10) {
        RetVal.Amount = 10;
        RetVal.Type = MerchantCodeType.Coupon;
        RetVal.Valid = true;
        RetVal.Message = "You saved $10!";
      }
      else if (MerchantCode.ToUpper() == SAVE20) {
        RetVal.Amount = 20;
        RetVal.Type = MerchantCodeType.Coupon;
        RetVal.Valid = true;
        RetVal.Message = "You saved $20!";
      }
      else if (MerchantCode.ToUpper() == GIFTCERT) {
        RetVal.Amount = 23.46m;
        RetVal.Type = MerchantCodeType.GiftCertificate;
        RetVal.Valid = true;
        RetVal.Message = "Your gift certificate has a balance of $23.46.";
      }
      else {
        RetVal.Message = "Sorry, we didn't recognize code '" + MerchantCode + 
          "'.";
      }
      return RetVal;
    }

    public override decimal GetTaxResult(Order ThisOrder, 
      AnonymousAddress Address, decimal ShippingRate) {
      decimal RetVal = 0;
      if (Address.Region == "HI") {
        decimal Total = 0;
        foreach (OrderLine Line in ThisOrder) {
          Total += Line.UnitPrice * Line.Quantity;
        }
        RetVal = decimal.Round(Total * 0.09m, 2);
      }
      return RetVal;
    }

    public override ShippingResult GetShippingResult(string ShipMethodName, 
      Order ThisOrder, AnonymousAddress Address) {
      ShippingResult RetVal = new ShippingResult();
      if (ShipMethodName == "UPS Ground" && Address.Region != "HI" && 
        Address.Region != "AL") {
        RetVal.Shippable = true;
        RetVal.ShippingRate = 20;
      }
      if (ShipMethodName == "SuperShip") {
        RetVal.Shippable = true;
        RetVal.ShippingRate = 0;
      }
      return RetVal;
    }

  }
}
