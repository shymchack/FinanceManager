using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.DAL.src.Repositories.Contracts
{
    public interface IUsersRepository
    {
        int CreateUser(string userName, string firstName, string lastName);
        bool DoesUserExist(string userName);
    }
}
