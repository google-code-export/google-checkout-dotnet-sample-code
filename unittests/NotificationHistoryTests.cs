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
//using GCheckout.AutoGen;

namespace GoogleCheckoutUnitTests {

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
        "GoogleCheckoutUnitTests.Xml.NotificationHistoryResponse1.xml")) {

        StreamReader sr = new StreamReader(s);
        _response_regular_string = sr.ReadToEnd();
        s.Position = 0;

        _response_regular 
          = GCheckout.Util.EncodeHelper.Deserialize(s) 
          as GCheckout.AutoGen.NotificationHistoryResponse;
      }
    }

    [Test]
    public void Verify_Regular_Response_Works() {

      NotificationHistoryResponse test 
        = new NotificationHistoryResponse(_response_regular_string);
    }

  }
}