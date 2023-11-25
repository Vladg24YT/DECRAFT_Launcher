﻿using DeCraftLauncher.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using static DeCraftLauncher.Utils.JarUtils;

namespace DeCraftLauncher.Configs
{

    public class JarConfig
    {
        public string friendlyName = "";
        public string jarFileName;
        public List<EntryPoint> entryPoints = new List<EntryPoint>();
        public List<string> foundMods = new List<string>();
        public string LWJGLVersion = "2.9.3";
        public string playerName = "DECRAFT_Player";
        public bool entryPointsScanned = false;
        public string instanceDirName = "";
        public string jvmArgs = "-Djava.util.Arrays.useLegacyMergeSort=true";
        public string proxyHost = "";
        public int windowW = 854;
        public int windowH = 480;
        public string maxJavaVersion = "";
        public string minJavaVersion = "";
        public bool isServer = false;

        //advanced options
        public string sessionID = "0";
        public string gameArgs = "";
        public bool cwdIsDotMinecraft = true;
        public bool appletEmulateHTTP = true;
        public string documentBaseUrl = "http://www.minecraft.net/play.jsp";
        public bool workaroundRetroMCP = false;
        public bool appletRedirectSkins = true;
        public string appletSkinRedirectPath = "C:\\skincache";
        public bool jarHasLWJGLClasses = false;
        public string jarBuiltInLWJGLDLLs = "";
        public bool appletLogHTTPRequests = false;
        public bool appletIsOnePointSixHereYet = false;

        public JarConfig(string jarFileName)
        {
            this.jarFileName = jarFileName;
            instanceDirName = jarFileName;
        }

        private XmlNode SetVal(XmlNode a, string v)
        {
            a.InnerText = v;
            return a;
        }

        public void SaveToXMLDefault()
        {
            SaveToXML($"{MainWindow.configDir}/{jarFileName}.xml");
        }

