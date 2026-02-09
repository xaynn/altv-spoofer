using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace altv_spoofer
{

    internal class Program
    {
        public static bool blockInput = false;
        static Dictionary<string, Action> menuActions = new Dictionary<string, Action>
{
    { "1", Spoofer.runSpoofer },   
    { "2", Spoofer.runSpooferSocial},
    {"3", Cleaner.RunCleaner},
    { "4", () => Environment.Exit(0) }
};
   
        public static void printMenu()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("1: - Permanent Spoof");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("2: - Temp Spoof socialID, socialName (only works on servers without cloud auth)");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("3: - Cleaner");

           // Console.ForegroundColor = ConsoleColor.Blue;
          //  Console.WriteLine("4: - Save old HWID Registry");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("4: - Exit");

            Console.ResetColor();
        }
        static void Main(string[] args)
        {
            Settings.CheckAndInitLogs();
            ConsoleKeyInfo key;
            Console.TreatControlCAsInput = true;
            Console.WriteLine(Settings.logo);
            printMenu();
            Console.CursorVisible = false;
            while (true)
                if (!blockInput)
            {
                key = Console.ReadKey(true);
                string option = key.KeyChar.ToString();
                if (menuActions.ContainsKey(option))
                {
                    menuActions[option]();
                }
            }
        }
    }
}
