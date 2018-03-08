namespace ManagementAPI.Repositories.Accounts
{
    using ManagementAPI.Models;
    using ManagementAPI.ViewModels;
    using System.Collections.Generic;

    public interface IAccountRepository
    {
        Account Get(int id);

        List<Account> GetAll();

        bool Save(AccountViewModel vm);

        bool Delete(int id);
    }
}