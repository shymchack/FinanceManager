namespace FinanceManager.API.Serialization.Types
{
    public class AccountBalance
    {
        public decimal Value { get; set; }
        public decimal SafeValue { get; set; }
        public string Date { get; set; }
    }
}