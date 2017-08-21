using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceManager.Database.Context;
using FinanceManager.Database.Entities;

namespace FinanceManager.DAL.Repositories.Contracts
{
    public interface IUsersRepository : IDisposable
    {
        int AddUser(User user);
        bool DoesUserExist(string userName);
    }
}
