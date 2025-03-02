using System;
using System.IO;
using System.Management;
using System.Threading;

namespace ExternalDriveCopyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Waiting for external drive to be connected...");

            // Start a separate thread to monitor USB connection events
            Thread monitorThread = new Thread(MonitorUSBDrive);
            monitorThread.Start();

            Console.ReadLine(); // Keep the app running
        }

        static void MonitorUSBDrive()
        {
            // Set up a ManagementEventWatcher to listen for drive connection events
            ManagementEventWatcher watcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");
            watcher.EventArrived += (sender, e) => OnExternalDriveConnected();
            watcher.Query = query;
            watcher.Start();
        }

        // This method will be called when an external drive is connected
        static void OnExternalDriveConnected()
        {
            Console.WriteLine("External drive detected! Starting file copy...");

            string sourceDir = @"C:\SourceFolder";  // Change to your source folder
            string targetDir = @"D:\";  // Automatically detect the external drive or hard-code the drive letter

            // Ensure the target directory exists
            if (!Directory.Exists(targetDir))
            {
                Console.WriteLine("Target directory not found. Exiting.");
                return;
            }

            // Copy files from source to target
            CopyFiles(sourceDir, targetDir);
        }

        static void CopyFiles(string sourceDir, string targetDir)
        {
            try
            {
                // Get all files in the source directory
                string[] files = Directory.GetFiles(sourceDir);

                foreach (var file in files)
                {
                    string destFile = Path.Combine(targetDir, Path.GetFileName(file));
                    File.Copy(file, destFile, true);  // Set 'true' to overwrite existing files

                    Console.WriteLine($"Copied {file} to {destFile}");
                }

                Console.WriteLine("File copy completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during file copy: {ex.Message}");
            }
        }
    }
}
