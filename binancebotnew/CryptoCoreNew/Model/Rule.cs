namespace CryptoCoreNew.Model
{
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