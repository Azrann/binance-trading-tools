**Languages:** [English](README.md) · [Español](README.es.md)

# binance-trading-tools

Bots y herramientas históricas para observar y operar en Binance, con integración Telegram.

<!-- BEGIN project-context -->

## Antigüedad y evidencia

**Año de referencia de esta versión: 2021.** Se toma el archivo de código o configuración más reciente con una fecha histórica recuperable; no se ha podido confirmar la fecha exacta de creación.

- Archivo más reciente datable: **2021-02-07**, [`binancebotnew/BinanceBot.Common/TelegramReader.cs`](binancebotnew/BinanceBot.Common/TelegramReader.cs); fecha de modificación conservada.
- Primer registro entre los repositorios Git originales incluidos: **2021-02-02**. Puede corresponder a una importación; no prueba cuándo se escribió por primera vez.
- Dependencia fechada más reciente entre las comprobadas: [Binance.Net 6.6.4](https://www.nuget.org/packages/Binance.Net/6.6.4), publicada el **2021-01-22** (versión declarada), en [`binancebotnew/CryptoCoreNew/CryptoCoreNew.csproj`](binancebotnew/CryptoCoreNew/CryptoCoreNew.csproj).
- Referencia adicional: destino `netcoreapp3.1` en [`binancebotnew/BinanceBot.Common/BinanceBot.Common.csproj`](binancebotnew/BinanceBot.Common/BinanceBot.Common.csproj); su [versión estable se publicó el 2019-12-03](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core). Es una pista de generación tecnológica; pudieron existir versiones preliminares anteriores.

La publicación de una dependencia sitúa la configuración conservada a partir de esa versión, si la referencia es válida. Una actualización de paquetes puede ser mucho más reciente que el inicio del proyecto. No se restauraron paquetes para comprobarlo.

Años de referencia por componente (archivo más reciente datable):

| Componente | Año |
| --- | --- |
| [`binanceBot`](binanceBot/) | 2018 |
| [`binancebotnew`](binancebotnew/) | 2021 |

## Por qué merece la pena mirarlo

Permite comparar un bot pequeño de compra/venta con una organización posterior de adaptadores, monitores y procesadores de mercado. La abstracción IExchange y el procesamiento del libro de órdenes son las partes más útiles para estudiar el diseño.

Puntos de entrada al código:

- [`binanceBot/BinanceBot/Binance.cs`](binanceBot/BinanceBot/Binance.cs)
- [`binancebotnew/CryptoCoreNew/IExchange.cs`](binancebotnew/CryptoCoreNew/IExchange.cs)
- [`binancebotnew/BinanceBot.Common/OrderBookProcessor.cs`](binancebotnew/BinanceBot.Common/OrderBookProcessor.cs)

<!-- END project-context -->

## Contenido

| Directorio | Función |
| --- | --- |
| [`binanceBot/`](binanceBot/) | Bot de consola para operar en Binance: compra a mercado, coloca una venta limitada y permite gestionar o cancelar la posición desde el teclado. |
| [`binancebotnew/`](binancebotnew/) | Conjunto de herramientas de consola para negociación y observación del mercado de Binance, con integración de clientes Telegram. |

binanceBot es la variante anterior. Algunos ejecutables envían órdenes reales: revisar Program.cs y usar una configuración de pruebas antes de ejecutarlos.

## Requisitos y preparación

.NET Framework y .NET Core 3.1; Binance.Net y OpenTL.

Esta es una copia histórica. Consultar los archivos de proyecto y los README de cada componente para sus versiones y dependencias. Restaurar NuGet/npm desde la carpeta correspondiente; las bibliotecas internas pueden necesitar reconstrucción. Preparar las bases de datos y los servicios externos con cuentas propias.

Soluciones conservadas:

- `binanceBot/BinanceBot.sln`
- `binancebotnew/BinanceBot.sln`

## Configuración y seguridad

Los secretos originales se han eliminado o sustituido por `REPLACE_WITH_YOUR_VALUE`. Buscar este marcador antes de ejecutar y aportar los valores mediante configuración local o variables de entorno. Las sesiones, claves privadas, certificados de firma, bases de datos, registros, perfiles de despliegue y paquetes binarios no se publican. El `.gitignore` protege estos archivos locales.

Se publica un historial nuevo: los commits antiguos permanecen únicamente en la copia local original. Los valores de ejemplo no permiten conectarse a los servicios originales.

## Estado de validación

Se realizó una revisión estática y un escaneo de secretos antes de publicar. No se ejecutaron aplicaciones, operaciones de trading, despliegues ni pruebas que contacten servicios externos. No se garantiza la compilación con herramientas actuales. Algunos proyectos necesitan dependencias históricas y activos que no están incluidos.

## Licencias y procedencia

Se conservan las licencias y atribuciones de terceros presentes en el código. No se añade una licencia global a los componentes que no la tenían. Esta importación conserva fuentes locales y no afirma ser una versión actual del proyecto original.
