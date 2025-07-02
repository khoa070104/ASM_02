using FUNewsManagement.Models;
using FUNewsManagement.Repositories;

namespace FUNewsManagement.Services.Impl;

public class SystemAccountService : ISystemAccountService
{
    private readonly IUnitOfWork _unitOfWork;

    public SystemAccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SystemAccount>> GetAllAccountsAsync()
    {
        return await _unitOfWork.SystemAccounts.GetAllAsync();
    }

    public async Task<SystemAccount?> GetAccountByIdAsync(short id)
    {
        return await _unitOfWork.SystemAccounts.GetByIdAsync(id);
    }

    public async Task<SystemAccount?> GetAccountByEmailAsync(string email)
    {
        return await _unitOfWork.SystemAccounts.GetByEmailAsync(email);
    }

    public async Task<SystemAccount?> AuthenticateAsync(string email, string password)
    {
        return await _unitOfWork.SystemAccounts.AuthenticateAsync(email, password);
    }

    public async Task<IEnumerable<SystemAccount>> GetAccountsByRoleAsync(int role)
    {
        return await _unitOfWork.SystemAccounts.GetByRoleAsync(role);
    }

    public async Task<SystemAccount> CreateAccountAsync(SystemAccount account)
    {
        if (await EmailExistsAsync(account.AccountEmail))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        await _unitOfWork.SystemAccounts.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();
        return account;
    }

    public async Task<SystemAccount> UpdateAccountAsync(SystemAccount account)
    {
        if (await EmailExistsAsync(account.AccountEmail, account.AccountID))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        _unitOfWork.SystemAccounts.Update(account);
        await _unitOfWork.SaveChangesAsync();
        return account;
    }

    public async Task<bool> DeleteAccountAsync(short id)
    {
        var account = await _unitOfWork.SystemAccounts.GetByIdAsync(id);
        if (account == null)
            return false;

        _unitOfWork.SystemAccounts.Remove(account);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SystemAccount>> SearchAccountsAsync(string searchTerm)
    {
        return await _unitOfWork.SystemAccounts.FindAsync(a =>
            a.AccountName.Contains(searchTerm) ||
            a.AccountEmail.Contains(searchTerm));
    }

    public async Task<bool> EmailExistsAsync(string email, short? excludeID = null)
    {
        if (excludeID.HasValue)
        {
            return await _unitOfWork.SystemAccounts.ExistsAsync(a =>
                a.AccountEmail == email && a.AccountID != excludeID.Value);
        }

        return await _unitOfWork.SystemAccounts.ExistsAsync(a => a.AccountEmail == email);
    }

    public async Task<bool> HasCreatedNewsAsync(short accountID)
    {
        return await _unitOfWork.NewsArticles.ExistsAsync(n => n.CreatedById == accountID);
    }
}
