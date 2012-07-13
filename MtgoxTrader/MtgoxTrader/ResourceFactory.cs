//****************************************************************************             
//
// @File: ResourceFactory.cs
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
using System.Resources;
using System.Globalization;

namespace MtGoxTrader.Trader
{
    public class ResourceFactory
    {
        public static ResourceManager ResourceMan{get; set;}
        public static CultureInfo ResourceCulture{get; set;}
        public static string Culture { get; set; }
        public static ResourceManager EnResourceManager = new ResourceManager("MtGoxTrader.Trader.Strings.en.us", typeof(Strings_en_us).Assembly);
        public static ResourceManager CnResourceManager = new ResourceManager("MtGoxTrader.Trader.Strings.zh.cn", typeof(Strings_zh_cn).Assembly);
        public static CultureInfo EnCultureInfo = CultureInfo.GetCultureInfo("en-us");
        public static CultureInfo CnCultureInfo = CultureInfo.GetCultureInfo("zh-cn");
        private static ResourceFactory resourceFactory;

        public static ResourceFactory Instance()
        {
            if (resourceFactory == null)
            {
                resourceFactory = new ResourceFactory();
            }
            return resourceFactory;
        }

        private ResourceFactory()
        {
            
        }

        public static string GetString(string name)
        {
            return ResourceMan.GetString(name, ResourceCulture);
        }
    }
}
