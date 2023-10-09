using BankingApi.Entities;

namespace BankingApi.Data;

public class DataStore : IDataStore
{
    public DataStore()
    {
        Users = new InMemoryDataStore<User>();
        Accounts = new InMemoryDataStore<Account>();
        Transactions = new InMemoryDataStore<Transaction>();
    }

    public InMemoryDataStore<User> Users { get; private set; }
    public InMemoryDataStore<Account> Accounts { get; private set; }
    public InMemoryDataStore<Transaction> Transactions { get; private set; }
}
