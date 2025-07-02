using FUNewsManagement.Models;

namespace FUNewsManagement.Services;

public interface ISystemAccountService
{
    Task<IEnumerable<SystemAccount>> GetAllAccountsAsync();
    Task<SystemAccount?> GetAccountByIdAsync(short id);
    Task<SystemAccount?> GetAccountByEmailAsync(string email);
    Task<SystemAccount?> AuthenticateAsync(string email, string password);
    Task<IEnumerable<SystemAccount>> GetAccountsByRoleAsync(int role);
    Task<SystemAccount> CreateAccountAsync(SystemAccount account);
    Task<SystemAccount> UpdateAccountAsync(SystemAccount account);
    Task<bool> DeleteAccountAsync(short id);
    Task<bool> HasCreatedNewsAsync(short accountID);
    Task<IEnumerable<SystemAccount>> SearchAccountsAsync(string searchTerm);
    Task<bool> EmailExistsAsync(string email, short? excludeId = null);
}
