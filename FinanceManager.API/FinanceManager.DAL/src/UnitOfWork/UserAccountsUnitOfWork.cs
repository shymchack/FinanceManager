﻿using Financemanager.Database.Context;
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