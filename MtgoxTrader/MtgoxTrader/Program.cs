//****************************************************************************             
//
// @File: Program.cs
// @owner: iamapi 
//    
// Notes:
//	
// @EndHeader@
//****************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Resources;
using MtGoxTrader.Model;

namespace MtGoxTrader.Trader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        //static TraceSwitch tracer = new TraceSwitch("AgentTracer", "Local Agent Application"); 
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
            string culture = Consts.enLanguage;
            try
            {
                string configFolder = Path.Combine(Application.StartupPath, Consts.DataFolder);
                string settingsFileFullPath = Path.Combine(configFolder, Consts.SettingsFileName);
                Settings settings = (Settings)Utils.LoadFromFile(settingsFileFullPath, typeof(Settings));
                if (settings == null)
                {
                    settings = new Settings();
                    settings.RefreshTime = Consts.DefaultRefreshTime;
                    settings.ShowMinAmount = Consts.DefaultShowMinAmount;
                    settings.ShowOrderNO = Consts.DefaultShowOrderNo;
                    settings.WarnHighBuyPrice = Consts.DefaultHighestBuyPrice;
                    settings.WarnLowSellPrice = Consts.DefaultLowestSellPrice;
                    settings.Language = Consts.enLanguage;
                    Utils.SaveToFile(settings, settingsFileFullPath);
                }
                culture = settings.Language;
            }
            catch (Exception ex)
            {
            }

            if (culture == Consts.cnLanguage)
            {
                ResourceFactory.ResourceCulture = ResourceFactory.CnCultureInfo;
                ResourceFactory.ResourceMan = ResourceFactory.CnResourceManager;
                Thread.CurrentThread.CurrentUICulture = ResourceFactory.CnCultureInfo;
            }
            else
            {
                ResourceFactory.ResourceCulture = ResourceFactory.EnCultureInfo;
                ResourceFactory.ResourceMan = ResourceFactory.EnResourceManager;
                Thread.CurrentThread.CurrentUICulture = ResourceFactory.EnCultureInfo;
            }
            ResourceFactory.Culture = culture;           
                        
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string procName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            string logFolder = Path.Combine(Application.StartupPath, Consts.logFolder);
            Trace.Listeners.Add(new LogTraceListener(Path.Combine(logFolder, Consts.logFileName)));
            Trace.AutoFlush = true;
            if ((System.Diagnostics.Process.GetProcessesByName(procName)).GetUpperBound(0) > 0)
            {
                MessageBox.Show(ResourceFactory.GetString("AlreadyRunning"));
                return;
            }

            Configure config = new Configure();
            config.Show();
            Application.Run();
        }

        static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                Trace.WriteLine(string.Format("{0} \r\nstack:{1}", Utils.GetDetailedException(ex), Utils.GetStackTrace(ex)));
            }
        }
    }
}
