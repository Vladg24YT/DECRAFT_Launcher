﻿using DeCraftLauncher.Configs;
using DeCraftLauncher.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static DeCraftLauncher.Utils.JarUtils;

namespace DeCraftLauncher
{
    public class AppletWrapper
    {
        public static void LaunchAppletWrapper(string className, JarConfig jar, Dictionary<string, string> appletParameters)
        {
            //first, compile the applet wrapper
            //todo: clean this up in the same way as i did with javaexec
            MainWindow.EnsureDir("./java_temp");
            File.WriteAllText("./java_temp/AppletWrapper.java", JavaCode.GenerateAppletWrapperCode(className, jar, appletParameters));
            if (jar.appletEmulateHTTP)
            {
                File.WriteAllText("./java_temp/InjectedStreamHandlerFactory.java", JavaCode.GenerateHTTPStreamInjectorCode(jar));
            }
            List<string> compilerOut;
            try
            {
                compilerOut = RunProcessAndGetOutput(MainWindow.mainRTConfig.javaHome + "javac", $"-cp \"{MainWindow.jarDir}/{jar.jarFileName}\" " +
                    $"./java_temp/AppletWrapper.java " +
                    (jar.appletEmulateHTTP ? $"./java_temp/InjectedStreamHandlerFactory.java " : "") +
                    $"-d ./java_temp " +
                    (jar.appletEmulateHTTP && MainWindow.mainRTConfig.isJava9 ? "--add-exports java.base/sun.net.www.protocol.http=ALL-UNNAMED " : ""), true);
            } catch (ApplicationException)
            {
                MessageBox.Show("Failed to compile the Applet Wrapper.\n\nNote: the Applet Wrapper only supports JDK 6+", "DECRAFT");
                return;
            }
            Console.WriteLine("Compilation log:");
            foreach (string a in compilerOut)
            {
                Console.WriteLine(a);
            }


            //now we launch the compiled class
            JavaExec appletExec = new JavaExec("decraft_internal.AppletWrapper");

            //class paths
            //todo: make this cleaner (preferrably without getting rid of relative paths)
            string relativePath = (jar.cwdIsDotMinecraft ? "../" : "") + "../../";
            appletExec.classPath.Add($"{relativePath}java_temp/");
            if (jar.LWJGLVersion != "+ built-in")
            {
                appletExec.classPath.Add($"{relativePath}lwjgl/{jar.LWJGLVersion}/*");
            }
            appletExec.classPath.Add($"{relativePath}{MainWindow.jarDir}/{jar.jarFileName}");


            //jvm args
            appletExec.jvmArgs.Add($"-Djava.library.path={relativePath}lwjgl/{(jar.LWJGLVersion == "+ built-in" ? "_temp_builtin" : jar.LWJGLVersion)}/native");
            appletExec.jvmArgs.Add(jar.jvmArgs);
            if (jar.proxyHost != "")
            {
                appletExec.jvmArgs.Add($"-Dhttp.proxyHost={jar.proxyHost.Replace(" ", "%20")}");
            }
            if (jar.appletEmulateHTTP && MainWindow.mainRTConfig.isJava9)
            {
                appletExec.jvmArgs.Add("--add-exports java.base/sun.net.www.protocol.http=ALL-UNNAMED");
            }

            //game args
            if (jar.gameArgs != "")
            {
                appletExec.programArgs.Add(jar.gameArgs);
            }

            Console.WriteLine($"[LaunchAppletWrapper] Running command: java {appletExec.GetFullArgsString()}");

            string emulatedAppDataDir = Path.GetFullPath($"{MainWindow.currentDirectory}/{MainWindow.instanceDir}/{jar.instanceDirName}");
            appletExec.appdataDir = emulatedAppDataDir;
            appletExec.workingDirectory = $"{emulatedAppDataDir}{(jar.cwdIsDotMinecraft ? "/.minecraft" : "")}";
            try
            {
                Process newProcess = appletExec.Start();
                new WindowProcessLog(newProcess).Show();
                Thread.Sleep(1000);
                Util.SetWindowDarkMode(newProcess.MainWindowHandle);

                //nproc = JarUtils.RunProcess(MainWindow.mainRTConfig.javaHome + "java", args, emulatedAppDataDir);
            } catch (Win32Exception w32e)
            {
                MessageBox.Show($"Error launching java process: {w32e.Message}\n\nVerify that Java is installed in \"Runtime settings\".");
            }
        }

        public static void TryLaunchAppletWrapper(string classpath, JarConfig jarConfig, Dictionary<string, string> appletParameters = null)
        {
            if (!classpath.Contains('.'))
            {
                MessageBox.Show("Launching default package applets is not implemented.", "DECRAFT");
            }
            else
            {
                try
                {
                    AppletWrapper.LaunchAppletWrapper(classpath, jarConfig, appletParameters != null ? appletParameters : new Dictionary<string, string>());
                }
                catch (Win32Exception)
                {
                    MessageBox.Show("Applet wrapper requires JDK installed.");
                }
            }
        }
    }
}
