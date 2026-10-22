**Languages:** [English](Readme.md) · [Español](README.es.md)

# binancebotnew

Console tools for trading and monitoring Binance markets, with Telegram client integrations.

Documentation based on local files reviewed on 21 October 2026.

<!-- BEGIN project-context -->

## Historical dates and evidence

**Reference year for this version: 2021.** This uses the most recent source or configuration file with a recoverable historical date; the exact creation date could not be confirmed.

- Most recent datable file: **2021-02-07**, [`binancebotnew/BinanceBot.Common/TelegramReader.cs`](BinanceBot.Common/TelegramReader.cs); preserved modification timestamp.
- First record in the original Git repository containing this code: **2021-02-02**. This may be an import and does not establish when the code was first written.
- Most recently published dependency among those checked: [Binance.Net 6.6.4](https://www.nuget.org/packages/Binance.Net/6.6.4), published on **2021-01-22** (declared version), in [`binancebotnew/CryptoCoreNew/CryptoCoreNew.csproj`](CryptoCoreNew/CryptoCoreNew.csproj).
- Additional reference: target `netcoreapp3.1` in [`binancebotnew/BinanceBot.Common/BinanceBot.Common.csproj`](BinanceBot.Common/BinanceBot.Common.csproj); its [stable release was published on 2019-12-03](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core). This indicates the technology generation; earlier preview releases may have existed.

A dependency's publication places the preserved configuration at or after that version, if the reference is valid. Package updates may be much newer than the project's beginnings. Packages were not restored to verify this.

## Why this is worth exploring

Separating IExchange, the BinanceExchange adapter, and order book processors shows how to organize trading tools without coupling all logic to one provider. Volume and trade monitors demonstrate different consumers of the same market data.

Code entry points:

- [`CryptoCoreNew/IExchange.cs`](CryptoCoreNew/IExchange.cs)
- [`CryptoCoreNew/BinanceExchange.cs`](CryptoCoreNew/BinanceExchange.cs)
- [`BinanceBot.Common/OrderBookProcessor.cs`](BinanceBot.Common/OrderBookProcessor.cs)

<!-- END project-context -->

## Structure and responsibilities

| Path | Responsibility |
| --- | --- |
| `BinanceBotNew/` | Trading executable. |
| `BinanceDataDownloader/` | Market data download. |
| `DataMonitorOnly/, OrderBookMonitor/, PumpTradesAndOrderBookMonitor/, VolumeMonitorNew/` | Executables for monitoring prices, order books, trades, and volume. |
| `CryptoCoreNew/` | IExchange abstraction and BinanceExchange client, orders, balances, and fees. |
| `BinanceBot.Common/, Core.Common/` | Market processing, HTTP utilities, console tools, and logging. |
| `Mentor.telegram.client/` | OpenTL client and persistent Telegram sessions. |
| `LogReader/` | Auxiliary .NET Framework 4.5.2 tool for reading logs. |

## Technologies and requirements

.NET Core 3.1 for the main executables; Windows and .NET Framework 4.5.2 for LogReader. Relevant dependencies include Binance.Net 6.6.4, RestSharp, and OpenTL.

Targets read from project files: `netcoreapp3.1`, `v4.5.2`.

## Setup and startup

Choose an executable for the task you need. Review its Program.cs before using dotnet run: some operate on real accounts while others only monitor them.

Setup commands, run from this folder after reviewing private configuration:

```text
dotnet restore BinanceBot.sln
dotnet build BinanceBotNew/BinanceBotNew.csproj
```

Solutions present: `BinanceBot.sln`.

## Configuration and integrations

Several Program.cs files contain Binance keys. Mentor.telegram.client/ClientFactory.cs configures Telegram access, and session*.dat files preserve authenticated sessions. Replace these credentials and use your own sessions.

## Status of this copy

Publish profiles and sessions are local configuration. The review did not initiate connections to Binance or Telegram.

The review was static: dependencies were not restored, and applications and tests were neither built nor run. The commands above are setup instructions, not validation results. Listed versions describe the preserved code.


## Public copy

Original credentials have been removed or replaced with `REPLACE_WITH_YOUR_VALUE`. Local data, sessions, private certificates, and deployment profiles are omitted. Configure your own services and credentials before running. Building and operation with external services have not been validated.
