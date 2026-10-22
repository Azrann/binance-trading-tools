**Languages:** [English](README.md) · [Español](README.es.md)

# binance-trading-tools

Historical bots and tools for monitoring and trading on Binance, with Telegram integration.

<!-- BEGIN project-context -->

## Historical dates and evidence

**Reference year for this version: 2021.** This uses the most recent source or configuration file with a recoverable historical date; the exact creation date could not be confirmed.

- Most recent datable file: **2021-02-07**, [`binancebotnew/BinanceBot.Common/TelegramReader.cs`](binancebotnew/BinanceBot.Common/TelegramReader.cs); preserved modification timestamp.
- Earliest record among the original Git repositories included: **2021-02-02**. This may be an import and does not establish when the code was first written.
- Most recently published dependency among those checked: [Binance.Net 6.6.4](https://www.nuget.org/packages/Binance.Net/6.6.4), published on **2021-01-22** (declared version), in [`binancebotnew/CryptoCoreNew/CryptoCoreNew.csproj`](binancebotnew/CryptoCoreNew/CryptoCoreNew.csproj).
- Additional reference: target `netcoreapp3.1` in [`binancebotnew/BinanceBot.Common/BinanceBot.Common.csproj`](binancebotnew/BinanceBot.Common/BinanceBot.Common.csproj); its [stable release was published on 2019-12-03](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core). This indicates the technology generation; earlier preview releases may have existed.

A dependency's publication places the preserved configuration at or after that version, if the reference is valid. Package updates may be much newer than the project's beginnings. Packages were not restored to verify this.

Reference years by component (most recent datable file):

| Component | Year |
| --- | --- |
| [`binanceBot`](binanceBot/) | 2018 |
| [`binancebotnew`](binancebotnew/) | 2021 |

## Why this is worth exploring

Compare a small buy/sell bot with a later organization of adapters, monitors, and market processors. The IExchange abstraction and order book processing are the most useful parts for studying the design.

Code entry points:

- [`binanceBot/BinanceBot/Binance.cs`](binanceBot/BinanceBot/Binance.cs)
- [`binancebotnew/CryptoCoreNew/IExchange.cs`](binancebotnew/CryptoCoreNew/IExchange.cs)
- [`binancebotnew/BinanceBot.Common/OrderBookProcessor.cs`](binancebotnew/BinanceBot.Common/OrderBookProcessor.cs)

<!-- END project-context -->

## Contents

| Directory | Function |
| --- | --- |
| [`binanceBot/`](binanceBot/) | Console bot for trading on Binance: buys at market price, places a limit sell order, and lets you manage or cancel the position from the keyboard. |
| [`binancebotnew/`](binancebotnew/) | Console tools for trading and monitoring Binance markets, with Telegram client integrations. |

binanceBot is the earlier variant. Some executables submit real orders: review Program.cs and use a test configuration before running them.

## Requirements and setup

.NET Framework and .NET Core 3.1; Binance.Net and OpenTL.

This is a historical copy. Consult each component's project files and README for versions and dependencies. Restore NuGet/npm from the appropriate folder; internal libraries may need rebuilding. Prepare databases and external services using your own accounts.

Preserved solutions:

- `binanceBot/BinanceBot.sln`
- `binancebotnew/BinanceBot.sln`

## Configuration and security

Original secrets have been removed or replaced with `REPLACE_WITH_YOUR_VALUE`. Search for this marker before running and provide values through local configuration or environment variables. Sessions, private keys, signing certificates, databases, logs, deployment profiles, and binary packages are not published. The `.gitignore` protects these local files.

A fresh history is published: old commits remain only in the original local copy. Example values cannot connect to the original services.

## Validation status

A static review and secret scan were performed before publication. Applications, trading operations, deployments, and tests contacting external services were not run. Building with current tools is not guaranteed. Some projects need historical dependencies and assets that are not included.

## Licenses and provenance

Third-party licenses and attributions present in the code are retained. No blanket license is added to components that lacked one. This import preserves local sources and does not claim to be a current version of the original project.
