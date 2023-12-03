﻿using DeCraftLauncher.Configs;
using DeCraftLauncher.Utils;
using SourceChord.FluentWPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DeCraftLauncher.Configs.UI
{

    public partial class WindowRuntimeConfig : AcrylicWindow
    {
        MainWindow parent;

        public WindowRuntimeConfig(MainWindow parent)
        {
            this.parent = parent;
            InitializeComponent();
            Util.UpdateAcrylicWindowBackground(this);
            jre_path.Text = MainWindow.mainRTConfig.javaHome;
            checkbox_isjava9.IsChecked = MainWindow.mainRTConfig.isJava9;
            checkbox_autoexitprocesslog.IsChecked = MainWindow.mainRTConfig.autoExitProcessLog;
            checkbox_enablediscord.IsChecked = MainWindow.mainRTConfig.enableDiscordRPC;
        }

        public void FixJavaHomeString()
        {
            jre_path.Text = jre_path.Text.TrimEnd(' ').TrimStart(' ');
            if (jre_path.Text != "" && (!jre_path.Text.EndsWith("\\") && !jre_path.Text.EndsWith("/")))
            {
                jre_path.Text += "\\";
            }
        }

        public void UpdateJREVersionString()
        {
            FixJavaHomeString();

            if (jre_path.Text != "" && !File.Exists(jre_path.Text + "java.exe") && File.Exists(jre_path.Text + "bin\\java.exe"))
            {
                jre_path.Text += "bin\\";
            }

            string verre = JarUtils.GetJREInstalled(jre_path.Text);
            string verdk = JarUtils.GetJDKInstalled(jre_path.Text);
            if (verdk != null)
            {
                int JDKVer = Util.TryParseJavaCVersionString(verdk);
                Console.WriteLine($"Detected JDK Version: {JDKVer}");
                if (JDKVer != -1)
                {
                    checkbox_isjava9.IsChecked = JDKVer >= 9;
                }
            }

            jreconfig_version.Content = Util.CleanStringForXAML($"<press Enter to test>\nJRE: {(verre != null ? verre : "<none>")}\nJDK: {(verdk != null ? verdk : "<none>")}");

        }

        public void SetAndTestJavaPath(string path)
        {
            this.jre_path.Text = path;
            UpdateJREVersionString();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void jre_path_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateJREVersionString();
            }
        }

        private void AcrylicWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                FixJavaHomeString();
                MainWindow.mainRTConfig.javaHome = jre_path.Text;
                MainWindow.mainRTConfig.isJava9 = checkbox_isjava9.IsChecked == true;
                MainWindow.mainRTConfig.autoExitProcessLog = checkbox_autoexitprocesslog.IsChecked == true;
                MainWindow.mainRTConfig.enableDiscordRPC = checkbox_enablediscord.IsChecked == true;
                parent.SaveRuntimeConfig();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "DECRAFT");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new WindowJavaFinder(this).Show();
        }

        private void checkbox_enablediscord_Checked(object sender, RoutedEventArgs e)
        {
            GlobalVars.discordRPCManager.Init(parent);
        }

        private void checkbox_enablediscord_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalVars.discordRPCManager.Close();
        }
    }
}
