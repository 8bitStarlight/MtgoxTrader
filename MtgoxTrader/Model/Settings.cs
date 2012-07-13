//****************************************************************************             
//
// @File: Settings.cs
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
using System.Runtime.Serialization;

namespace MtGoxTrader.Model
{
    [DataContract]
    public class Settings
    {
        [DataMember]
        public double WarnLowSellPrice { get; set; }
        [DataMember]
        public double WarnHighBuyPrice { get; set; }
        [DataMember]
        public int RefreshTime { get; set; }
        [DataMember]
        public double ShowMinAmount { get; set; }
        [DataMember]
        public int ShowOrderNO { get; set; }
        [DataMember]
        public string Language { get; set; }
        [DataMember]
        public List<AutoTradeSettings> AutoTrade { get; set; }
        public Settings()
        {
            AutoTrade = new List<AutoTradeSettings>();
        }
    }

    [DataContract]
    public class AutoTradeSettings
    {
        public enum OrderStatus
        {
            Pending,
            Executed
        }
        public enum OrderType
        {
            Buy,
            Sell
        }
        [DataMember]
        public bool Warn { get; set; }
        [DataMember]
        public string Sound { get; set; }
        [DataMember]
        public Guid OrderId { get; set; }
        [DataMember]
        public DateTime OrderTime { get; set; }
        [DataMember]
        public bool Trade { get; set; }
        [DataMember]
        public double TradeAmount { get; set; }
        [DataMember]
        public OrderType TradeType { get; set; }
        [DataMember]
        public OrderStatus Status { get; set; }
        [DataMember]
        public DateTime ExecuteTime { get; set; }
        [DataMember]
        public List<RuleSettings> Rules { get; set; }
        public AutoTradeSettings()
        {
            OrderId = Guid.NewGuid();
            Rules = new List<RuleSettings>();
        }
    }

    [DataContract]
    public class RuleSettings
    {
        [DataMember]
        public int RuleIndex { get; set; }
        [DataMember]
        public double RuleCondition { get; set; }
    }
}
