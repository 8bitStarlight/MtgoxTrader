//****************************************************************************             
//
// @File: HistoryInfo.cs
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
using MtGoxTrader.MtGoxAPIClient;
using LumenWorks.Framework.IO.Csv;

namespace MtGoxTrader.Trader
{
    public class HistoryInfo
    {
        public string History {get;set;}
        public double TotalEarn {get; set;}
        public double TotalSpent { get; set; }
        public double TotalDeposit { get; set; }
        public double TotalWithdraw { get; set; }        
    }

    public class HistoryHelper
    {
        public static void analyzeHistory(ref HistoryInfo info)
        {
            using (TextReader textReader = new StringReader(info.History))
            {
                CsvReader reader = new CsvReader(textReader, true);
                CsvReader.RecordEnumerator record = reader.GetEnumerator();
                double spent = 0.0, earn = 0.0, deposit = 0.0, withdraw = 0.0;                
                double temp = 0.0;
                while (record.MoveNext())
                {
                    if (record.Current[2] == "spent")
                    {
                        if (Double.TryParse(record.Current[4], out temp))
                            spent += temp;
                    }
                    else if (record.Current[2] == "fee")
                    {
                        if (Double.TryParse(record.Current[4], out temp))
                            spent += temp;
                    }
                    else if (record.Current[2] == "earned")
                    {
                        if (Double.TryParse(record.Current[4], out temp))
                            earn += temp;
                    }
                    else if (record.Current[2] == "deposit")
                    {
                        if (Double.TryParse(record.Current[4], out temp))
                            deposit += temp;
                    }
                    else if (record.Current[2] == "withdraw")
                    {
                        if (Double.TryParse(record.Current[4], out temp))
                            withdraw += temp;
                    }
                }
                info.TotalDeposit = deposit;
                info.TotalEarn = earn;
                info.TotalSpent = spent;
                info.TotalWithdraw = withdraw;
            }
        }
    }
}
