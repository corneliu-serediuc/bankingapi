namespace BankingApi.Entities;

public class Transaction : Entity
{
    public string AccountId { get; set; }
    public string Type { get; set; }  // deposit, withdrawal
    public decimal Amount { get; set; }
}
