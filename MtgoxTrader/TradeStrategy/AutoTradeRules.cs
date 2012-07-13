using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MtGoxTrader.MtGoxAPIClient;
using MtGoxTrader.Model;

namespace MtGoxTrader.TradeStrategy
{
    public class AutoTradeRules
    {
        public enum TradeRules
        {
            AutoTradePriceHigher = 0,
            AutoTradePriceLower = 1,            
            AutoTradeFiveMinIncreaseHigher = 2,
            AutoTradeFiveMinDropHigher = 3,
            AutoTradeFiveMinDealAmountHigher = 4, 
            AutoTradeFiveMinAmountPerTradeHigher = 5,
            AutoTradeOneMinIncreaseHigher = 6,
            AutoTradeOneMinDropHigher = 7,
            AutoTradeOneMinDealAmountHigher = 8, 
            AutoTradeOneMinAmountPerTradeHigher = 9,
            AutoTradeSellAmountIn10Higher = 10, 
            AutoTradeBuyAmountIn10Higher = 11            
        }
    }

    public interface IAutoTradeRule
    {
        bool ShouldExecute(MtGoxDepthInfo depth, List<MtGoxTrade> tradeListInOneMin, List<MtGoxTrade> tradeListInFiveMin, MtGoxTickerItem ticker, double condition);
    }

    public class AutoTradeRuleFactory
    {
        public static IAutoTradeRule CreateAutoTradeRule(int index)
        {
            AutoTradeRules.TradeRules rule = (AutoTradeRules.TradeRules)Enum.ToObject(typeof(AutoTradeRules.TradeRules), index);
            switch (rule)
            {
                case AutoTradeRules.TradeRules.AutoTradePriceHigher:
                    return new AutoTradePriceHigher();
                case AutoTradeRules.TradeRules.AutoTradePriceLower:
                    return new AutoTradePriceLower();
                case AutoTradeRules.TradeRules.AutoTradeFiveMinIncreaseHigher:
                    return new AutoTradeFiveMinIncreaseHigher();
                case AutoTradeRules.TradeRules.AutoTradeFiveMinDropHigher:
                    return new AutoTradeFiveMinDropHigher();
                case AutoTradeRules.TradeRules.AutoTradeFiveMinDealAmountHigher:
                    return new AutoTradeFiveMinDealAmountHigher();
                case AutoTradeRules.TradeRules.AutoTradeFiveMinAmountPerTradeHigher:
                    return new AutoTradeFiveMinAmountPerTradeHigher();
                case AutoTradeRules.TradeRules.AutoTradeOneMinIncreaseHigher:
                    return new AutoTradeOneMinIncreaseHigher();
                case AutoTradeRules.TradeRules.AutoTradeOneMinDropHigher:
                    return new AutoTradeOneMinDropHigher();
                case AutoTradeRules.TradeRules.AutoTradeOneMinDealAmountHigher:
                    return new AutoTradeOneMinDealAmountHigher();
                case AutoTradeRules.TradeRules.AutoTradeOneMinAmountPerTradeHigher:
                    return new AutoTradeOneMinAmountPerTradeHigher();
                case AutoTradeRules.TradeRules.AutoTradeSellAmountIn10Higher:
                    return new AutoTradeSellAmountIn10Higher();
                case AutoTradeRules.TradeRules.AutoTradeBuyAmountIn10Higher:
                    return new AutoTradeBuyAmountIn10Higher();
                default:
                    break;
            }
            return null;
        }
    }


    public class AutoTradePriceHigher : IAutoTradeRule
    {
        public bool ShouldExecute(MtGoxDepthInfo depth, List<MtGoxTrade> tradeListInOneMin, List<MtGoxTrade> tradeListInFiveMin, MtGoxTickerItem ticker, double condition)
        {
            return ticker.last > condition;
        }
    }

    public class AutoTradePriceLower : IAutoTradeRule
    {
        public bool ShouldExecute(MtGoxDepthInfo depth, List<MtGoxTrade> tradeListInOneMin, List<MtGoxTrade> tradeListInFiveMin, MtGoxTickerItem ticker, double condition)
        {
            return ticker.last < condition;
        }
    }

    public class AutoTradeFiveMinIncreaseHigher : IAutoTradeRule
    {
        public bool ShouldExecute(MtGoxDepthInfo depth, List<MtGoxTrade> tradeListInOneMin, List<MtGoxTrade> tradeListInFiveMin, MtGoxTickerItem ticker, double condition)
        {
            if (tradeListInFiveMin != null && tradeListInFiveMin.Count >= 2)
            {
                return (ticker.last - tradeListInFiveMin[0].price) * 100 / tradeListInFiveMin[0].price > condition;
            }
            return false;
        }
    }

