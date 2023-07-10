﻿using Application.DaoInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shared.Model;

namespace DataAccess.DAOs;

public class UserDao : IUserDao
{
    private readonly DataContext context;
    
    public UserDao(DataContext context)
    {
        this.context = context;
    }

    public Task<IEnumerable<User>> GetAllUsersAsync()
    {
        IEnumerable<User> list = context.Users.Where(u => u.Role == "Member").ToList();
        return Task.FromResult(list);
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        User? existing = await context.Users.FirstOrDefaultAsync(u =>
            u.UserName.ToLower().Equals(username.ToLower())
        );
        return existing;
    }

    public async Task<User> CreateAsync(User user)
    {
        EntityEntry<User> added = await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return added.Entity;
    }
    

    public async Task DeleteAsync(string userName)
    {
        User? existing = await GetByUsernameAsync(userName);
        if (existing == null)
        {
            throw new Exception($"User with username {userName} not found");
        }

        context.Users.Remove(existing);
        await context.SaveChangesAsync();
    }
}