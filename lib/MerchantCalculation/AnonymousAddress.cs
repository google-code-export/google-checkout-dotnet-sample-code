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
  /// This class creates an object that identifies the customer's shipping 
  /// address. Your class that inherits from CallbackRules.cs will receive an 
  /// object of this type that identifies the customer's shipping address.
  /// </summary>
  public class AnonymousAddress {
    private string _City;
    private string _CountryCode;
    private string _Id;
    private string _PostalCode;
    private string _Region;

    public AnonymousAddress(AutoGen.AnonymousAddress ThisAddress) {
      _City = ThisAddress.city;
      _CountryCode = ThisAddress.countrycode;
      _Id = ThisAddress.id;
      _PostalCode = ThisAddress.postalcode;
      _Region = ThisAddress.region;
    }

    public string City {
      get {
        return _City;
      }
    }

    public string CountryCode {
      get {
        return _CountryCode;
      }
    }

    public string Id {
      get {
        return _Id;
      }
    }
  
    public string PostalCode {
      get {
        return _PostalCode;
      }
    }
  
    public string Region {
      get {
        return _Region;
      }
    }

  }
}
