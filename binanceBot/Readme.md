# binanceBot

Bot de consola para operar en Binance: compra a mercado, coloca una venta limitada y permite gestionar o cancelar la posición desde el teclado.

Documentación basada en los archivos locales revisados el 21 de octubre de 2026.

<!-- BEGIN project-context -->

## Antigüedad y evidencia

**Año de referencia de esta versión: 2018.** Se toma el archivo de código o configuración más reciente con una fecha histórica recuperable; no se ha podido confirmar la fecha exacta de creación.

- Archivo más reciente datable: **2018-01-09**, [`binanceBot/BinanceBot/Binance.cs`](BinanceBot/Binance.cs); fecha de modificación conservada.
- Dependencia fechada más reciente entre las comprobadas: [RestSharp 106.2.1](https://www.nuget.org/packages/RestSharp/106.2.1), publicada el **2018-01-06** (versión declarada), en [`binanceBot/BinanceBot/packages.config`](BinanceBot/packages.config).
- Referencia adicional: destino `netcoreapp1.1` en [`binanceBot/Binance/Binance.csproj`](Binance/Binance.csproj); su [versión estable se publicó el 2016-11-16](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core). Es una pista de generación tecnológica; pudieron existir versiones preliminares anteriores.

La publicación de una dependencia sitúa la configuración conservada a partir de esa versión, si la referencia es válida. Una actualización de paquetes puede ser mucho más reciente que el inicio del proyecto. No se restauraron paquetes para comprobarlo.

## Por qué merece la pena mirarlo

El flujo de una operación es fácil de seguir: compra a mercado, prepara una venta limitada y ofrece controles de consola para gestionarla. Es un ejemplo pequeño para estudiar el ciclo de una orden y su integración HTTP con un exchange.

Puntos de entrada al código:

- [`BinanceBot/Binance.cs`](BinanceBot/Binance.cs)
- [`BinanceBot/Program.cs`](BinanceBot/Program.cs)

<!-- END project-context -->

## Estructura y responsabilidades

| Ruta | Responsabilidad |
| --- | --- |
| `BinanceBot/` | Aplicación principal para .NET Framework 4.6.2; Program.cs inicia la compra, venta y seguimiento. |
| `Binance/` | Otro ejecutable para .NET Core 1.1. |
| `BinanceCore/` | Biblioteca .NET Standard 1.4; su contenido es mínimo. |
| `BinanceBot/TickerRules.cs` | Reglas de precisión y cantidades por par de negociación. |

## Tecnologías y requisitos

Windows, Visual Studio con las herramientas de .NET Framework 4.6.2 y NuGet. La variante Binance requiere el SDK histórico de .NET Core 1.1. Se usan Binance.API.Csharp.Client y Newtonsoft.Json.

Destinos leídos de los proyectos: `netcoreapp1.1`, `netstandard1.4`, `v4.6.2`.

## Preparación y arranque

Abrir BinanceBot.sln, restaurar los paquetes y seleccionar BinanceBot como proyecto de inicio. Revisar Binance.cs y las reglas antes de iniciar: Main puede enviar órdenes reales inmediatamente.

Soluciones presentes: `BinanceBot.sln`.

## Configuración e integraciones

Las credenciales están incrustadas en BinanceBot/Binance.cs. El capital, los pares y la estrategia también se fijan en el código. Extraer las claves a configuración privada antes de reconstruir el proyecto.

## Estado de esta copia

Es un prototipo de ejecución de órdenes; no se ha probado la compatibilidad con la API actual de Binance. No se ejecutó durante esta revisión.

La revisión fue estática: no se restauraron dependencias, no se compiló ni se ejecutaron aplicaciones o pruebas. Los comandos anteriores son instrucciones de preparación, no resultados de validación. Las versiones indicadas son las del código conservado.


## Copia pública

Las credenciales originales se han retirado o sustituido por `REPLACE_WITH_YOUR_VALUE`. Los datos locales, sesiones, certificados privados y perfiles de despliegue se omiten. Configurar servicios y credenciales propios antes de ejecutar. La compilación y el funcionamiento con servicios externos no se han validado.
