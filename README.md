# binance-trading-tools

Bots y herramientas históricas para observar y operar en Binance, con integración Telegram.

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
