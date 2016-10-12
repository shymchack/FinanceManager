using Financemanager.Database.Context;

namespace FinanceManager.DAL.Repositories
{
    public interface IFinanceManagerRepository
    {
        FinanceManagerContext Context { get; set; }
    }
}