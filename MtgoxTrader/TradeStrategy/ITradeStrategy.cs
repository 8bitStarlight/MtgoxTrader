using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MtGoxTrader.MtGoxAPIClient;
using MtGoxTrader.Model;

namespace MtGoxTrader.TradeStrategy
{
    public interface ITradeStrategy
    {
        bool ComputeNewOrders(MtGoxDepthInfo depth, List<MtGoxTrade> tradeListInOneMin, List<MtGoxTrade> tradeListInFiveMin, MtGoxTickerItem ticker, UserInfo user, MtGoxAPIV0 api, List<AutoTradeSettings> autoTradeSettingsList);
    }
}
