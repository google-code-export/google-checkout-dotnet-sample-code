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
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace GCheckout.Util {
  /// <summary>
  /// Collection of various encode-related static methods.
  /// </summary>
  public class EncodeHelper
  {
    private EncodeHelper()
    {}

	  /// <summary>
	  /// Converts a string to bytes in UTF-8 encoding.
	  /// </summary>
	  /// <param name="InString">The string to convert.</param>
	  /// <returns>The UTF-8 bytes.</returns>
    public static byte[] StringToUtf8Bytes(string InString) {
      UTF8Encoding utf8encoder = new UTF8Encoding(false, true);
      return utf8encoder.GetBytes(InString);
    }

	  /// <summary>
	  /// Converts bytes in UTF-8 encoding to a regular string.
	  /// </summary>
	  /// <param name="InBytes">The UTF-8 bytes.</param>
	  /// <returns>The input bytes as a string.</returns>
    public static string Utf8BytesToString(byte[] InBytes) {
      UTF8Encoding utf8encoder = new UTF8Encoding(false, true);
      return utf8encoder.GetString(InBytes);
    }

    /// <summary>
    /// Converts UTF8-encoded bytes from a stream to a string.
    /// </summary>
    /// <param name="Utf8Stream">The UTF8 stream.</param>
    /// <returns>
    /// The full stream contents as a string. Also closes the stream.
    /// </returns>
    public static string Utf8StreamToString(Stream Utf8Stream) {
      StreamReader SReader = new StreamReader(Utf8Stream, Encoding.UTF8);
      string RetVal = SReader.ReadToEnd();
      SReader.Close();
      return RetVal;
    }

    /// <summary>
	  /// Gets the top element of an XML string.
	  /// </summary>
	  /// <param name="Xml">
	  /// The XML string to extract the top element from.
	  /// </param>
	  /// <returns>
	  /// The name of the first regular XML element.
	  /// </returns>
	  /// <example>
	  /// Calling GetTopElement(Xml) where Xml is:
	  /// <code>
	  /// &lt;?xml version="1.0" encoding="UTF-8"?&gt;
	  /// &lt;new-order-notification xmlns="http://checkout.google.com/schema/2"
	  /// serial-number="85f54628-538a-44fc-8605-ae62364f6c71"&gt;
	  /// &lt;google-order-number&gt;841171949013218&lt;/google-order-number&gt;
	  /// ...
	  /// &lt;/new-order-notification&gt;
	  /// </code>
	  /// will return the string <b>new-order-notification</b>.
	  /// </example>
	  public static string GetTopElement(string Xml) {
      StringReader SReader = new StringReader(Xml);
      XmlTextReader XReader = new XmlTextReader(SReader);
      XReader.WhitespaceHandling = WhitespaceHandling.None;
      XReader.Read();
      XReader.Read();
      string RetVal = XReader.Name;
      XReader.Close();
      SReader.Close();
      return RetVal;
    }

    /// <summary>
    /// Gets the value of the first element or attribute in a piece of XML.
    /// </summary>
    /// <param name="Xml">
    /// The XML to extract the element or attribute value from.
    /// </param>
    /// <param name="Element">
    /// Name of the element or attribute to search for.
    /// </param>
    /// <returns>
    /// The value of the first element or attribute. If there is no such
    /// element or attribute in the XML, an empty string is returned.
    /// </returns>
    /// <example>
    /// Assume the string variable Xml has this value:
    /// <code>
    /// &lt;?xml version="1.0" encoding="UTF-8"?&gt;
    /// &lt;new-order-notification xmlns="http://checkout.google.com/schema/2"
    /// serial-number="85f54628-538a-44fc-8605-ae62364f6c71"&gt;
    /// &lt;google-order-number&gt;841171949013218&lt;/google-order-number&gt;
    /// ...
    /// &lt;/new-order-notification&gt;
    /// </code>
    /// Calling <b>GetElementValue(Xml, "google-order-number")</b> will return 
    /// <b>841171949013218</b>. Calling 
    /// <b>GetElementValue(Xml, "serial-number")</b> will return 
    /// <b>85f54628-538a-44fc-8605-ae62364f6c71</b>.
    /// </example>
    public static string GetElementValue(string Xml, string Element) {
      string RetVal = "";
      StringReader SReader = new StringReader(Xml);
      XmlTextReader XReader = new XmlTextReader(SReader);
      XReader.WhitespaceHandling = WhitespaceHandling.None;
      XReader.Read();
      while (!XReader.EOF ) {
        if (XReader.Name == Element) {
          XReader.Read();
          RetVal = XReader.Value;
          break;
        }
        if (XReader.MoveToAttribute(Element)) {
          RetVal = XReader.Value;
          break;
        }
        XReader.Read();
      }
      XReader.Close();
      return RetVal;
    }

    /// <summary>
    /// Makes an object out of an XML string.
    /// </summary>
    /// <param name="Xml">The XML that should be made into an object.</param>
    /// <param name="ThisType">
    /// Type of object that produced the XML. 
    /// </param>
    /// <returns>The reconstituted object.</returns>
    /// <example>
    /// <code>
    /// Car MyCar1 = new Car();
    /// byte[] CarBytes = EncodeHelper.Serialize(MyCar1);
    /// string CarXml = EncodeHelper.Utf8BytesToString(CarBytes);
    /// Car MyCar2 = (Car) Deserialize(CarXml, typeof(Car));
    /// // MyCar2 is now a copy of MyCar1.
    /// </code>
    /// </example>
    public static object Deserialize(string Xml, Type ThisType) {
      XmlSerializer myDeserializer = new XmlSerializer(ThisType);
      StringReader myReader = new StringReader(Xml);
      object RetVal = myDeserializer.Deserialize(myReader);
      myReader.Close();
      return RetVal;
    }

    /// <summary>
    /// Makes XML out of an object.
    /// </summary>
    /// <param name="ObjectToSerialize">The object to serialize.</param>
    /// <returns>An XML string representing the object.</returns>
    /// <example>
    /// <code>
    /// Car MyCar1 = new Car();
    /// byte[] CarBytes = EncodeHelper.Serialize(MyCar1);
    /// string CarXml = EncodeHelper.Utf8BytesToString(CarBytes);
    /// Car MyCar2 = (Car) Deserialize(CarXml, typeof(Car));
    /// // MyCar2 is now a copy of MyCar1.
    /// </code>
    /// </example>
    public static byte[] Serialize(object ObjectToSerialize) 
    {
      XmlSerializer Ser = new XmlSerializer(ObjectToSerialize.GetType());
      MemoryStream MS = new MemoryStream();
      XmlTextWriter W = new XmlTextWriter(MS, new UTF8Encoding(false));
      W.Formatting = Formatting.Indented;
      Ser.Serialize(W, ObjectToSerialize);
      W.Flush();
      return MS.ToArray();
    }

    /// <summary>
    /// Escapes XML characters &lt; &gt; and &amp;.
    /// </summary>
    /// <param name="In">
    /// String that could contain &lt; &gt; and &amp; characters.
    /// </param>
    /// <returns>
    /// A new string where 
    /// <b>&amp;</b> has been replaced by <b>&amp;#x26;</b>,
    /// <b>&lt;</b> has been replaced by <b>&amp;#x3c;</b> and
    /// <b>&gt;</b> has been replaced by <b>&amp;#x3e;</b>.
    /// These replacements are mandated here in the Dev Guide:
    /// <a href="http://code.google.com/apis/checkout/developer/index.html#api_request_guidelines">
    /// http://code.google.com/apis/checkout/developer/index.html#api_request_guidelines
    /// </a>
    /// </returns>
    public static string EscapeXmlChars(string In) {
      string RetVal = In;
      RetVal = RetVal.Replace("&", "&#x26;");
      RetVal = RetVal.Replace("<", "&#x3c;");
      RetVal = RetVal.Replace(">", "&#x3e;");
      return RetVal;
    }

  }
}
