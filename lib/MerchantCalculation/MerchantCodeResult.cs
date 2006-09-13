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
  /// This class creates an object to indicate whether the coupon and gift 
  /// certificate codes that were supplied by the customer are valid. The 
  /// object also indicates whether the code is for a coupon, gift certificate 
  /// or other discount as well as the amount of the credit associated with 
  /// the code. Your class that inherits from CallbackRules.cs will access 
  /// these objects to calculate discounts associated with coupons and gift 
  /// certificates.
  /// </summary>
  public class MerchantCodeResult {
    public MerchantCodeType Type = MerchantCodeType.Undefined;
    public bool Valid = false;
    public decimal Amount = 0;
    public string Message = "";
  }

  public enum MerchantCodeType {
    Undefined = 0,
    GiftCertificate = 1,
    Coupon = 2
  }

}
