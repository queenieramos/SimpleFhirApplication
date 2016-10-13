using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppChallenge.Logging
{
    /// <summary>
    /// Logs to a text file that is saved in the User's desktop.
    /// </summary>
    public class Logger
    {
        private static string logFileLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "AppChallengeLog.txt");

        public static void WriteLogToFile(string logMessage)
        {
            using (StreamWriter sw = new StreamWriter(logFileLocation, true))
            {
                DateTime currDate = DateTime.Now;
                sw.WriteLine("{0} {1}: {2}", currDate.ToLongDateString(), currDate.ToLongTimeString(), logMessage);
            }
        }

        public static void WriteExceptionLogToFile(string operation, Exception exc)
        {
            using (StreamWriter sw = new StreamWriter(logFileLocation, true))
            {
                DateTime currDate = DateTime.Now;
                sw.WriteLine("{0} {1}: Error occurred when executing {2}", currDate.ToLongDateString(), currDate.ToLongTimeString(), operation);
                sw.WriteLine("Error Message: {0}", exc.Message);
                sw.WriteLine("Error Source: {0}", exc.Source);
                sw.WriteLine("Error Stack Trace: {0}", exc.StackTrace);
            }
        }
    }
}