    public class AutoTradeFiveMinDropHigher : IAutoTradeRule
    {
        public bool ShouldExecute(MtGoxDepthInfo depth, List<MtGoxTrade> tradeListInOneMin, List<MtGoxTrade> tradeListInFiveMin, MtGoxTickerItem ticker, double condition)
        {
            if (tradeListInFiveMin != null && tradeListInFiveMin.Count >= 2)
            {
                return (tradeListInFiveMin[0].price - ticker.last) * 100 / tradeListInFiveMin[0].price > condition;
            }
            return false;
        }
    }

    public class AutoTradeFiveMinDealAmountHigher : IAutoTradeRule
    {
        public bool ShouldExecute(MtGoxDepthInfo depth, List<MtGoxTrade> tradeListInOneMin, List<MtGoxTrade> tradeListInFiveMin, MtGoxTickerItem ticker, double condition)
        {
            if (tradeListInFiveMin != null)
            {
                double amount = 0;
                foreach (MtGoxTrade trade in tradeListInFiveMin)
                {
                    amount += trade.amount;
                }
                return amount > condition;
            }
            return false;
        }
    }

    public class AutoTradeFiveMinAmountPerTradeHigher : IAutoTradeRule
    {
        public bool ShouldExecute(MtGoxDepthInfo depth, List<MtGoxTrade> tradeListInOneMin, List<MtGoxTrade> tradeListInFiveMin, MtGoxTickerItem ticker, double condition)
        {
            if (tradeListInFiveMin != null)
            {
                foreach (MtGoxTrade trade in tradeListInFiveMin)
                {
                    if (trade.amount > condition)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class AutoTradeOneMinIncreaseHigher : IAutoTradeRule
    {
        public bool ShouldExecute(MtGoxDepthInfo depth, List<MtGoxTrade> tradeListInOneMin, List<MtGoxTrade> tradeListInFiveMin, MtGoxTickerItem ticker, double condition)
        {
            if (tradeListInOneMin != null && tradeListInOneMin.Count >= 2)
            {
                return (ticker.last - tradeListInOneMin[0].price) * 100 / tradeListInOneMin[0].price > condition;
            }
            return false;
        }
    }

    public class AutoTradeOneMinDropHigher : IAutoTradeRule
    {
        public bool ShouldExecute(MtGoxDepthInfo depth, List<MtGoxTrade> tradeListInOneMin, List<MtGoxTrade> tradeListInFiveMin, MtGoxTickerItem ticker, double condition)
        {
            if (tradeListInOneMin != null && tradeListInOneMin.Count >= 2)
            {
                return (tradeListInOneMin[0].price - ticker.last) * 100 / tradeListInOneMin[0].price > condition;
            }
            return false;
        }
    }

    public class AutoTradeOneMinDealAmountHigher : IAutoTradeRule
    {
        public bool ShouldExecute(MtGoxDepthInfo depth, List<MtGoxTrade> tradeListInOneMin, List<MtGoxTrade> tradeListInFiveMin, MtGoxTickerItem ticker, double condition)
        {
            if (tradeListInOneMin != null)
            {
                double amount = 0;
                foreach (MtGoxTrade trade in tradeListInOneMin)
                {
                    amount += trade.amount;
                }
                return amount > condition;
            }
            return false;
        }
    }

    public class AutoTradeOneMinAmountPerTradeHigher : IAutoTradeRule
    {
        public bool ShouldExecute(MtGoxDepthInfo depth, List<MtGoxTrade> tradeListInOneMin, List<MtGoxTrade> tradeListInFiveMin, MtGoxTickerItem ticker, double condition)
        {
            if (tradeListInOneMin != null)
            {
                foreach (MtGoxTrade trade in tradeListInOneMin)
                {
                    if (trade.amount > condition)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class AutoTradeSellAmountIn10Higher : IAutoTradeRule
    {
        public bool ShouldExecute(MtGoxDepthInfo depth, List<MtGoxTrade> tradeListInOneMin, List<MtGoxTrade> tradeListInFiveMin, MtGoxTickerItem ticker, double condition)
        {
            if (depth != null && depth.asks.Count > 10)
            {
                double dAmount = 0;
                int index = 0;
                foreach (MtGoxAsk ask in depth.asks)
                {
                    if (++index > 10)
                        break;
                    dAmount += ask.amount;
                }
                return dAmount > condition;
            }
            return false;
        }
    }

    public class AutoTradeBuyAmountIn10Higher : IAutoTradeRule
    {
        public bool ShouldExecute(MtGoxDepthInfo depth, List<MtGoxTrade> tradeListInOneMin, List<MtGoxTrade> tradeListInFiveMin, MtGoxTickerItem ticker, double condition)
        {
            if (depth != null && depth.bids.Count > 10)
            {
                double dAmount = 0;
                int index = 0;
                foreach (MtGoxBid bid in depth.bids)
                {
                    if (++index > 10)
                        break;
                    dAmount += bid.amount;
                }
                return dAmount > condition;
            }
            return false;
        }
    }    
}
