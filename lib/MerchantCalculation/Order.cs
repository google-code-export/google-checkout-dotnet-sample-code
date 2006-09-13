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
  /// This class contains methods that parse a 
  /// &lt;merchant-calculation-callback&gt; request and creates an object 
  /// identifying the items in the customer's shopping cart. Your class that 
  /// inherits from CallbackRules.cs will receive an object of this type to 
  /// identify the items in the customer's order.
  /// </summary>
  public class Order {
    private ArrayList _OrderLines;

    public Order(AutoGen.MerchantCalculationCallback Callback) {
      _OrderLines = new ArrayList();
      for (int i = 0; i < Callback.shoppingcart.items.Length; i++) {
        AutoGen.Item ThisItem = Callback.shoppingcart.items[i];
        _OrderLines.Add(
          new OrderLine(ThisItem.itemname, ThisItem.itemdescription,
          ThisItem.quantity, ThisItem.unitprice.Value, 
          ThisItem.taxtableselector));
      }
    }

    public IEnumerator GetEnumerator() {
      return _OrderLines.GetEnumerator();
    }

  }
}
