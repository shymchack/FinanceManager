using Financemanager.Database.Context;
using FinanceManager.DAL.Dtos;
using FinanceManager.DAL.Repositories;
using FinanceManager.DAL.Repositories.Contracts;
using FinanceManager.Database.Entities;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.DAL.UnitOfWork
{
    public class UserAccountsUnitOfWork : FinanceManagerUnitOfWork
    {
        private UsersRepository _usersRepository;
        private AccountsRepository _accountsRepository;

        public UserAccountsUnitOfWork()
        {
            Context = new FinanceManagerContext();

            _usersRepository = new UsersRepository(Context);
            _accountsRepository = new AccountsRepository(Context);
        }

        public int CreateUser(string userName, string firstName, string lastName)
        {
            User user = new User();
            user.UserName = userName;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.IsActive = true;
            return _usersRepository.AddUser(user);
        }

        public bool DoesUserExist(string userName)
        {
            return _usersRepository.DoesUserExist(userName);
        }

        public List<AccountDto> GetAccounts()
        {
            return _accountsRepository.GetAccounts();
        }

        public int CreateAccount(string name, int userID)
        {
            Account newAccount = Context.Accounts.Create();
            newAccount.CreationDate = DateTime.UtcNow;
            newAccount.Name = name;

            User user = Context.Users.FirstOrDefault(u => u.ID == userID);
            if (user != null)
            {
                UserAccount userAccount = new UserAccount();
                userAccount.User = user;
                newAccount.UsersAccounts.Add(userAccount);
            }
            return _accountsRepository.AddAccount(newAccount);
        }

        #region IDisposable stuff
        private bool disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                if (_usersRepository != null)
                {
                    _usersRepository.Dispose();
                }
                if (_accountsRepository != null)
                {
                    _accountsRepository.Dispose();
                }
            }

            disposed = true;
            base.Dispose(disposing);
        }
        #endregion
    }
}
