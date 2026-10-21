namespace CryptoCoreNew.Model
{
    public class Currency
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public decimal WithdrawFee { get; set; }
        public decimal MinWithdraw { get; set; }
    }
}