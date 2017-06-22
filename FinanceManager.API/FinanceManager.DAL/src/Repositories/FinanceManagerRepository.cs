using Financemanager.Database.Context;
using System;

namespace FinanceManager.DAL.Repositories
{
    public class FinanceManagerRepository
    {
        private IFinanceManagerContext _context;
    
        public IFinanceManagerContext Context => _context;

        public FinanceManagerRepository(IFinanceManagerContext context)
        {
            _context = context;
        }
    }
}