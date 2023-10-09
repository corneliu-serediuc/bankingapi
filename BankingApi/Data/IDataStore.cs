using BankingApi.Entities;

namespace BankingApi.Data;

public interface IDataStore
{
    InMemoryDataStore<User> Users { get; }
    InMemoryDataStore<Account> Accounts { get; }
    InMemoryDataStore<Transaction> Transactions { get; }
}
