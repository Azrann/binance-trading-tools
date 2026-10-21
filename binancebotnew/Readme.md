# binancebotnew

Conjunto de herramientas de consola para negociación y observación del mercado de Binance, con integración de clientes Telegram.

Documentación basada en los archivos locales revisados el 21 de octubre de 2026.

## Estructura y responsabilidades

| Ruta | Responsabilidad |
| --- | --- |
| `BinanceBotNew/` | Ejecutable de negociación. |
| `BinanceDataDownloader/` | Descarga de datos del mercado. |
| `DataMonitorOnly/, OrderBookMonitor/, PumpTradesAndOrderBookMonitor/, VolumeMonitorNew/` | Ejecutables para observar precios, libro de órdenes, operaciones y volumen. |
| `CryptoCoreNew/` | Abstracción IExchange y cliente BinanceExchange, órdenes, saldos y comisiones. |
| `BinanceBot.Common/, Core.Common/` | Procesamiento del mercado, utilidades HTTP, consola y registro. |
| `Mentor.telegram.client/` | Cliente OpenTL y sesiones persistentes de Telegram. |
| `LogReader/` | Herramienta auxiliar .NET Framework 4.5.2 para leer registros. |

## Tecnologías y requisitos

.NET Core 3.1 para los ejecutables principales; Windows y .NET Framework 4.5.2 para LogReader. Binance.Net 6.6.4, RestSharp y OpenTL son dependencias relevantes.

Destinos leídos de los proyectos: `netcoreapp3.1`, `v4.5.2`.

## Preparación y arranque

Elegir un ejecutable según su función. Revisar su Program.cs antes de usar dotnet run: algunos operan sobre cuentas reales y otros solo monitorizan.

Comandos de preparación, desde esta carpeta, después de revisar la configuración privada:

```text
dotnet restore BinanceBot.sln
dotnet build BinanceBotNew/BinanceBotNew.csproj
```

Soluciones presentes: `BinanceBot.sln`.

## Configuración e integraciones

Varios Program.cs contienen claves de Binance. Mentor.telegram.client/ClientFactory.cs configura el acceso a Telegram y los archivos session*.dat conservan sesiones autenticadas. Sustituir estas credenciales y usar sesiones propias.

## Estado de esta copia

Los perfiles de publicación y las sesiones son configuración local. La revisión no inició conexiones a Binance ni Telegram.

La revisión fue estática: no se restauraron dependencias, no se compiló ni se ejecutaron aplicaciones o pruebas. Los comandos anteriores son instrucciones de preparación, no resultados de validación. Las versiones indicadas son las del código conservado.


## Copia pública

Las credenciales originales se han retirado o sustituido por `REPLACE_WITH_YOUR_VALUE`. Los datos locales, sesiones, certificados privados y perfiles de despliegue se omiten. Configurar servicios y credenciales propios antes de ejecutar. La compilación y el funcionamiento con servicios externos no se han validado.
