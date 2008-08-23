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
 *  August 2008   Joe Feser joe.feser@joefeser.com
 *  Initial Release
*/

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using GCheckout.OrderProcessing;

namespace GCheckout.Checkout.Tests {

  [TestFixture]
  public class PollingApiUnitTests {

    static string CONN_STRING = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=.\GoogleCheckout.mdb;";

    [Test]
    public void VerifyDatabase() {
      PollingApi api = new PollingApi("123456", "456789", EnvironmentType.Sandbox.ToString(), CONN_STRING);

      Assert.AreEqual(string.Empty, api.Token);
      //api.PollServer();
    }
  }
}
