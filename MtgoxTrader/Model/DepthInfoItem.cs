//****************************************************************************             
//
// @File: DepthInfoItem.cs
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
    public class DepthInfoItem
    {
        public double Price { get; set; }
        public double Amount { get; set; }
        public int ItemNO { get; set; }
        public double BtcCount { get; set; }
        public double FundCount { get; set; }        
    }

    public class DepthInfoGroup
    {
        public List<DepthInfoItem> asks { get; set; }

        public List<DepthInfoItem> bids { get; set; }

        public DepthInfoGroup()
        {
            this.asks = new List<DepthInfoItem>();
            this.bids = new List<DepthInfoItem>();
        }
    }
}
