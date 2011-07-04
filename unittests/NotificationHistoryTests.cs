/*************************************************
 * Copyright (C) 2008 Google Inc.
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
/*
 Edit History:
 *  9-7-2008   Joe Feser joe.feser@joefeser.com
 *  Initial Release.
 * 
*/
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using System.IO;
using GCheckout.OrderProcessing;
using GCheckout.Util;
using System.Diagnostics;
//using GCheckout.AutoGen;

namespace GCheckout.Checkout.Tests {

  [TestFixture]
  public class NotificationHistoryTests {

    private static GCheckout.AutoGen.NotificationHistoryResponse _response_regular;
    private static string _response_regular_string = null;

    /// <summary>
    /// Initialize the Xml Documents
    /// </summary>
    static NotificationHistoryTests() {
      using (Stream s = Assembly.GetExecutingAssembly()
        .GetManifestResourceStream(
        "GCheckout.Checkout.Tests.Xml.NotificationHistoryResponse1.xml")) {

        StreamReader sr = new StreamReader(s);
        _response_regular_string = sr.ReadToEnd();
        s.Position = 0;

        _response_regular
          = GCheckout.Util.EncodeHelper.Deserialize(s)
          as GCheckout.AutoGen.NotificationHistoryResponse;
      }
    }

    [Test]
    public void Verify_Correct_Message_Returned_When_Incorrect_Message_Format() {
      var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                  <charge-amount-notification xmlns=""http://checkout.google.com/schema/2"" serial-number=""95d44287-12b1-4722-bc56-cfaa73f4c0d1"">
                    <google-order-number>841171949013218</google-order-number>
                    <latest-charge-amount currency=""USD"">226.06</latest-charge-amount>
                    <total-charge-amount currency=""USD"">226.06</total-charge-amount>
                    <timestamp>2007-03-19T15:12:26.051Z</timestamp>
                  </charge-amount-notification>";

      var test
        = new NotificationHistoryResponse(xml);

      Assert.AreEqual(false, test.IsGood);
      Assert.AreEqual("Couldn't parse ResponseXml. Message type found was ChargeAmountNotification. Please See ResponseXml", test.ErrorMessage);
    }

    [Test]
    public void Verify_Serial_Number_Supported() {

      var sn = "123456";
      var request = new NotificationHistoryRequest(new NotificationHistorySerialNumber(sn));

      var roundTrip = EncodeHelper.Deserialize(EncodeHelper.Utf8BytesToString(request.GetXml()),
        typeof(GCheckout.AutoGen.NotificationHistoryRequest)) as GCheckout.AutoGen.NotificationHistoryRequest;

      Assert.AreEqual(sn, roundTrip.serialnumber);
    }

    [Test]
    public void Verify_Regular_Response_Works() {

      NotificationHistoryResponse test
        = new NotificationHistoryResponse(_response_regular_string);
    }

  }
}
