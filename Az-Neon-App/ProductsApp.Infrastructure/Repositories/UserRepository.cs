using Microsoft.EntityFrameworkCore;
using ProductsApp.Domain.Users;
using ProductsApp.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsApp.Infrastructure.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task AddAsync(User user)
    {
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        dbContext.Users.Remove(new User { Id = id });
        await dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x=> x.Id == id);
    }

    public async Task UpdateAsync(User user)
    {
        dbContext.Update(user);
        await dbContext.SaveChangesAsync();
    }
}
