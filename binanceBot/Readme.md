**Languages:** [English](Readme.md) · [Español](README.es.md)

# binanceBot

Console bot for trading on Binance: buys at market price, places a limit sell order, and lets you manage or cancel the position from the keyboard.

Documentation based on local files reviewed on 21 October 2026.

<!-- BEGIN project-context -->

## Historical dates and evidence

**Reference year for this version: 2018.** This uses the most recent source or configuration file with a recoverable historical date; the exact creation date could not be confirmed.

- Most recent datable file: **2018-01-09**, [`binanceBot/BinanceBot/Binance.cs`](BinanceBot/Binance.cs); preserved modification timestamp.
- Most recently published dependency among those checked: [RestSharp 106.2.1](https://www.nuget.org/packages/RestSharp/106.2.1), published on **2018-01-06** (declared version), in [`binanceBot/BinanceBot/packages.config`](BinanceBot/packages.config).
- Additional reference: target `netcoreapp1.1` in [`binanceBot/Binance/Binance.csproj`](Binance/Binance.csproj); its [stable release was published on 2016-11-16](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core). This indicates the technology generation; earlier preview releases may have existed.

A dependency's publication places the preserved configuration at or after that version, if the reference is valid. Package updates may be much newer than the project's beginnings. Packages were not restored to verify this.

## Why this is worth exploring

The order flow is easy to follow: buy at market price, prepare a limit sell order, and manage it through console controls. This is a small example for studying an order's lifecycle and its HTTP integration with an exchange.

Code entry points:

- [`BinanceBot/Binance.cs`](BinanceBot/Binance.cs)
- [`BinanceBot/Program.cs`](BinanceBot/Program.cs)

<!-- END project-context -->

## Structure and responsibilities

| Path | Responsibility |
| --- | --- |
| `BinanceBot/` | Main .NET Framework 4.6.2 application; Program.cs starts buying, selling, and tracking. |
| `Binance/` | Another executable targeting .NET Core 1.1. |
| `BinanceCore/` | .NET Standard 1.4 library with minimal content. |
| `BinanceBot/TickerRules.cs` | Precision and quantity rules for each trading pair. |

## Technologies and requirements

Windows, Visual Studio with .NET Framework 4.6.2 tools, and NuGet. The Binance variant requires the historical .NET Core 1.1 SDK. It uses Binance.API.Csharp.Client and Newtonsoft.Json.

Targets read from project files: `netcoreapp1.1`, `netstandard1.4`, `v4.6.2`.

## Setup and startup

Open BinanceBot.sln, restore packages, and select BinanceBot as the startup project. Review Binance.cs and the rules before starting: Main can immediately submit real orders.

Solutions present: `BinanceBot.sln`.

## Configuration and integrations

Credentials are embedded in BinanceBot/Binance.cs. Capital, trading pairs, and strategy are also set in code. Move keys to private configuration before rebuilding the project.

## Status of this copy

This is an order execution prototype; compatibility with the current Binance API has not been tested. It was not run during this review.

The review was static: dependencies were not restored, and applications and tests were neither built nor run. The commands above are setup instructions, not validation results. Listed versions describe the preserved code.


## Public copy

Original credentials have been removed or replaced with `REPLACE_WITH_YOUR_VALUE`. Local data, sessions, private certificates, and deployment profiles are omitted. Configure your own services and credentials before running. Building and operation with external services have not been validated.
