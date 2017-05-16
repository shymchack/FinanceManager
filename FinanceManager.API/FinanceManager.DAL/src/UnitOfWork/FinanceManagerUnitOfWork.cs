using Financemanager.Database.Context;
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace FinanceManager.DAL.UnitOfWork
{
    public class FinanceManagerUnitOfWork : IDisposable
    {
        private FinanceManagerContext _context;

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
            set { _context = value; }
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