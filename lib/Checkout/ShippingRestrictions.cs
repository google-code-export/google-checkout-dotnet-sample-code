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

namespace GCheckout.Checkout {
  public class ShippingRestrictions {
    private AutoGen.ShippingRestrictions _Restrictions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShippingRestrictions"/> 
    /// class.
    /// </summary>
    public ShippingRestrictions() {
      _Restrictions = new AutoGen.ShippingRestrictions();
      _Restrictions.allowedareas = 
        new AutoGen.ShippingRestrictionsAllowedareas();
      _Restrictions.allowedareas.Items = new object[0];
      _Restrictions.excludedareas = 
        new AutoGen.ShippingRestrictionsExcludedareas();
      _Restrictions.excludedareas.Items = new object[0];
    }

    /// <summary>
    /// This method adds an allowed zip code pattern to a 
    /// &lt;us-zip-area&gt; element. The &lt;us-zip-area&gt; element, 
    /// in turn, appears as a subelement of &lt;allowed-areas&gt;.
    /// </summary>
    /// <param name="ZipPattern">The zip pattern.</param>
    public void AddAllowedZipPattern(string ZipPattern) {
      AutoGen.USZipArea NewArea = new AutoGen.USZipArea();
      NewArea.zippattern = ZipPattern;
      AddNewAllowedArea(NewArea);
    }

    /// <summary>
    /// This method adds an allowed U.S. state code to a 
    /// &lt;us-state-area&gt; element. The &lt;us-state-area&gt; element, 
    /// in turn, appears as a subelement of &lt;allowed-areas&gt;.
    /// </summary>
    /// <param name="StateCode">The state code.</param>
    public void AddAllowedStateCode(string StateCode) {
      AutoGen.USStateArea NewArea = new AutoGen.USStateArea();
      NewArea.state = StateCode;
      AddNewAllowedArea(NewArea);
    }

    /// <summary>
    /// This method adds an allowed U.S. country area to a 
    /// &lt;us-country-area&gt; element. The &lt;us-country-area&gt; element, 
    /// in turn, appears as a subelement of &lt;allowed-areas&gt;.
    /// </summary>
    /// <param name="CountryArea">The country area.</param>
    public void AddAllowedCountryArea(AutoGen.USAreas CountryArea) {
      AutoGen.USCountryArea NewArea = new AutoGen.USCountryArea();
      NewArea.countryarea = CountryArea;
      AddNewAllowedArea(NewArea);
    }

    /// <summary>
    /// This method adds an allowed zip code area, state area or country area 
    /// to a Checkout API request.
    /// </summary>
    /// <param name="NewArea">The new area.</param>
    private void AddNewAllowedArea(object NewArea) {
      object[] NewAllowedAreas = 
        new object[_Restrictions.allowedareas.Items.Length + 1];
      for (int i = 0; i < _Restrictions.allowedareas.Items.Length; i++) {
        NewAllowedAreas[i] = _Restrictions.allowedareas.Items[i];
      }
      NewAllowedAreas[NewAllowedAreas.Length - 1] = NewArea;
      _Restrictions.allowedareas.Items = NewAllowedAreas;
    }

    /// <summary>
    /// This method adds an excluded zip code pattern to a 
    /// &lt;us-zip-area&gt; element. The &lt;us-zip-area&gt; element, 
    /// in turn, appears as a subelement of &lt;excluded-areas&gt;.
    /// </summary>
    /// <param name="ZipPattern">The zip pattern.</param>
    public void AddExcludedZipPattern(string ZipPattern) {
      AutoGen.USZipArea NewArea = new AutoGen.USZipArea();
      NewArea.zippattern = ZipPattern;
      AddNewExcludedArea(NewArea);
    }

    /// <summary>
    /// This method adds an excluded U.S. state code to a 
    /// &lt;us-state-area&gt; element. The &lt;us-state-area&gt; element, 
    /// in turn, appears as a subelement of &lt;excluded-areas&gt;.
    /// </summary>
    /// <param name="StateCode">The state code.</param>
    public void AddExcludedStateCode(string StateCode) {
      AutoGen.USStateArea NewArea = new AutoGen.USStateArea();
      NewArea.state = StateCode;
      AddNewExcludedArea(NewArea);
    }

    /// <summary>
    /// This method adds an excluded U.S. country area to a 
    /// &lt;us-country-area&gt; element. The &lt;us-country-area&gt; element, 
    /// in turn, appears as a subelement of &lt;excluded-areas&gt;.
    /// </summary>
    /// <param name="CountryArea">The country area.</param>
    public void AddExcludedCountryArea(AutoGen.USAreas CountryArea) {
      AutoGen.USCountryArea NewArea = new AutoGen.USCountryArea();
      NewArea.countryarea = CountryArea;
      AddNewExcludedArea(NewArea);
    }

    /// <summary>
    /// This method adds an excluded zip code area, state area or country area 
    /// to a Checkout API request.
    /// </summary>
    /// <param name="NewArea">The new area.</param>
    private void AddNewExcludedArea(object NewArea) {
      object[] NewExcludedAreas = 
        new object[_Restrictions.excludedareas.Items.Length + 1];
      for (int i = 0; i < _Restrictions.excludedareas.Items.Length; i++) {
        NewExcludedAreas[i] = _Restrictions.excludedareas.Items[i];
      }
      NewExcludedAreas[NewExcludedAreas.Length - 1] = NewArea;
      _Restrictions.excludedareas.Items = NewExcludedAreas;
    }

    /// <summary>
    /// This method returns the complete set of shipping restrictions for 
    /// a Checkout API request.
    /// </summary>
    /// <value>The XML restrictions.</value>
    public AutoGen.ShippingRestrictions XmlRestrictions {
      get {
        return _Restrictions;
      }
    }

  }
}
