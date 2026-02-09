using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using System;
using System.IO;
using System.Net.Http;

namespace altv_spoofer
{
    internal class Settings
    {
        public static string logo = @"       .__   __                                        _____             
_____  |  |_/  |____  __   ____________   ____   _____/ ____\___________ 
\__  \ |  |\   __\  \/ /  /  ___/\____ \ /  _ \ /  _ \   __\/ __ \_  __ \
 / __ \|  |_|  |  \   /   \___ \ |  |_> >  <_> |  <_> )  | \  ___/|  | \/
(____  /____/__|   \_/   /____  >|   __/ \____/ \____/|__|  \___  >__|   
     \/                       \/ |__|                           \/       ";

        static string pathLogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs.txt");
        public enum logType { 
            Error,
            Warning,
            Success
        }

        public static void createLog(logType type, string message)
        {

            string titleInfo = type.ToString().ToUpper();


            string logEntry = $"[{DateTime.Now}] [{titleInfo}] {message}{Environment.NewLine}";

            File.AppendAllText(pathLogFile, logEntry);
        }


        public static void saveOldHWID() { 
        
        }

        public static void CheckAndInitLogs()
        {
            if (!File.Exists(pathLogFile))
            {
                try
                {
                    createLog(logType.Success, "Log file initialized.");

                }
                catch (Exception)
                {
                    Console.WriteLine("Unable to create log file, program will not start.");
                    throw;
                }
            }
        }


    }
}