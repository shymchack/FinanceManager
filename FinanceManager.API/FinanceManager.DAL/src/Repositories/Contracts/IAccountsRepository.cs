using FinanceManager.DAL.Dtos;
using System.Collections.Generic;

namespace FinanceManager.DAL.Repositories
{
    public interface IAccountsRepository : IFinanceManagerRepository
    {
        List<AccountDto> GetAccounts();
    }
}
