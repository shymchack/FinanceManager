using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Financemanager.Database.Context;

namespace FinanceManager.DAL.Repositories.Contracts
{
    public interface IUsersRepository : IDisposable
    {
        FinanceManagerContext Context { get; set; }

        int CreateUser(string userName, string firstName, string lastName);
        bool DoesUserExist(string userName);
    }
}
