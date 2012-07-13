//****************************************************************************             
//
// @File: MtGoxBid.cs
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

namespace MtGoxTrader.Model
{   
    public class MtGoxBid
    {
        public double price { get; set; }

        public double amount { get; set; }

        public MtGoxBid()
        { }

        public MtGoxBid(double price, double amount)
        { 
            this.price = price;
            this.amount = amount;
        }
    }
}
