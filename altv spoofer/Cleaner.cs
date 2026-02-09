using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace altv_spoofer
{
    internal class Cleaner
    {
        private static string directory = $@"C:\Users\{Environment.UserName}\AppData\Local\altv\cache";
        private static string secondDirectory = $@"C:\Users\{Environment.UserName}\AppData\Local\altv\cef\cache";

        public static void RunCleaner()
        {
            Program.blockInput = true; 

            Console.WriteLine("Clearing altV cache...");
            CleanPath(directory);

            Console.WriteLine("Clearing CEF cache...");
            CleanPath(secondDirectory);

            Console.WriteLine("Cleaning completed.");
            Thread.Sleep(2000);
            Console.Clear();
            Program.printMenu();
            Program.blockInput = false; 
        }

        private static void CleanPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Not found: {path}");
                return;
            }

            try
            {
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                        Console.WriteLine($"File deleted: {Path.GetFileName(file)}");
                        Settings.createLog(Settings.logType.Success, $"File deleted: {Path.GetFileName(file)}");
                    }
                    catch
                    {
                        Settings.createLog(Settings.logType.Error, $"The file is currently in use and could not be deleted (disable ALTV): {Path.GetFileName(file)}");

                        Console.WriteLine($"[ERROR] The file is currently in use and could not be deleted (disable ALTV): {Path.GetFileName(file)}");
                    }
                }

                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    try
                    {
                        Directory.Delete(dir, true);
                        Console.WriteLine($"Directory deleted: {Path.GetFileName(dir)}");
                        Settings.createLog(Settings.logType.Success, $"Directory deleted: {Path.GetFileName(dir)}");

                    }
                    catch
                    {
                        Settings.createLog(Settings.logType.Error, $"The folder is currently in use: {Path.GetFileName(dir)}");

                        Console.WriteLine($"[ERROR] The folder is currently in use: {Path.GetFileName(dir)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Settings.createLog(Settings.logType.Error, $"A general error occurred while cleaning {path}: {ex.Message}");

                Console.WriteLine($"A general error occurred while cleaning {path}: {ex.Message}");

            }
        }

    }
}