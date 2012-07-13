//****************************************************************************             
//
// @File: MtGoxTrade.cs
// @owner: iamapi 
//    
// Notes:
//	
// @EndHeader@
//****************************************************************************
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace MtGoxTrader.Model
{    
    public class MtGoxTrade
    {
        public int date { get; set; }

        public double price { get; set; }

        public double amount { get; set; }

        public Int64 price_int { get; set; }

        public Int64 amount_int { get; set; }

        public string tid { get; set; }

        public MtGoxCurrencySymbol price_currency { get; set; }

        public string item { get; set; }

        public MtGoxTradeType trade_type { get; set; }

        public string primary { get; set; }

        /// <summary>
        /// Parses the JSON data returned by the 0/data/getTrades.php method
        /// </summary>        
        public static List<MtGoxTrade> getObjects(string jsonDataStr)
        {
            List<MtGoxTrade> tradeList = new List<MtGoxTrade>();
            string json = jsonDataStr;
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic obj = serializer.Deserialize(json, typeof(object));
            for (int i = 0; i < obj.Length; i++)
            {
                MtGoxTrade trade = new MtGoxTrade();
                trade.date = obj[i].date;
                trade.price = Double.Parse(obj[i].price);
                trade.amount = Double.Parse(obj[i].amount);
                trade.price_int = Int64.Parse(obj[i].price_int);
                trade.amount_int = Int64.Parse(obj[i].amount_int);
                trade.tid = obj[i].tid;
                if (Enum.IsDefined(typeof(MtGoxCurrencySymbol), obj[i].price_currency))
                    trade.price_currency = (MtGoxCurrencySymbol)Enum.Parse(typeof(MtGoxCurrencySymbol), obj[i].price_currency, true);
                trade.item = obj[i].item;
                if (Enum.IsDefined(typeof(MtGoxTradeType), obj[i].trade_type))
                    trade.trade_type = (MtGoxTradeType)Enum.Parse(typeof(MtGoxTradeType), obj[i].trade_type, true);
                trade.primary = obj[i].primary;
                tradeList.Add(trade);
                if (i > 100)
                    break;
            }
            return tradeList;
        }
    }
}
