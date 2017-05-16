using Financemanager.Database.Context;
using System;

namespace FinanceManager.DAL.Repositories
{
    public class FinanceManagerRepository
    {
        private FinanceManagerContext _context;
    
        public FinanceManagerContext Context => _context;

        public FinanceManagerRepository(FinanceManagerContext context)
        {
            _context = context;
        }
    }
}