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
using MtGoxTrader.MtGoxAPIClient;
using MtGoxTrader.Model;

namespace MtGoxTrader.Trader
{  
    [DataContract]
    public class SellStopOrder : StopOrder
    {
        public SellStopOrder(StopOrder order) : base(order)
        {

        }
        public override bool ShouldExecute(double currentSell, double currentBuy)
        {
            bool returnValue = false;
            if (Status == OrderStatus.Executed)
                return returnValue;
            if ((currentSell - orderTol <= Price) || (currentBuy  < Price))
                returnValue = true;
            return returnValue;
        }
        public override bool Execute(double currentSell, double currentBuy)
        {
            bool returnValue = false;
            if(!ShouldExecute(currentSell, currentBuy))
                return returnValue;
            List<MtGoxOrder> order = Form1.mtgoxV0.sellBTC(Amount, Currency, currentBuy - orderTol);
            if (order != null)
            {                
                returnValue = true;
            }
            return returnValue;
        }
    }
}
