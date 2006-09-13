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
using System.Collections;

namespace GCheckout.MerchantCalculation {
  /// <summary>
  /// This abstract class defines methods for constructing a 
  /// &lt;merchant-calculation-results&gt; XML response.
  /// </summary>
  public abstract class CallbackRules {
    public virtual MerchantCodeResult GetMerchantCodeResult(Order ThisOrder, 
      AnonymousAddress Address, string MerchantCode) {
      return new MerchantCodeResult();
    }

    public virtual decimal GetTaxResult(Order ThisOrder, 
      AnonymousAddress Address, decimal ShippingRate) {
      return 0;
    }

    public virtual ShippingResult GetShippingResult(string ShipMethodName, 
      Order ThisOrder, AnonymousAddress Address) {
      return new ShippingResult();
    }
  }

}
