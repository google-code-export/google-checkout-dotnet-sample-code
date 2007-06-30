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
  /// The &lt;digital-content&gt; tag contains information relating to
  /// digital delivery of an item.
  /// </summary>
  public class DigitalItem {
    private const string MISSING_KEY_OR_URL = @"You must provide either a URL
where the buyer can access the digital content or a key needed to download 
or unlock the content.";

    private const string DESCRIPTION_LONG = @"This field has a maximum length
of 1024 characters and may contain HTML tags.";

    private string _key = string.Empty;
    private string _url = string.Empty;
    private bool _emaildelivery;
    private string _description = string.Empty;

    /// <summary>
    /// The &lt;key&gt; tag contains a key needed to download or 
    /// unlock a digital content item.
    /// </summary>
    public string Key {
      get {
        return _key;
      }
    }
    
    /// <summary>
    /// The &lt;url&gt; tag contains a URI from which the customer 
    /// can download the purchased content.
    /// </summary>
    public string Url {
      get {
        return _url;
      }
    }

    /// <summary>
    /// The &lt;email-delivery&gt; tag indicates that the merchant will 
    /// send emailto the buyer explaining how to access the digital
    /// content. Email delivery allows the merchant to charge the buyer
    /// for an order before allowing the buyer to access the digital
    /// content.
    /// </summary>
    public bool EmailDelivery {
      get {
        return _emaildelivery;
      }
    }
    
    /// <summary>
    /// The &lt;description&gt; tag contains instructions for downloading a digital 
    /// content item. Please use the &lt;item-description&gt; tag to provide
    /// the description of the item being purchased.
    /// </summary>
    /// <remarks>Note: This field has a maximum length of 1024 characters and 
    /// may contain HTML tags. (HTML tags must be XML-escaped as described 
    /// in the Google Checkout Developer's Guide.)
    ///</remarks>
    public string Description {
      get {
        return _description;
      }
    }

    /// <summary>
    /// Create a new instance of the &lt;digital-content&gt;
    /// for an &lt;email-delivery&gt;
    /// </summary>
    /// <remarks>No other tags can be set for &lt;email-delivery&gt;</remarks>
    public DigitalItem() {
      _emaildelivery = true;
    }

    /// <summary>
    /// Create a new instance of a Url/Key &lt;digital-content&gt; object
    /// </summary>
    /// <param name="key">
    /// The &lt;key&gt; tag contains a key needed to download or 
    /// unlock a digital content item.
    /// </param>
    /// <remarks></remarks>
    public DigitalItem(string key) {
      key = GCheckout.Util.EncodeHelper.CleanString(key);
      if (key.Length == 0)
        throw new ArgumentNullException(MISSING_KEY_OR_URL);
      _key = key;
    }

    /// <summary>
    /// Create a new instance of a Url/Key &lt;digital-content&gt; object
    /// </summary>
    /// <param name="key">
    /// The &lt;key&gt; tag contains a key needed to download or 
    /// unlock a digital content item.
    /// </param>
    /// <param name="description">
    /// The &lt;description&gt; tag contains instructions for downloading a digital 
    /// content item. Please use the &lt;item-description&gt; tag to provide
    /// the description of the item being purchased.
    /// </param>
    /// <remarks></remarks>
    public DigitalItem(string key, string description) {
      key = EncodeHelper.CleanString(key);
      if (key.Length == 0)
        throw new ArgumentNullException(MISSING_KEY_OR_URL);
      _key = key;

      SetDescription(description);
    }

    /// <summary>
    /// Create a new instance of a Url/Key &lt;digital-content&gt; object
    /// </summary>
    /// <param name="key">
    /// The &lt;key&gt; tag contains a key needed to download or 
    /// unlock a digital content item.
    /// </param>
    /// <param name="url">
    /// The &lt;url&gt; tag contains a URI from which the customer 
    /// can download the purchased content.
    /// </param>
    /// <param name="description">
    /// The &lt;description&gt; tag contains instructions for downloading a digital 
    /// content item. Please use the &lt;item-description&gt; tag to provide
    /// the description of the item being purchased.
    /// </param>
    /// <remarks>Either the Key or the Url is required.
    /// You may pass in null or String.Empty into one of the two parameters.</remarks>
    public DigitalItem(string key, string url, string description) {
      key = EncodeHelper.CleanString(key);
      url = EncodeHelper.CleanString(url);

      //either can be set, we don't care which one.
      if (key.Length == 0 && url.Length == 0)
        throw new ArgumentNullException(MISSING_KEY_OR_URL);

      _key = key;
      _url = url;

      SetDescription(description);
    }

    /// <summary>
    /// Create a new instance of a Url/Key &lt;digital-content&gt; object
    /// </summary>
    /// <param name="url">
    /// The &lt;url&gt; tag contains a URI from which the customer 
    /// can download the purchased content.
    /// </param>
    /// <remarks></remarks>
    public DigitalItem(Uri url) {
      if (url == null)
        throw new ArgumentNullException(MISSING_KEY_OR_URL);
      _url = url.ToString();
    }

    /// <summary>
    /// Create a new instance of a Url/Key &lt;digital-content&gt; object
    /// </summary>
    /// <param name="url">
    /// The &lt;url&gt; tag contains a URI from which the customer 
    /// can download the purchased content.
    /// </param>
    /// <param name="description">
    /// The &lt;description&gt; tag contains instructions for downloading a digital 
    /// content item. Please use the &lt;item-description&gt; tag to provide
    /// the description of the item being purchased.
    /// </param>
    /// <remarks></remarks>
    public DigitalItem(Uri url, string description) {
      if (url == null)
        throw new ArgumentNullException(MISSING_KEY_OR_URL);
      _url = url.ToString();

      SetDescription(description);
    }

    /// <summary>
    /// helper method to validate the description
    /// </summary>
    /// <param name="description">The description to set.</param>
    private void SetDescription(string description) {
      description = EncodeHelper.CleanString(description);
      if (description.Length > 0)
        description = EncodeHelper.EscapeXmlChars(description);

      if (description.Length > 1024)
        throw new ArgumentException(DESCRIPTION_LONG, "description");

      _description = description;
      //The value is not required so we are not going to throw an exception
      //if it is not set.
    }

  }
}
