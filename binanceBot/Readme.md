# binanceBot

Bot de consola para operar en Binance: compra a mercado, coloca una venta limitada y permite gestionar o cancelar la posición desde el teclado.

Documentación basada en los archivos locales revisados el 21 de octubre de 2026.

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
