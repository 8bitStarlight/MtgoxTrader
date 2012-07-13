//****************************************************************************             
//
// @File: Consts.cs
// @owner: iamapi 
//    
// Notes:
//	
// @EndHeader@
//****************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MtGoxTrader.Trader
{
    public class Consts
    {
        public static int DefaultShowOrderNo = 20;

        public static string SoundFolder = "sound";
        public static string[] SoundTypes = { ".wav", ".mp3" };

        public static string DataFolder = "data";

        public static string logFolder = "log";
        public static string logFileName = "logs.log";

        public static string StopOrderFileName = "orders.dat";

        public static string ConfigFileName = "config.dat";

        public static double MarketTol = 0.1;

        public static int DefaultRefreshTime = 10;

        public static double DefaultShowMinAmount = 1;

        public static string supportMail = "mtgoxtrade@gmail.com";

        public static double DefaultHighestBuyPrice = 100;

        public static double DefaultLowestSellPrice = 1;

        public static string SettingsFileName = "user.settings";

        public static string cnLanguage = "zh-cn";
        public static string enLanguage = "en-us";
        
    }
}
