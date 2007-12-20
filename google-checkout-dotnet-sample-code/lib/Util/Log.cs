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
using System.IO;
using System.Diagnostics;
using System.Configuration;

namespace GCheckout.Util {
  /// <summary>
  /// This class contains methods that write messages, including debugging 
  /// and error information, to log files.
  /// </summary>
  public class Log {

    internal static readonly object lockObject = new object();

    /// <summary>
    /// Writes a string to a file.
    /// </summary>
    /// <param name="strFileName">Path and name of the file.</param>
    /// <param name="strLine">The line to write.</param>
    public static void WriteToFile(string strFileName, string strLine) {
      if (LoggingOn()) {
        lock (lockObject) {
          using(StreamWriter objStreamWriter = new StreamWriter(strFileName, true)) {
            // Write a line of text.
            objStreamWriter.WriteLine(strLine);
          }   
        }
      }
    }

    /// <summary>
    /// Writes a debug message to the file X\debug.txt where X is read from
    /// the config file key "LogDirectory".
    /// </summary>
    /// <param name="strLine">The debug message to write.</param>
    public static void Debug(string strLine) {
      if (LoggingOn()) {
        WriteToFile(GetLogPath() + "debug.txt", DateTime.Now + " - " + 
          strLine);
      }
    }

    /// <summary>
    /// Writes an error message to the file X\error.txt where X is read from
    /// the config file key "LogDirectory".
    /// </summary>
    /// <param name="strLine">The error message to write.</param>
    public static void Err(string strLine) {
      if (LoggingOn()) {
        WriteToFile(GetLogPath() + "error.txt", DateTime.Now + " - " + 
          strLine + (new StackTrace()).ToString());
      }
    }

    /// <summary>
    /// Returns true if logging is on, that is if the config file contains
    /// values for a key for "LogDirectory" and if the value for the key
    /// "Logging" is "true".
    /// </summary>
    /// <returns>True if the config keys are correct.</returns>
    public static bool LoggingOn() {
      bool RetVal = true;
      if ((GetLogPath() == null || GetLogPath() == string.Empty) 
        || !GCheckoutConfigurationHelper.Logging) {
        RetVal = false;
      }
      return RetVal;
    }

    /// <summary>
    /// Gets the log path, that is the value of the "LogDirectory" key in the
    /// config file.
    /// </summary>
    /// <returns>Log path.</returns>
    private static string GetLogPath() {
      return GCheckoutConfigurationHelper.LogDirectory;
    }
  }
}
