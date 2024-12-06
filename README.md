# Outside Bar Trading Strategy for cTrader

## Description
This is an automated trading robot for cTrader that implements the Outside Bar trading strategy. The strategy looks for specific candlestick patterns to identify potential trading opportunities in the forex market.

## Features
- Implements Outside Bar pattern recognition
- Supports multiple timeframes (default: H4)
- Automated stop-loss management
- Immediate profit-taking on winning trades
- Configurable position size
- Detailed logging for strategy analysis

## Parameters
- **Position Volume**: Trading volume size (default: 0.1)
- **Stop Loss**: Stop loss in pips (default: 250)
- **Timeframe**: Trading timeframe (default: H4)

## Trading Logic
### Long Entry Conditions
1. Current candle is bearish (Open > Close)
2. Current high is greater than previous high
3. Current low is less than previous low
4. Current close is less than previous low

### Short Entry Conditions
1. Current candle is bullish (Open < Close)
2. Current low is less than previous low
3. Current high is greater than previous high
4. Current close is greater than previous high

## Installation
1. Open cTrader
2. Create a new Custom Robot
3. Copy the code from `SimpleOutsideBarStrategy.cs`
4. Compile the robot
5. Test using the cTrader backtester

## Usage
1. Apply the robot to a chart
2. Configure the parameters according to your risk preferences
3. Enable AutoTrading if trading live

## Requirements
- cTrader Platform
- Valid cTrader account (demo or live)

## Disclaimer
This trading robot is for educational purposes only. Always test thoroughly on a demo account before using with real money. Trading involves risk of loss.
