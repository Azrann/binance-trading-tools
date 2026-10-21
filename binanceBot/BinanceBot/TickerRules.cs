using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceBot
{
    public class TickerRules
    {
        public Dictionary<string, Rule> Rules;

        public TickerRules()
        {
            Rules = new Dictionary<string, Rule>
            {
                {"ETHBTC", new Rule(3, 6)},
                {"LTCBTC", new Rule(2, 6)},
                {"BNBBTC", new Rule(0, 8)},
                {"NEOBTC", new Rule(2, 6)},
                {"GASBTC", new Rule(2, 6)},
                {"BCCBTC", new Rule(3, 6)},
                {"MCOBTC", new Rule(2, 6)},
                {"WTCBTC", new Rule(0, 8)},
                {"QTUMBTC", new Rule(2, 6)},
                {"OMGBTC", new Rule(2, 6)},
                {"ZRXBTC", new Rule(0, 8)},
                {"STRATBTC", new Rule(2, 6)},
                {"SNGLSBTC", new Rule(0, 8)},
                {"BQXBTC", new Rule(0, 8)},
                {"KNCBTC", new Rule(0, 8)},
                {"FUNBTC", new Rule(0, 8)},
                {"SNMBTC", new Rule(0, 8)},
                {"LINKBTC", new Rule(0, 8)},
                {"XVGBTC", new Rule(0, 8)},
                {"CTRBTC", new Rule(0, 8)},
                {"SALTBTC", new Rule(2, 6)},
                {"IOTABTC", new Rule(0, 8)},
                {"MDABTC", new Rule(0, 8)},
                {"MTLBTC", new Rule(0, 6)},
                {"SUBBTC", new Rule(0, 8)},
                {"EOSBTC", new Rule(0, 8)},
                {"SNTBTC", new Rule(0, 8)},
                {"ETCBTC", new Rule(2, 6)},
                {"MTHBTC", new Rule(0, 8)},
                {"ENGBTC", new Rule(0, 8)},
                {"DNTBTC", new Rule(0, 8)},
                {"BNTBTC", new Rule(0, 8)},
                {"ASTBTC", new Rule(0, 8)},
                {"DASHBTC", new Rule(3, 6)},
                {"ICNBTC", new Rule(0, 8)},
                {"OAXBTC", new Rule(0, 8)},
                {"BTGBTC", new Rule(2, 6)},
                {"EVXBTC", new Rule(0, 8)},
                {"REQBTC", new Rule(0, 8)},
                {"LRCBTC", new Rule(0, 8)},
                {"VIBBTC", new Rule(0, 8)},
                {"HSRBTC", new Rule(0, 6)},
                {"TRXBTC", new Rule(0, 8)},
                {"POWRBTC", new Rule(0, 8)},
                {"ARKBTC", new Rule(2, 7)},
                {"YOYOBTC", new Rule(0, 8)},
                {"XRPBTC", new Rule(0, 8)},
                {"MODBTC", new Rule(0, 7)},
                {"ENJBTC", new Rule(0, 8)},
                {"STORJBTC", new Rule(0, 8)},
                {"VENBTC", new Rule(0, 8)},
                {"KMDBTC", new Rule(0, 7)},
                {"RCNBTC", new Rule(0, 8)},
                {"NULSBTC", new Rule(0, 8)},
                {"RDNBTC", new Rule(0, 8)},
                {"XMRBTC", new Rule(3, 6)},
                {"DLTBTC", new Rule(3, 8)},
                {"AMBBTC", new Rule(3, 8)},
                {"BATBTC", new Rule(0, 8)},
                {"ZECBTC", new Rule(3, 6)},
                {"BCPTBTC", new Rule(0, 8)},
                {"ARNBTC", new Rule(0, 8)},
                {"GVTBTC", new Rule(2, 7)},
                {"CDTBTC", new Rule(0, 8)},
                {"GXSBTC", new Rule(2, 7)},
                {"POEBTC", new Rule(0, 8)},
                {"QSPBTC", new Rule(0, 8)},
                {"BTSBTC", new Rule(0, 8)},
                {"XZCBTC", new Rule(2, 6)},
                {"LSKBTC", new Rule(2, 7)},
                {"TNTBTC", new Rule(0, 8)},
                {"FUELBTC", new Rule(0, 8)},
                {"MANABTC", new Rule(0, 8)},
                {"BCDBTC", new Rule(3, 6)},
                {"DGDBTC", new Rule(3, 6)},
                {"ADXBTC", new Rule(0, 8)},
                {"ADABTC", new Rule(0, 8)},
                {"PPTBTC", new Rule(2, 7)},
                {"CMTBTC", new Rule(0, 8)},
                {"XLMBTC", new Rule(0, 8)},
                {"CNDBTC", new Rule(0, 8)},
                {"LENDBTC", new Rule(0, 8)},
                {"WABIBTC", new Rule(0, 8)},
                {"TNBBTC", new Rule(0, 8)},
                {"WAVES", new Rule(2, 6)},
                {"ICXBTC", new Rule(2, 7)},
                {"GTOBTC", new Rule(0, 8)},
                {"OSTBTC", new Rule(0, 8)},
                {"ELFBTC", new Rule(0, 8)},
                {"AIONBTC", new Rule(0, 7)},
                {"NEBLBTC", new Rule(0, 7)},
                {"BRDBTC", new Rule(0, 8)},
                {"EDOBTC", new Rule(0, 7)},
                {"WINGS", new Rule(0, 8)},
                {"NAVBTC", new Rule(0, 7)},
                {"LUNBTC", new Rule(0, 7)},
                {"TRIGBTC", new Rule(0, 7)},
                {"APPCBTC", new Rule(0, 8)}
            };
        }
    }

    public class Rule
    {
        public int MinAmountDecimals { get; set; }
        public int MinPriceDecimals { get; set; }

        public Rule(int minAmountDecimals, int minPriceDecimals)
        {
            MinAmountDecimals = minAmountDecimals;
            MinPriceDecimals = minPriceDecimals;
        }
    }
}
