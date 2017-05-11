using Financemanager.Database.Context;
using System;

namespace FinanceManager.DAL.Repositories
{
    public class FinanceManagerRepository : IDisposable
    {
        private FinanceManagerContext _context;

        public FinanceManagerRepository()
        {
        }

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

            set
            {
                if (value != _context)
                    _context = value;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}