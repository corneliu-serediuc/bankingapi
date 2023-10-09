using BankingApi.Entities;
using BankingApi.Models;

namespace BankingApi.Extensions;

public static class AccountExtensions
{
    public static bool HasValidInitialBalance(this Account account)
    {
        return account.Balance >= 100;
    }

    public static bool CanDeposit(this AmountRequest request)
    {
        return request.Amount <= 10000;
    }

    public static bool CanWithdraw(this Account account, decimal amount)
    {
        var condition1 = account.Balance - amount < 100;
        var condition2 = account.Balance * 9 / 10 < amount;

        if (condition1 || condition2)
            return false;

        return true;
    }
}
