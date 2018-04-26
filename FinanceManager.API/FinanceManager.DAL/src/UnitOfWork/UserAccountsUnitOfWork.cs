using FinanceManager.Database.Context;
using FinanceManager.DAL.Dtos;
using FinanceManager.DAL.Repositories.Contracts;
using FinanceManager.Database.Entities;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using FinanceManager.DAL.UnitOfWork;

namespace FinanceManager.DAL
{
    public class UserAccountsUnitOfWork : IUserAccountsUnitOfWork
    {
        private IUsersRepository _usersRepository;
        private IAccountsRepository _accountsRepository;

        private IFinanceManagerContext _context;


        public UserAccountsUnitOfWork(IFinanceManagerContext context, IUsersRepository usersRepository, IAccountsRepository accountsRepository)
        {
            _context = context;
            _usersRepository = usersRepository;
            _accountsRepository = accountsRepository;
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
            Account newAccount = _accountsRepository.CreateAccount();
            newAccount.CreationDate = DateTime.UtcNow;
            newAccount.Name = name;

            User user = _context.Users.FirstOrDefault(u => u.ID == userID);
            if (user != null)
            {
                UserAccount userAccount = new UserAccount();
                userAccount.User = user;
                newAccount.UsersAccounts.Add(userAccount);
            }
            return _accountsRepository.AddAccount(newAccount);
        }

        public IEnumerable<AccountDto> GetAccountsByUserId(int userId)
        {
            IEnumerable<Account> accounts = _accountsRepository.GetAccountsByUserId(userId);
            List<AccountDto> accountsDtos = new List<AccountDto>();
            foreach(Account account in accounts)
            {
                accountsDtos.Add(ReadAccountDtoFromData(account));
            }

            return accountsDtos;
        }

        private AccountDto ReadAccountDtoFromData(Account account)
        {
            AccountDto dto = new AccountDto();
            dto.CurrentAmount = account.CurrentAmount;
            dto.ID = account.ID;
            dto.InitialAmount = account.InitialAmount;
            dto.Name = account.Name;

            return dto;
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
