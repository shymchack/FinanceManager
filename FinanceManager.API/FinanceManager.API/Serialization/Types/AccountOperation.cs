namespace FinanceManager.API.Serialization.Types
{
    public class AccountOperation
    {
        public string Name { get; set; }
        public decimal TotalValue { get; set; }
        public string ExecutionDate { get; set; }
        public decimal AlreadyPayed { get; set; }
    }
}