using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Microsoft.Win32;

namespace SendPCImfo.App
{
    internal class MyDriveInfo
    {
        public string Name { get; set; }
        public string DriveFormat { get; set; }
        public string TotalSize { get; set; }
        public string AvailableFreeSpace { get; set; }
    }

    internal class PCInfo
    {
        // SYSTEM INFO
        public string PCIPV4 { get; set; } = null;
        public string PCName { get; set; } = null;
        public string OSVersion { get; set; } = null;
        public string SystemBitRate { get; set; } = null;
        public string SystemCatalogPath { get; set; } = null;
        // CPU INFO
        public string CPUModel { get; set; } = null;
        public string CPUName { get; set; } = null;
        public string CPUManufacturerer { get; set; } = null;
        public string CPUNMaxClockSpeed { get; set; } = null;
        public int CPUPhysicalCount { get; set; }
        public int CPUPCoresCount { get; set; }
        public int LogicalCPUCount { get; set; } 
        // PC DRIVE INFO
        public int DriveCount { get; set; } = 0;
        public string HDDName { get; set; } = null;
        public string HDDSize { get; set; } = null;
        public List<MyDriveInfo> DriveInfos { get; set; } = new List<MyDriveInfo>();
        // RAM INFO
        public int RAMCount { get; set; }
        public string TotalRAM { get; set; }
        // PC Screen Resolution
        public int ScreenCount { get; set; }
        public string HResol { get; set; } = null;
        public string WResol { get; set; } = null;
        // PC VIDEO CARD
        public string VideoCardName { get; set; } = null;
        public string VideoCardMemoryAmount { get; set; } = null;



        public PCInfo()
        {
            var ramMonitor = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            var query = new ObjectQuery("SELECT * FROM Win32_VideoController");
            var scope = new ManagementScope();
            scope.Connect();
            int RAM_count = 0;

            PCIPV4 = GetIP4Address();
            OSVersion = Environment.OSVersion.ToString();
            PCName = Environment.MachineName;
            SystemBitRate = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            CPUModel = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
            foreach (var item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {
                CPUPhysicalCount = Convert.ToInt32(item["NumberOfProcessors"]);
            }
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }
            CPUPCoresCount = coreCount;
            SystemCatalogPath = Environment.SystemDirectory;
            LogicalCPUCount = Environment.ProcessorCount;
            CPUName = GetHardwareInfo("Win32_Processor", "Name");

            RegistryKey frecKey = Registry.LocalMachine;
            frecKey = frecKey.OpenSubKey(
               @"HARDWARE\DESCRIPTION\System\CentralProcessor\0", false);
            CPUNMaxClockSpeed = frecKey.GetValue("~MHz").ToString();

            CPUManufacturerer = GetHardwareInfo("Win32_Processor", "Manufacturer");
            VideoCardName = GetHardwareInfo("Win32_VideoController", "Name");
            VideoCardMemoryAmount = GetHardwareInfo("Win32_VideoController", "AdapterRAM");
            HDDName = GetHardwareInfo("Win32_DiskDrive", "Caption");
            HDDSize = (GetHardwareInfo("Win32_DiskDrive", "Size"));

            foreach (DriveInfo dI in DriveInfo.GetDrives())
            {
                DriveInfos.Add(new MyDriveInfo
                {
                    Name = dI.Name,
                    DriveFormat = dI.DriveFormat,
                    TotalSize = (Math.Round((double)dI.TotalSize / 1024 / 1024 / 1024, 2)).ToString(),
                    AvailableFreeSpace = (Math.Round((double)dI.AvailableFreeSpace / 1024 / 1024 / 1024, 2)).ToString(),
                });
            }
            DriveCount = DriveInfos.Count;


            foreach (ManagementObject objram in ramMonitor.Get())
            {
                RAM_count++;
                double totalRam = Convert.ToDouble(GetHardwareInfo("Win32_ComputerSystem", "TotalPhysicalMemory")) / 1024 / 1024;
                TotalRAM = (Math.Round(totalRam, 0)).ToString();
            }
            RAMCount = RAM_count;

            using (var searcher = new ManagementObjectSearcher(scope, query))
            {
                var results = searcher.Get();
                foreach (var result in results)
                {
                    if (result.GetPropertyValue("CurrentHorizontalResolution") != null)
                    {
                        HResol = result.GetPropertyValue("CurrentHorizontalResolution").ToString();
                        WResol = result.GetPropertyValue("CurrentVerticalResolution").ToString();
                    }
                }
            }
            ScreenCount = System.Windows.Forms.Screen.AllScreens.Length;

        }

        public static string GetIP4Address()
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }

        private static string GetHardwareInfo(string WIN32_Class, string ClassItemField)
        {
            var result = "";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM " + WIN32_Class);

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    result += (obj[ClassItemField].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }
    }
}
