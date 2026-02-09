using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace altv_spoofer
{
    class RegistryTask
    {
        public string FullPath { get; set; }
        public string ValueName { get; set; }
        public object NewValue { get; set; }
    }

    internal class Spoofer
    {
        private static bool loaded = false;
        //"altv-client.dll"0x3AD85A8

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++) stringChars[i] = chars[random.Next(chars.Length)];
            return new string(stringChars);
        }

        public static void runSpooferSocial()
        {
            MemoryEditor mem = new MemoryEditor();
            loaded = false;
            Random rnd = new Random();

            string targetNick = RandomString(rnd.Next(6, 11));  
            int targetID = rnd.Next(100000000, 1000000000);      

            string signature = "73 ? 6f 75 ? 6c 75"; 
            long staticOffset = 0x3AD85A8;          

            while (!loaded)
            {
                if (mem.Attach("GTA5"))
                {
                    IntPtr moduleBase = mem.GetModuleBaseAddress("altv-client.dll");

                    if (moduleBase != IntPtr.Zero)
                    {
                        long nameAddress = 0;

                        IntPtr foundAddress = mem.ScanPattern("altv-client.dll", signature);

                        if (foundAddress != IntPtr.Zero)
                        {
                            nameAddress = foundAddress.ToInt64();
                            Console.WriteLine($"[SCAN] Found adress by signature: {nameAddress:X}");
                        }
                        else
                        {
                            Console.WriteLine($"[SCAN] Spoofer doesn't work, signature not found, enable spoof before connecting to server");
                            break;
                        }


                        long idAddress = nameAddress - 0x28;

                        string currentName = mem.ReadString(nameAddress, 30, isUnicode: false);

                        if (!string.IsNullOrEmpty(currentName))
                        {
                            if (currentName == targetNick)
                            {
                                Settings.createLog(Settings.logType.Success, "Spoofer already active.");
                                loaded = true;
                            }
                            else
                            {
                                Console.WriteLine($"[DETECTED] Current Name: {currentName}. Changing...");


                                mem.WriteInt(idAddress, targetID);
                                mem.WriteString(nameAddress, targetNick, isUnicode: false);

                                Settings.createLog(Settings.logType.Success, $"Spoofed! actual nickname: {targetNick}, ID: {targetID}");
                                Console.WriteLine($"[SPOOFED] Name: {targetNick} | ID: {targetID}");

                                loaded = true;

                                
                               Program.blockInput = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Waiting for altV...");
                            Thread.Sleep(2000);
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    Settings.createLog(Settings.logType.Warning, "Waiting for GTA5 Process...");
                    Thread.Sleep(2000);
                }
            }
        }

        private static string RandomNumber()
        {

            Random rnd = new Random();
            string CurrentVersion = $"{rnd.Next(6, 11)}.{rnd.Next(0, 4)}";
            return CurrentVersion;
        }

        private static byte[] GenerateRandomBytes(int length)
        {
            byte[] randomBytes = new byte[length];
            new Random().NextBytes(randomBytes);
            return randomBytes;
        }
        private static (string, string, string) GenerateBuildLabs()
        {
            Random rnd = new Random();


            int buildNum = rnd.Next(19045, 26200);
            string buildNumStr = buildNum.ToString(); 

    
            DateTime randomDate = DateTime.Now.AddDays(-rnd.Next(5, 365));
            string dateStr = randomDate.ToString("yyMMdd");
            string timeStr = rnd.Next(0, 24).ToString("00") + rnd.Next(0, 60).ToString("00");

            string lab = $"{buildNumStr}.ge_release.{dateStr}-{timeStr}";
            string labEx = $"{buildNumStr}.1.amd64fre.ge_release.{dateStr}-{timeStr}";

            return (lab, labEx, buildNumStr);
        }

        private static int GenerateInstallDate()
        {
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();


            Random rnd = new Random();
            long randomPast = now - rnd.Next(86400, 63000000);

            return (int)randomPast;
        }

        private static string GenerateProductId()
        {
            Random rnd = new Random();

            string part1 = rnd.Next(0, 99999).ToString("D5");

            string part2 = rnd.Next(0, 99999).ToString("D5");

            string part3 = rnd.Next(0, 99999).ToString("D5");


            string part4 = RandomString(2) + rnd.Next(100, 999).ToString();

            return $"{part1}-{part2}-{part3}-{part4}";
        }
        public static void runSpoofer()
        {
            Program.blockInput = true;
            string newGuid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
            string newHostname = "DESKTOP-" + RandomString(7);
            string newName = RandomString(7);
            string buildGuid = Guid.NewGuid().ToString();
            var (valBuildLab, valBuildLabEx, valCurrentBuild) = GenerateBuildLabs();
            string CurrentBuild = valCurrentBuild;
            string CurrentVersion = RandomNumber();
            byte[] digitalProductId = GenerateRandomBytes(164);
            byte[] digitalProductId4 = GenerateRandomBytes(1272);
            int installDate = GenerateInstallDate();
            string ProductId = GenerateProductId();
            string HwProfileGuid = Guid.NewGuid().ToString("B");
            string machineGuid = Guid.NewGuid().ToString();
            string SusClientId = Guid.NewGuid().ToString();
            var tasks = new List<RegistryTask>
            {
                new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\SQMClient",
                    ValueName = "MachineId",
                    NewValue = newGuid
                },
                new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters",
                    ValueName = "Hostname",
                    NewValue = newHostname
                },
                new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters",
                    ValueName = "NV Hostname",
                    NewValue = newHostname
                },
                new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\ComputerName\",
                    ValueName = "ComputerName",
                    NewValue = newHostname
                },
                new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\ActiveComputerName\",
                    ValueName = "ComputerName",
                    NewValue = newHostname
                },
                new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    ValueName = "RegisteredOwner",
                    NewValue = newName
                },
                new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    ValueName = "BuildGUID",
                    NewValue = buildGuid
                },
                new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    ValueName = "BuildLab",
                    NewValue = valBuildLab
                },
                 new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    ValueName = "BuildLabEx",
                    NewValue = valBuildLabEx
                },
                new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    ValueName = "CurrentBuild",
                    NewValue = CurrentBuild
                },
                new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    ValueName = "CurrentBuildNumber",
                    NewValue = CurrentBuild
                },
                 new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    ValueName = "CurrentVersion",
                    NewValue = CurrentVersion
                },
                 new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    ValueName = "DigitalProductId",
                    NewValue = digitalProductId
                },
                 new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    ValueName = "DigitalProductId4",
                    NewValue = digitalProductId4
                },
                 new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    ValueName = "installDate",
                    NewValue = installDate
                },
                  new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    ValueName = "ProductId",
                    NewValue = ProductId
                },
                  new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\IDConfigDB\Hardware Profiles\0001",
                    ValueName = "HwProfileGuid",
                    NewValue = HwProfileGuid
                },

                new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography",
                    ValueName = "MachineGuid",
                    NewValue = machineGuid
                },
                new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate",
                    ValueName = "SusClientId",
                    NewValue = SusClientId
                },
                new RegistryTask
                {
                    FullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate",
                    ValueName = "SusClientIdValidation",
                    NewValue = GenerateRandomBytes(24)
                }

            };


            foreach (var task in tasks)
            {
                try
                {
                    //var oldVal = Registry.GetValue(task.FullPath, task.ValueName, null);
                    //Console.WriteLine($"Klucz: {task.ValueName}");
                   // Console.WriteLine($"  Było: {oldVal}");

                    Registry.SetValue(task.FullPath, task.ValueName, task.NewValue);

                    var newVal = Registry.GetValue(task.FullPath, task.ValueName, null);
                    string val = newVal?.ToString();
                    if (newVal is byte[] bytes)
                    {
                        val = BitConverter.ToString(bytes);
                    }
                    else
                    {
                        val = newVal?.ToString();
                    }
                    Settings.createLog(Settings.logType.Success, $" [{task.ValueName}] new value: {val}");

                    // Console.WriteLine($"  Jest: {newVal}");
                    //Console.WriteLine("-----------------------");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to change {task.ValueName}: {ex.Message}");
                    Settings.createLog(Settings.logType.Error, $"Failed to change {task.ValueName}: {ex.Message}");

                }
            }
            Program.blockInput = false;
            Settings.createLog(Settings.logType.Success, "ALT:V has been spoofed. Remember to change your browser (use a new profile with clean cookies), change your IP address, change your Discord account, and change your Social Club (if the server uses Cloud Auth).");
            Console.WriteLine("ALT:V has been spoofed. Remember to change your browser (use a new profile with clean cookies), change your IP address, change your Discord account, and change your Social Club (if the server uses Cloud Auth).");
        }
    }
}