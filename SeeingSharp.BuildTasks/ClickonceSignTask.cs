#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SeeingSharp.BuildTasks
{
    /// <summary>
    /// This task is responsible for automated signing of clickonce installation files.
    /// All steps of the following coding are besed on the workflow described here:
    /// http://blogs.msdn.com/b/vsto/archive/2009/04/29/signing-and-re-signing-manifests-in-clickonce.aspx
    /// </summary>
    public class ClickonceSignTask : Task
    {
        #region Constants
        private const string SUBDIRECTORY_APPFILES = "Application Files";
        private const string FILE_PASSWORD = "Password.txt";
        private const string EXTENSION_DEPLOY = ".deploy";
        #endregion

        /// <summary>
        /// Führt beim Überschreiben in einer abgeleiteten Klasse die Aufgabe aus.
        /// </summary>
        public override bool Execute()
        {
            this.Log.LogMessage("***************************");
            this.Log.LogMessage("Entered custom ClickonceSignTask");

            if (string.IsNullOrEmpty(PublishDirectory))
            {
                this.Log.LogError("No publish path given!");
                return false;
            }
            if(string.IsNullOrEmpty(CertInformationDirectory))
            {
                this.Log.LogError("Certification information path not given!");
                return false;
            }

            // Log start of the action
            this.Log.LogMessage("Signing application in publish path {0}", this.PublishDirectory);

            // Search main tool paths (take them from WindowsSDK 7.1)
            string winSdkPath = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\Microsoft SDKs\\Windows\\v7.1", "InstallationFolder", null)
                as string;
            if (string.IsNullOrEmpty(winSdkPath))
            {
                base.Log.LogError("Unable to compile shader: Windows SDK 7.1 not found!");
                return false;
            }
            string mageToolPath = Path.Combine(winSdkPath, "Bin\\mage.exe");
            string signToolPath = Path.Combine(winSdkPath, "Bin\\signtool.exe");

            // Get the directory with certificate information
            string certDirectory = this.CertInformationDirectory;
            if(!Directory.Exists(certDirectory))
            {
                this.Log.LogError("No certificate informatin found in publish directory!");
                return false;
            }

            // Get the certificate file
            string certFile = Directory.GetFiles(certDirectory, "*.pfx").FirstOrDefault();
            if(string.IsNullOrEmpty(certFile))
            {
                this.Log.LogError("No certificate file found in Cert directory!");
                return false;
            }

            // Password file
            string passwordFilePath = Path.Combine(certDirectory, FILE_PASSWORD);
            if(!File.Exists(passwordFilePath))
            {
                this.Log.LogError("No password file found in Cert directory!");
                return false;
            }
            
            // Get the password
            string password = File.ReadAllLines(passwordFilePath).FirstOrDefault();
            if(string.IsNullOrEmpty(password))
            {
                this.Log.LogError("No password found in password file!");
                return false;
            }

            // Check for 'Application Files' directory
            string appFileDirectory = Path.Combine(this.PublishDirectory, SUBDIRECTORY_APPFILES);
            if (!Directory.Exists(appFileDirectory))
            {
                this.Log.LogError("Subdirectory for application files not found!");
                return false;
            }

            // Select the newest 'Application Files' directory
            string newestAppFilesDir = 
                (from actSubdirectory in Directory.GetDirectories(appFileDirectory)
                let splittedName = Path.GetFileName(actSubdirectory).Split(
                    new char[]{ '_' }, 
                    StringSplitOptions.RemoveEmptyEntries)
                where splittedName.Length >= 4
                let actVersion = TryParseVersion(splittedName)
                where actVersion != null
                select new { Version = actVersion, DirectoryName = actSubdirectory})
                .OrderByDescending((actEntry) => actEntry.Version)
                .Select((actEntry) => actEntry.DirectoryName)
                .FirstOrDefault();
            if(string.IsNullOrEmpty(newestAppFilesDir))
            {
                this.Log.LogError("No suitable 'Application Files' subdirectory found!");
                return false;
            }
            else
            {
                this.Log.LogMessage("Signing application data in directory {0}", newestAppFilesDir);
            }

            // Remove all deploy extensions
            RenameAllFiles_RemoveDeply(newestAppFilesDir);
            try
            {
                // Do sign the setup.exe file (using signtool.exe)
                foreach(string actSetupExeFile in Directory.GetFiles(PublishDirectory, "*.exe"))
                {
                    bool result = TryRunProcess(
                        signToolPath,
                        "sign",
                        "/f", certFile,
                        "/p", password,
                        "/v", actSetupExeFile);
                    if (!result) { return false; }
                }

                // Do sign all exe files first (using signtool.exe)
                foreach(string actExeFile in Directory.GetFiles(newestAppFilesDir, "*.exe"))
                {
                    bool result = TryRunProcess(
                        signToolPath,
                        "sign",
                        "/f", certFile,
                        "/p", password,
                        "/v", actExeFile);
                    if (!result) { return false; }
                }

                // Do sign all *.exe.manifest files first (using mage.exe)
                foreach (string actManifestFile in Directory.GetFiles(newestAppFilesDir, "*.exe.manifest"))
                {
                    bool result = TryRunProcess(
                        mageToolPath,
                        "-update", actManifestFile,
                        "-certfile", certFile,
                        "-password", password);
                    if (!result) { return false; }
                }

                // Do sign all *.application files first (using mage.exe)
                foreach (string actApplicationFile in Directory.GetFiles(newestAppFilesDir, "*.application"))
                {
                    bool result = TryRunProcess(
                        mageToolPath,
                        "-update", actApplicationFile,
                        "-appmanifest", Path.Combine(
                            Path.GetDirectoryName(actApplicationFile),
                            Path.GetFileNameWithoutExtension(actApplicationFile) + ".exe.manifest"),
                        "-certfile", certFile,
                        "-password", password);
                    if (!result) { return false; }
                }

                // Do sign main *.application file (using mage.exe)
                foreach (string actApplicationFile in Directory.GetFiles(PublishDirectory, "*.application"))
                {
                    bool result = TryRunProcess(
                        mageToolPath,
                        "-update", actApplicationFile,
                        "-appmanifest", Path.Combine(
                            newestAppFilesDir,
                            Path.GetFileNameWithoutExtension(actApplicationFile) + ".exe.manifest"),
                        "-certfile", certFile,
                        "-password", password);
                    if (!result) { return false; }
                }
            }
            finally
            {
                RenameAllFiles_AddDeploy(newestAppFilesDir);
            }

            return true;
        }

        /// <summary>
        /// Tries to parse the version out of the given parts from a 'Application Files' subdirectory.
        /// </summary>
        /// <param name="splittedDirectoryName">The splitted directory name.</param>
        private static Version TryParseVersion(string[] splittedDirectoryName)
        {
            int value1 = 0;
            int value2 = 0;
            int value3 = 0;
            int value4 = 0;
            if(Int32.TryParse(splittedDirectoryName[splittedDirectoryName.Length - 4], out value1) &&
               Int32.TryParse(splittedDirectoryName[splittedDirectoryName.Length - 3], out value2) &&
               Int32.TryParse(splittedDirectoryName[splittedDirectoryName.Length - 2], out value3) &&
               Int32.TryParse(splittedDirectoryName[splittedDirectoryName.Length - 1], out value4))
            {
                return new Version(value1, value2, value3, value4);
            }

            return null;
        }

        private static void RenameAllFiles_RemoveDeply(string directoryName)
        {
            // Remove all .deploy extensions
            foreach(string actFileName in Directory.GetFiles(directoryName))
            {
                if(actFileName.EndsWith(EXTENSION_DEPLOY, StringComparison.OrdinalIgnoreCase))
                {
                    File.Move(actFileName, actFileName.Substring(0, actFileName.Length - EXTENSION_DEPLOY.Length));
                }
            }

            foreach(string actSubdirectory in Directory.GetDirectories(directoryName))
            {
                RenameAllFiles_RemoveDeply(actSubdirectory);
            }
        }

        private static void RenameAllFiles_AddDeploy(string directoryName)
        {
            // Add all .deploy extensions
            foreach (string actFileName in Directory.GetFiles(directoryName))
            {
                if (actFileName.EndsWith(".application", StringComparison.OrdinalIgnoreCase)) { continue; }
                if (actFileName.EndsWith(".exe.manifest", StringComparison.OrdinalIgnoreCase)) { continue; }

                if (!actFileName.EndsWith(EXTENSION_DEPLOY, StringComparison.OrdinalIgnoreCase))
                {
                    File.Move(actFileName, actFileName + EXTENSION_DEPLOY);
                }
            }

            foreach (string actSubdirectory in Directory.GetDirectories(directoryName))
            {
                RenameAllFiles_AddDeploy(actSubdirectory);
            }
        }

        /// <summary>
        /// Tries to run the given process with the given parameters.
        /// </summary>
        /// <param name="exeName">Name of the executable.</param>
        /// <param name="parameters">The parameters.</param>
        private bool TryRunProcess(string exeName, params string[] parameters)
        {
            // Build the argument string
            StringBuilder argString = new StringBuilder(1024);
            foreach (string actParameter in parameters)
            {
                if (argString.Length > 0) { argString.Append(' '); }
                if (actParameter.Contains(' '))
                {
                    argString.Append('"');
                    argString.Append(actParameter);
                    argString.Append('"');
                }
                else
                {
                    argString.Append(actParameter);
                }
            }

            // Configure process call to fxc.exe
            bool childProcessNotifiedErrors = false;
            using (Process childProcess = new Process())
            {
                childProcess.StartInfo = new ProcessStartInfo(
                    exeName, argString.ToString());
                childProcess.EnableRaisingEvents = true;
                childProcess.StartInfo.StandardErrorEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
                childProcess.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
                childProcess.StartInfo.UseShellExecute = false;
                childProcess.StartInfo.CreateNoWindow = true;
                childProcess.StartInfo.RedirectStandardError = true;
                childProcess.StartInfo.RedirectStandardInput = true;
                childProcess.StartInfo.RedirectStandardOutput = true;

                // Register on error data
                StringBuilder errorStringBuilder = new StringBuilder();
                childProcess.ErrorDataReceived += (sender, eArgs) =>
                {
                    if (string.IsNullOrWhiteSpace(eArgs.Data)) { return; }
                    errorStringBuilder.Append(eArgs.Data);
                    childProcessNotifiedErrors = true;
                };

                // Start the process and wait for exit
                childProcess.Start();
                childProcess.BeginErrorReadLine();
                childProcess.WaitForExit();
                childProcess.CancelErrorRead();

                // Check fxc process result
                if ((childProcess.ExitCode != 0) ||
                    (childProcessNotifiedErrors))
                {
                    if (errorStringBuilder.Length > 0)
                    {
                        base.Log.LogError(errorStringBuilder.ToString());
                    }
                    else
                    {
                        base.Log.LogError("Unknown error during execution of " + exeName + " with arguments " + argString.ToString());
                    }
                }
            }
            return !childProcessNotifiedErrors;
        }


        [Required]
        public string PublishDirectory
        {
            get;
            set;
        }

        [Required]
        public string CertInformationDirectory
        {
            get;
            set;
        }
    }
}
