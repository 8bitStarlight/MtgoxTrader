//****************************************************************************             
//
// @File: MtGoxOrder.cs
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
    public class MtGoxOrder
    {

        public string oid { get; set; }

        public MtGoxCurrencySymbol currency { get; set; }
            
        public MtGoxCurrencySymbol item { get; set; }
        
        public MtGoxOrderType type  { get; set; }
            
        public double amount { get; set; }
        
        public Int64 amount_int { get; set; }
            
        public double price { get; set; }
        
        public Int64 price_int { get; set; }
            
        public MtGoxOrderStatus status { get; set; }
        
        public string real_status { get; set; }
            
        public int dark { get; set; }
            
        public int date { get; set; }
        
        public Int64 priority { get; set; }

        /// <summary>
        ///  Parse JSON data returned by the 0/buyBTC.php and 0/sellBTC.php methods
        /// </summary>       
        public static List<MtGoxOrder> getObjects(string jsonDataStr)
        {
            List<MtGoxOrder> orderList = new List<MtGoxOrder>();
            string json = jsonDataStr;
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic obj = serializer.Deserialize(json, typeof(object));
           
            for (int i = 0; i < obj.orders.Count; i++)
            {
                MtGoxOrder ord = new MtGoxOrder();
                ord.oid = obj.orders[i].oid.ToString();
                ord.currency = (MtGoxCurrencySymbol)Enum.Parse(typeof(MtGoxCurrencySymbol), obj.orders[i].currency.ToString(), true);
                ord.item = (MtGoxCurrencySymbol)Enum.Parse(typeof(MtGoxCurrencySymbol), obj.orders[i].item.ToString(), true);
                switch ((int)int.Parse(obj.orders[i].type.ToString()))
                {
                    case 1:
                        ord.type = MtGoxOrderType.Sell;
                        break;
                    case 2:
                        ord.type = MtGoxOrderType.Buy;
                        break;
                }
                ord.amount = double.Parse(obj.orders[i].amount.ToString());
                ord.amount_int = Int64.Parse(obj.orders[i].amount_int.ToString());
                ord.price = double.Parse(obj.orders[i].price.ToString());
                ord.price_int = Int64.Parse(obj.orders[i].price_int.ToString());
                switch ((int)int.Parse(obj.orders[i].status.ToString()))
                {
                    case 0:
                        ord.status = MtGoxOrderStatus.Invalid;
                        break;
                    case 2:
                        ord.status = MtGoxOrderStatus.Invalid;
                        break;
                }
                ord.real_status = obj.orders[i].real_status.ToString();
                ord.dark = int.Parse(obj.orders[i].dark.ToString());
                ord.date = int.Parse(obj.orders[i].date.ToString());
                ord.priority = Int64.Parse(obj.orders[i].priority.ToString());
                orderList.Add(ord);
            }
            return orderList;
        }
    }
}
