using FinanceManager.DAL.Dtos;

namespace FinanceManager.DAL.UnitOfWork
{
    public interface IAuthenticationUnitOfWork
    {
        TokenDto GetToken(string tokenData);
    }
}