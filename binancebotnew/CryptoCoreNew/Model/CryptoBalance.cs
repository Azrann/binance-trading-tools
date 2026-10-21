namespace CryptoCoreNew.Model
{
    public class CryptoBalance
    {
        public string Symbol { get; set; }
        public decimal Available { get; set; }
        public bool CanTradeOrDeposit { get; set; }
    }
}
