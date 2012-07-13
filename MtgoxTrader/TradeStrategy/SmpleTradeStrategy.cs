using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MtGoxTrader.MtGoxAPIClient;
using MtGoxTrader.Model;
using System.IO;
using System.Media;
using System.Reflection;

namespace MtGoxTrader.TradeStrategy
{
    public class SmpleTradeStrategy : ITradeStrategy
    {
        public double OrderTol = 0.1;
        public string SoundFolder { get; set; }
        public bool ComputeNewOrders(MtGoxDepthInfo depth, List<MtGoxTrade> tradeListInOneMin, List<MtGoxTrade> tradeListInFiveMin, MtGoxTickerItem ticker, UserInfo user, MtGoxAPIV0 api, List<AutoTradeSettings> autoTradeSettingsList)
        {
            try
            {
                double buyPrice = depth.asks[0].price + OrderTol;
                double sellPrice = depth.bids[0].price - OrderTol;
                int index = 0;
                foreach (AutoTradeSettings autoTrade in autoTradeSettingsList)
                {
                    index++;
                    if (autoTrade.Status == AutoTradeSettings.OrderStatus.Executed)
                        continue;
                    bool execute = false;                    

                    foreach (RuleSettings rule in autoTrade.Rules)
                    {
                        IAutoTradeRule tradeRule = AutoTradeRuleFactory.CreateAutoTradeRule(rule.RuleIndex);
                        if (tradeRule != null)
                        {
                            execute = tradeRule.ShouldExecute(depth, tradeListInOneMin, tradeListInFiveMin, ticker, rule.RuleCondition);
                            if (!execute)
                            {
                                break;
                            }
                        }
                    }
                    if (execute)
                    {
                        if (autoTrade.Warn)
                        {
                            try
                            {
                                SoundPlayer player = new SoundPlayer(Path.Combine(SoundFolder, autoTrade.Sound));
                                player.Play();
                            }
                            catch
                            {
                            }
                        }
                        if (autoTrade.Trade)
                        {
                            if (autoTrade.TradeType == AutoTradeSettings.OrderType.Sell)
                            {
                                List<MtGoxOrder> order = api.sellBTC(autoTrade.TradeAmount, user.Currency, sellPrice);
                            }
                            else
                            {
                                List<MtGoxOrder> order = api.buyBTC(autoTrade.TradeAmount, user.Currency, buyPrice);
                            }
                            autoTradeSettingsList[index - 1].ExecuteTime = System.DateTime.Now;
                            autoTradeSettingsList[index - 1].Status = AutoTradeSettings.OrderStatus.Executed;
                        }
                    }
                }
             
            }
            catch(Exception e)
            {
                throw;
            }
            return true;
        }
    }
}
