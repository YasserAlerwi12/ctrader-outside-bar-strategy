using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SimpleOutsideBarStrategy : Robot
    {
        [Parameter("Position Volume", DefaultValue = 0.1)]
        public double PositionVolume { get; set; }

        [Parameter("Stop Loss (Pips)", DefaultValue = 250)]
        public double StopLossPips { get; set; }

        [Parameter("Timeframe", Group = "Trading Parameters")]
        public TimeFrame TradeTimeframe { get; set; }

        private bool isNewBar = false;
        private DateTime lastBarOpenTime;

        protected override void OnStart()
        {
            // Set default timeframe if not specified
            if (TradeTimeframe == null)
                TradeTimeframe = TimeFrame.Hour4;

            lastBarOpenTime = MarketData.GetBars(TradeTimeframe).Last(0).OpenTime;
            Print("Robot started. Monitoring for Outside Bar patterns...");
        }

        protected override void OnTick()
        {
            // Check for new bar
            var currentBar = MarketData.GetBars(TradeTimeframe).Last(0);
            if (currentBar.OpenTime != lastBarOpenTime)
            {
                isNewBar = true;
                lastBarOpenTime = currentBar.OpenTime;
            }

            if (!isNewBar) return;
            isNewBar = false;

            // Get current position
            var position = Positions.Find(Symbol.Name, SymbolName);
            
            // If we have a winning position, close it
            if (position != null && position.NetProfit > 0)
            {
                ClosePosition(position);
                return;
            }

            // Check if we can place new position
            if (position != null)
                return;

            // Get the last two completed bars
            var bars = MarketData.GetBars(TradeTimeframe);
            var currentCompletedBar = bars.Last(1);
            var previousBar = bars.Last(2);

            // Print bar information for debugging
            Print($"Checking bars - Current Bar: O:{currentCompletedBar.Open} H:{currentCompletedBar.High} L:{currentCompletedBar.Low} C:{currentCompletedBar.Close}");
            Print($"Previous Bar: O:{previousBar.Open} H:{previousBar.High} L:{previousBar.Low} C:{previousBar.Close}");

            // Check for long signal
            if (IsLongSignal(currentCompletedBar, previousBar))
            {
                Print("Long signal detected!");
                ExecuteMarketOrder(TradeType.Buy, SymbolName, PositionVolume, "Long Entry", StopLossPips, null);
            }
            // Check for short signal
            else if (IsShortSignal(currentCompletedBar, previousBar))
            {
                Print("Short signal detected!");
                ExecuteMarketOrder(TradeType.Sell, SymbolName, PositionVolume, "Short Entry", StopLossPips, null);
            }
        }

        private bool IsLongSignal(Bar currentBar, Bar previousBar)
        {
            bool c0 = currentBar.Open > currentBar.Close; // Bearish candle
            bool c1 = currentBar.High > previousBar.High; // High is greater than previous high
            bool c2 = currentBar.Low < previousBar.Low;   // Low is less than previous low
            bool c3 = currentBar.Close < previousBar.Low; // Close is less than previous low

            if (c0 && c1 && c2 && c3)
            {
                Print($"Long conditions met: Bearish:{c0} HigherHigh:{c1} LowerLow:{c2} CloseBelowPrevLow:{c3}");
                return true;
            }
            return false;
        }

        private bool IsShortSignal(Bar currentBar, Bar previousBar)
        {
            bool c0 = currentBar.Open < currentBar.Close; // Bullish candle
            bool c1 = currentBar.Low < previousBar.Low;   // Low is less than previous low
            bool c2 = currentBar.High > previousBar.High; // High is greater than previous high
            bool c3 = currentBar.Close > previousBar.High;// Close is greater than previous high

            if (c0 && c1 && c2 && c3)
            {
                Print($"Short conditions met: Bullish:{c0} LowerLow:{c1} HigherHigh:{c2} CloseAbovePrevHigh:{c3}");
                return true;
            }
            return false;
        }

        protected override void OnStop()
        {
            Print("Robot stopped");
        }
    }
}
