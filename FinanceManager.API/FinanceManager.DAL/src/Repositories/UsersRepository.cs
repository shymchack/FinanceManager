using FinanceManager.DAL.Repositories;
using FinanceManager.DAL.src.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Financemanager.Database.Context;
using FinanceManager.Database.Entities;

namespace FinanceManager.DAL.src.Repositories
{
    public class UsersRepository : FinanceManagerRepository, IUsersRepository
    {
        public UsersRepository() : base()
        {

        }

        public int CreateUser(string userName, string firstName, string lastName)
        {
            User user = new User();
            user.UserName = userName;
            user.FirstName= firstName;
            user.LastName = lastName;
            user.IsActive = true;
            Context.Users.Add(user);
            Context.SaveChanges();
            return user.ID;
        }

        public bool DoesUserExist(string userName)
        {
            return Context.Users.Any(u => u.UserName.ToLower() == userName.ToLower());
        }
    }
}
