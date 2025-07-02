using Microsoft.EntityFrameworkCore;
using FUNewsManagement.Models;

namespace FUNewsManagement.Repositories.Impl;

public class SystemAccountRepository : GenericRepository<SystemAccount>, ISystemAccountRepository
{
    public SystemAccountRepository(FUNewsManagementDbContext context) : base(context)
    {
    }

    public async Task<SystemAccount?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.AccountEmail == email);
    }

    public async Task<SystemAccount?> AuthenticateAsync(string email, string password)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.AccountEmail == email && a.AccountPassword == password);
    }

    public async Task<IEnumerable<SystemAccount>> GetByRoleAsync(int role)
    {
        return await _dbSet.Where(a => a.AccountRole == role).ToListAsync();
    }
}
