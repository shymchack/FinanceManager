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

namespace FinanceManager.DAL
{
    public class UserAccountsUnitOfWork : IUserAccountsUnitOfWork
    {
        private FinanceManagerContext _context;
        private IUsersRepository _usersRepository;
        private IAccountsRepository _accountsRepository;

        public FinanceManagerContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new FinanceManagerContext();
                }
                return _context;
            }
        }

        public IUsersRepository UsersRepository
        {
            get
            {
                if (_usersRepository == null)
                {
                    _usersRepository = new UsersRepository(Context);
                }
                return _usersRepository;
            }
        }

        public IAccountsRepository AccountsRepository
        {
            get
            {
                if (_accountsRepository == null)
                {
                    _accountsRepository = new AccountsRepository(Context);
                }
                return _accountsRepository;
            }
        }


        public UserAccountsUnitOfWork()
        {
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
            //TODO think about SQL connection test and error handling
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
        private SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
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
                if (_context != null)
                {
                    _context.Dispose();
                }
            }

            disposed = true;
        }
        #endregion
    }
}
