namespace BankingApi.Entities;

public class Account : Entity
{
    public string UserId { get; set; }
    public decimal Balance { get; set; }
}
