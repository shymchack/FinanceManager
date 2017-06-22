using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Financemanager.Database.Context;
using FinanceManager.Database.Entities;
using FinanceManager.DAL.Repositories.Contracts;

namespace FinanceManager.DAL.Repositories
{
    public class UsersRepository : FinanceManagerRepository, IUsersRepository, IDisposable
    {        
        public UsersRepository(IFinanceManagerContext context) : base(context)
        {
        }

        public int AddUser(User user)
        {
            Context.Users.Add(user);
            Context.SaveChanges();
            return user.ID;
        }

        public bool DoesUserExist(string userName)
        {
            return Context.Users.Any(u => u.UserName.ToLower() == userName.ToLower());
        }

        public void Dispose()
        {
            Context.Dispose();
            //TODO: Research about disposing base class
        }
    }
}
