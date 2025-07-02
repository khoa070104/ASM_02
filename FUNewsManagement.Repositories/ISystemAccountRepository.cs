using FUNewsManagement.Models;

namespace FUNewsManagement.Repositories;

public interface ISystemAccountRepository : IGenericRepository<SystemAccount>
{
    Task<SystemAccount?> GetByEmailAsync(string email);
    Task<SystemAccount?> AuthenticateAsync(string email, string password);
    Task<IEnumerable<SystemAccount>> GetByRoleAsync(int role);
}
