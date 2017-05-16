using FinanceManager.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Financemanager.Database.Context;
using FinanceManager.Database.Entities;
using FinanceManager.DAL.Repositories.Contracts;

namespace FinanceManager.DAL.Repositories
{
    public class UsersRepository : FinanceManagerRepository, IDisposable
    {        
        public UsersRepository(FinanceManagerContext context) : base(context)
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
        }
    }
}
