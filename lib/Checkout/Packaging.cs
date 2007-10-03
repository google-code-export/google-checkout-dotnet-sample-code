/*************************************************
 * Copyright (C) 2007 Google Inc.
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
using GCheckout.Util;

namespace GCheckout.Checkout {
  
  /// <summary>
  /// The &lt;packaging&gt; tag specifies the type of packaging
  /// that will be used to ship the order.
  /// </summary>
  public enum Packaging {

    /// <summary>Box</summary>
    Box, 
    /// <summary>Carrier Box</summary>
    Carrier_Box, 
    /// <summary>Carrier Pak</summary>
    Carrier_Pak, 
    /// <summary>Carrier Tube</summary>
    Carrier_Tube, 
    /// <summary>Carrier Envelope</summary>
    Carrier_Envelope, 
    /// <summary>Card</summary>
    Card,
    /// <summary>Letter</summary>
    Letter
  }
}
