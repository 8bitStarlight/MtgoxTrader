//****************************************************************************             
//
// @File: StopOrder.cs
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
using System.IO;
using System.Runtime.Serialization;
using System.Security.Principal;


namespace MtGoxTrader.Model
{
    [DataContract]
    public class StopOrder
    {
        public StopOrder()
        {
            OrderId = Guid.NewGuid();
        }
        public StopOrder(StopOrder order)
        {
            OrderId = order.OrderId;
            this.Amount = order.Amount;
            this.Currency = order.Currency;
            this.ExecuteTime = order.ExecuteTime;
            this.OrderTime = order.OrderTime;
            this.Price = order.Price;
            this.Status = order.Status;
            this.Type = order.Type;
        }
        public double orderTol = 0.1;
        public enum OrderStatus
        {
            Pending,
            Executed
        }
        public enum OrderType
        {
            BuyStop,
            SellStop
        }
        [DataMember]
        public Guid OrderId { get; set; }
        [DataMember]
        public DateTime OrderTime { get; set; }
        [DataMember]
        public double Amount { get; set; }
        [DataMember]
        public double Price { get; set; }
        [DataMember]
        public OrderType Type { get; set; }
        [DataMember]
        public OrderStatus Status { get; set; }
        [DataMember]
        public DateTime ExecuteTime { get; set; }
        [DataMember]
        public MtGoxCurrencySymbol Currency { get; set; }

        public virtual bool ShouldExecute(double currentSell, double currentBuy)
        {           
            return false;
        }
        public virtual bool Execute(double currentSell, double currentBuy)
        {            
            return false;
        }

    } 
}