        public void SaveToXML(string path)
        {
            XmlDocument newXml = new XmlDocument();
            newXml.LoadXml("<?xml version=\"1.0\"?>\n<JarConfig>\n</JarConfig>");
            XmlNode rootElement = newXml.GetElementsByTagName("JarConfig")[0];
            rootElement.AppendChild(Util.GenElementChild(newXml, "FriendlyName", friendlyName));
            rootElement.AppendChild(Util.GenElementChild(newXml, "LWJGLVersion", LWJGLVersion));
            rootElement.AppendChild(Util.GenElementChild(newXml, "PlayerName", playerName));
            rootElement.AppendChild(Util.GenElementChild(newXml, "InstanceDirectory", instanceDirName));
            rootElement.AppendChild(Util.GenElementChild(newXml, "JVMArgs", jvmArgs));
            rootElement.AppendChild(Util.GenElementChild(newXml, "WindowW", windowW + ""));
            rootElement.AppendChild(Util.GenElementChild(newXml, "WindowH", windowH + ""));
            rootElement.AppendChild(Util.GenElementChild(newXml, "ProxyHost", proxyHost));

            //advanced options
            rootElement.AppendChild(Util.GenElementChild(newXml, "PassSessionID", sessionID));
            rootElement.AppendChild(Util.GenElementChild(newXml, "GameArgs", gameArgs));
            rootElement.AppendChild(Util.GenElementChild(newXml, "JavaCwdIsDotMinecraft", cwdIsDotMinecraft.ToString()));
            rootElement.AppendChild(Util.GenElementChild(newXml, "AppletEmulateHTTP", appletEmulateHTTP.ToString()));
            rootElement.AppendChild(Util.GenElementChild(newXml, "AppletDocumentURL", documentBaseUrl));
            rootElement.AppendChild(Util.GenElementChild(newXml, "AppletRedirectToLocalSkins", appletRedirectSkins.ToString()));
            rootElement.AppendChild(Util.GenElementChild(newXml, "AppletSkinRedirectLocalPath", appletSkinRedirectPath));
            rootElement.AppendChild(Util.GenElementChild(newXml, "AppletLogHTTPUrls", appletLogHTTPRequests.ToString()));
            rootElement.AppendChild(Util.GenElementChild(newXml, "AppletIsOnePointSixHereYet", appletIsOnePointSixHereYet.ToString()));

            //generated by jar scanner
            XmlNode nodeValsAutogenerated = rootElement.AppendChild(Util.GenElementChild(newXml, "AutoGenerated"));
            nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "EntryPointsScanned", entryPointsScanned.ToString()));
            nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "MaxJavaVersion", maxJavaVersion));
            nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "MinJavaVersion", minJavaVersion));
            nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "JarHasMissingSynthetics", workaroundRetroMCP.ToString()));
            nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "JarHasLWJGL", jarHasLWJGLClasses.ToString()));
            nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "JarLWJGLNativesDir", jarBuiltInLWJGLDLLs));
            nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "IsServer", isServer.ToString()));

            XmlNode nodeFoundMods = nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "MLMods"));
            foreach (string x in foundMods)
            {
                nodeFoundMods.AppendChild(Util.GenElementChild(newXml, "MLMod", x));
            }

            XmlNode entryPointsList = nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "EntryPoints"));
            foreach (EntryPoint a in entryPoints)
            {
                XmlNode nEntryPoint = entryPointsList.AppendChild(Util.GenElementChild(newXml, "EntryPoint"));
                nEntryPoint.AppendChild(Util.GenElementChild(newXml, "ClassPath", a.classpath));
                nEntryPoint.AppendChild(Util.GenElementChild(newXml, "LaunchType", ((int)a.type).ToString()));
                nEntryPoint.AppendChild(Util.GenElementChild(newXml, "ClassInfo", a.additionalInfo));
            }

            newXml.Save(path);
        }

        public static JarConfig LoadFromXML(string path, string jarName)
        {
            JarConfig newJarConf = new JarConfig(jarName);

            XmlDocument newXml = new XmlDocument();
            newXml.Load(path);
            XmlNode rootNode = newXml.SelectSingleNode("JarConfig");
            if (rootNode != null)
            {

                newJarConf.friendlyName = Util.GetInnerOrDefault(rootNode, "FriendlyName");
                newJarConf.LWJGLVersion = Util.GetInnerOrDefault(rootNode, "LWJGLVersion", "2.9.3");
                newJarConf.playerName = Util.GetInnerOrDefault(rootNode, "PlayerName", "DECRAFT_player");
                newJarConf.jvmArgs = Util.GetInnerOrDefault(rootNode, "JVMArgs", "-Djava.util.Arrays.useLegacyMergeSort=true");
                newJarConf.instanceDirName = Util.GetInnerOrDefault(rootNode, "InstanceDirectory", jarName);
                newJarConf.windowW = int.Parse(Util.GetInnerOrDefault(rootNode, "WindowW", "854", "int"));
                newJarConf.windowH = int.Parse(Util.GetInnerOrDefault(rootNode, "WindowH", "480", "int"));
                newJarConf.proxyHost = Util.GetInnerOrDefault(rootNode, "ProxyHost");

                newJarConf.sessionID = Util.GetInnerOrDefault(rootNode, "SessionID", "0");
                newJarConf.gameArgs = Util.GetInnerOrDefault(rootNode, "GameArgs");
                newJarConf.cwdIsDotMinecraft = bool.Parse(Util.GetInnerOrDefault(rootNode, "JavaCwdIsDotMinecraft", "true", "bool"));
                newJarConf.appletEmulateHTTP = bool.Parse(Util.GetInnerOrDefault(rootNode, "AppletEmulateHTTP", "true", "bool"));
                newJarConf.documentBaseUrl = Util.GetInnerOrDefault(rootNode, "AppletDocumentURL", "http://www.minecraft.net/play.jsp");
                newJarConf.appletRedirectSkins = bool.Parse(Util.GetInnerOrDefault(rootNode, "AppletRedirectToLocalSkins", "true", "bool"));
                newJarConf.appletSkinRedirectPath = Util.GetInnerOrDefault(rootNode, "AppletSkinRedirectLocalPath", "C:\\skincache");
                newJarConf.appletLogHTTPRequests = bool.Parse(Util.GetInnerOrDefault(rootNode, "AppletLogHTTPUrls", "false", "bool"));
                newJarConf.appletIsOnePointSixHereYet = bool.Parse(Util.GetInnerOrDefault(rootNode, "AppletIsOnePointSixHereYet", "false", "bool"));

                //generated by jar scanner
                XmlNode nodeValsAutogenerated = rootNode.SelectSingleNode("AutoGenerated");
                if (nodeValsAutogenerated != null)
                {
                    newJarConf.entryPointsScanned = bool.Parse(Util.GetInnerOrDefault(nodeValsAutogenerated, "EntryPointsScanned", "false", "bool"));
                    newJarConf.maxJavaVersion = Util.GetInnerOrDefault(nodeValsAutogenerated, "MaxJavaVersion");
                    newJarConf.minJavaVersion = Util.GetInnerOrDefault(nodeValsAutogenerated, "MinJavaVersion");
                    newJarConf.jarHasLWJGLClasses = bool.Parse(Util.GetInnerOrDefault(nodeValsAutogenerated, "JarHasLWJGL", "false", "bool"));
                    newJarConf.workaroundRetroMCP = bool.Parse(Util.GetInnerOrDefault(nodeValsAutogenerated, "JarHasMissingSynthetics", "false", "bool"));
                    newJarConf.jarBuiltInLWJGLDLLs = Util.GetInnerOrDefault(nodeValsAutogenerated, "JarLWJGLNativesDir");
                    newJarConf.isServer = bool.Parse(Util.GetInnerOrDefault(nodeValsAutogenerated, "IsServer", "false", "bool"));

                    XmlNode mlModsNode = nodeValsAutogenerated.SelectSingleNode("MLMods");
                    if (mlModsNode != null)
                    {
                        foreach (XmlNode foundModNode in mlModsNode.SelectNodes("MLMod"))
                        {
                            newJarConf.foundMods.Add(foundModNode.InnerText);
                        }
                    }
                }
                foreach (XmlNode a in newXml.GetElementsByTagName("EntryPoint"))
                {
                    EntryPoint b = new EntryPoint();
                    string classPath = Util.GetInnerOrDefault(a, "ClassPath", null);
                    string type = Util.GetInnerOrDefault(a, "LaunchType", null, "int");
                    string additionalInfo = Util.GetInnerOrDefault(a, "ClassInfo", "");
                    if (classPath == null || type == null)
                    {
                        continue;
                    }
                    b.classpath = classPath;
                    try
                    {
                        b.type = (EntryPointType)int.Parse(type);
                    }
                    catch (FormatException)
                    {
                        continue;
                    }
                    b.additionalInfo = additionalInfo;
                    newJarConf.entryPoints.Add(b);
                }
            }

            return newJarConf;
        }

        public void DoLaunch(EntryPoint entryPoint, MainWindow caller)
        {
            if (entryPoint.type == JarUtils.EntryPointType.STATIC_VOID_MAIN)
            {
                JavaExec mainFunctionExec = new JavaExec(entryPoint.classpath);

                mainFunctionExec.classPath.Add(Path.GetFullPath(MainWindow.jarDir + "/" + jarFileName));
                if (workaroundRetroMCP)
                {
                    mainFunctionExec.classPath.Add(Path.GetFullPath("./java_temp"));
                }
                if (LWJGLVersion != "+ built-in")
                {
                    //mainFunctionExec.classPath.Add($"{MainWindow.currentDirectory}/lwjgl/{jarConfig.LWJGLVersion}/*");

                    //the above is a much more efficient way of doing this right???
                    //yeah
                    //but it doesn't work with java 5 which can't handle wildcards

                    try
                    {
                        foreach (string f in Directory.GetFiles($"{MainWindow.currentDirectory}/lwjgl/{LWJGLVersion}/"))
                        {
                            if (f.EndsWith(".jar"))
                            {
                                mainFunctionExec.classPath.Add(f);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // don't care i think
                    }
                }

                if (proxyHost != "")
                {
                    mainFunctionExec.jvmArgs.Add($"-Dhttp.proxyHost={proxyHost.Replace(" ", "%20")}");
                }
                mainFunctionExec.jvmArgs.Add($"-Djava.library.path=\"{MainWindow.currentDirectory}/lwjgl/{(LWJGLVersion == "+ built-in" ? "_temp_builtin" : LWJGLVersion)}/native\"");
                mainFunctionExec.jvmArgs.Add(jvmArgs);

                if (!isServer)
                {
                    if (playerName != "")
                    {
                        mainFunctionExec.programArgs.Add($"\"{playerName}\"");
                    }
                    if (sessionID != "")
                    {
                        mainFunctionExec.programArgs.Add(sessionID);
                    }
                }
                mainFunctionExec.programArgs.Add(gameArgs);
                Console.WriteLine("Running command: java " + mainFunctionExec.GetFullArgsString());

                string emulatedAppDataDir = Path.GetFullPath($"{MainWindow.currentDirectory}/{MainWindow.instanceDir}/{instanceDirName}");
                mainFunctionExec.appdataDir = emulatedAppDataDir;
                mainFunctionExec.workingDirectory = $"{emulatedAppDataDir}{(cwdIsDotMinecraft ? "/.minecraft" : "")}";
                try
                {
                    mainFunctionExec.StartOpenWindowAndAddToInstances(caller, this);
                }
                catch (Win32Exception w32e)
                {
                    MessageBox.Show($"Error launching java process: {w32e.Message}\n\nVerify that Java is installed in \"Runtime settings\".");
                }
                
            }
            else if (entryPoint.type == JarUtils.EntryPointType.APPLET)
            {
                AppletWrapper.TryLaunchAppletWrapper(entryPoint.classpath, caller, this);
            }
            else if (entryPoint.type == JarUtils.EntryPointType.CUSTOM_LAUNCH_COMMAND)
            {
                WindowProcessLog processLog = new WindowProcessLog(new JavaExec(null).StartWithCustomArgsString(entryPoint.classpath), caller, isServer);
                processLog.Show();
                caller.AddRunningInstance(new UIControls.InstanceListElement.RunningInstanceData(friendlyName != "" ? friendlyName : jarFileName, processLog));
            }
            else
            {
                throw new NotImplementedException("What");
            }
        }

        public bool EvalPrereqs()
        {
            if (this.LWJGLVersion == "+ built-in")
            {
                JarUtils.ExtractBuiltInLWJGLToTempFolder(this);
            }

            MainWindow.EnsureDir(MainWindow.instanceDir + "/" + instanceDirName);
            if (cwdIsDotMinecraft)
            {
                MainWindow.EnsureDir(MainWindow.instanceDir + "/" + instanceDirName + "/.minecraft");
            }

            if (workaroundRetroMCP)
            {
                try
                {
                    File.WriteAllText("./java_temp/Minecraft.java", JavaCode.GenerateAlphaModWorkaround());
                    List<string> compilerOut = RunProcessAndGetOutput(MainWindow.mainRTConfig.javaHome + "javac",
                           $"./java_temp/Minecraft.java " +
                           $"-d ./java_temp ", true);
                    File.Delete("./java_temp/net/minecraft/client/Minecraft.class");
                    Console.WriteLine("Successfully built missing synthetic class workaround");
                } catch (ApplicationException)
                {
                    MessageBox.Show("Failed to compile missing synthetic class workaround.", "DECRAFT");
                    return false;
                } catch (Win32Exception)
                {
                    MessageBox.Show("Workaround for missing synthetic classes (currently enabled in Advanced settings) requires JDK installed.", "DECRAFT");
                    return false;
                }
            }

            return true;
        }
    }
}
