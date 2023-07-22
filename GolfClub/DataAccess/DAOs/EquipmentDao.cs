﻿using System.Diagnostics;
using Application.DaoInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shared.Dtos.EquipmentDto;
using Shared.Model;

namespace DataAccess.DAOs;

public class EquipmentDao: IEquipmentDao
{
    private readonly DataContext context;

    public EquipmentDao(DataContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<Equipment>> CreateEquipmentAsync(IEnumerable<EquipmentBasicDto>  equipment, int amount)//call in the page
    {
        List<Equipment> addedEquipment = new List<Equipment>();

        foreach (var equipmentItem in equipment)
        {
            for (int i = 0; i <= amount; i++)
            {

                Equipment newEquipment = new Equipment(equipmentItem.Name)
                {
                    Name = equipmentItem.Name

                };


                EntityEntry<Equipment> added = await context.Equipments.AddAsync(newEquipment);
               

                addedEquipment.Add(added.Entity);
                await context.SaveChangesAsync();
            }
        }

        return addedEquipment;
       
       
    }

    public async Task UpdateEquipmentAsync(string? name, int amount)
    {
        var entryToDelete = context.Equipments.FirstOrDefault(e => e.Name == name);
        if (entryToDelete != null)
        {
            context.Equipments.Remove(entryToDelete);
            await context.SaveChangesAsync();
        }
    }

    public Task<IEnumerable<Equipment>> GetEquipmentsAsync(SearchEquipmentDto searchParameters)
    {
      IEnumerable<Equipment> list = context.Equipments.ToList();
      if (!string.IsNullOrEmpty(searchParameters.NameContains))
      {
          list = list.Where(e => e.Name.Contains(searchParameters.NameContains, StringComparison.OrdinalIgnoreCase));
      }
        return Task.FromResult(list);
    }

    public async Task<Equipment?> GetEquipmentByNameAsync(string name)
    {
        var found = await context.Equipments
            .AsNoTracking().SingleOrDefaultAsync(e => e.Name == name);
        return found;
       
    }

    public int GetCountOfEquipment(string name)
    {
        IQueryable<Equipment> q = context.Equipments.AsQueryable();
        int count = 0;
        if (name != null)
        {
            count = q.Where(e=>e.Name==name).GroupBy(e => e.Name).Count();
                
        }

        return count;
    }

    public async Task<Equipment?> GetEquipmentByIdAsync(int id)
    {
        Equipment? found = await context.Equipments.AsNoTracking()
            .SingleOrDefaultAsync(e => e.Id == id);
        if (found == null)
        {
            throw new Exception($"Equipment with id {id} not found");
        }
        return found;
    }

    public async Task<List<Equipment>> GetEquipmentListByNameAsync(string name)
    {
        return await context.Equipments.Where(e => e.Name.Equals(name)).ToListAsync();
    }

    public async Task DeleteEquipmentAsync(string? name)
    {
        var entriesToDelete = context.Equipments.Where(e => e.Name == name).ToList();
        foreach (var entry in entriesToDelete)
        {
            context.Equipments.Remove(entry);
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteAllEquipmentByGameIdAsync(int gameId)
    {
        Debug.WriteLine($"Deleting equipment for Game ID: {gameId}");
        Game? existing = await context.Games.FirstOrDefaultAsync(g => g.Id == gameId);
        //IEnumerable<Equipment> rentedEquipmnet = await GetEquipmentByGameIdAsync(existing.Id);

        if (existing == null)
        {
            throw new Exception("Game not found with the specified ID.");
        }

        existing.Equipments!.Clear(); // Clear all existing equipment associations for the game

        await context.SaveChangesAsync();
       /* Game? existing = await context.Games.FirstOrDefaultAsync(g => g.Id == gameId);
        if (existing == null)
        {
            throw new Exception("Game not found with the specified ID.");
        }
        List<Equipment> rentedEquipment = new List<Equipment>();
        var getRentedEquipment = await GetEquipmentByGameIdAsync(existing.Id);
        foreach (var g in existing.Equipments )
        {
            Equipment? e = getRentedEquipment.FirstOrDefault(eq => eq.Id ==g.Id);
            if (e != null)
            {
                rentedEquipment.Remove(e);
            }
        }
        foreach (var equip in rentedEquipment)
        {
            existing.Equipments.Remove(
                equip);
        }

        await context.SaveChangesAsync();*/
       
        
    }

    public Task<IEnumerable<Equipment>> GetAvailableEquipmentAsync()
    {
        IEnumerable<Equipment> result = context.Equipments.Where(e=>!context.Games.Any(g=>g.Equipments.Contains(e))).AsEnumerable();

        return Task.FromResult(result);


    }
    
    public Task<IEnumerable<Equipment>> GetEquipmentByGameIdAsync(int gameId)
    {
        IEnumerable<Equipment> result = context.Equipments
            .Where(e => context.Games.Any(g => g.Equipments.Contains(e) && g.Id == gameId))
            .AsEnumerable();

        return Task.FromResult(result);

    }
    
    public async Task<List<int>> GetAvailableEquipmentIds()
    {
        var result = await context.Equipments
            .Where(e => !context.Games.Any(g => g.Equipments.Contains(e)))
            .Select(e => e.Id)
            .ToListAsync();

        return result;
    }

    public async Task<List<int>> GetGameEquipmentIds(int gameId)
    {
      
        var results = await context.Equipments
            .Where(e => context.Games.Any(g => g.Equipments.Contains(e) && g.Id == gameId))
            .Select(e => e.Id).ToListAsync();
        return results;

    }

    public async Task RentEquipment(RentEquipmentDto dto )
    {
        Game? existing = await context.Games.FirstOrDefaultAsync(g => g.Id == dto.GameId);
        if (existing == null)
        {
            throw new Exception("Game not found with the specified ID.");
        }
        List<Equipment> forRentEquipment = new List<Equipment>();
        var availableEquipment = await GetAvailableEquipmentAsync();
        foreach (int equipment in dto.EquipmentIds )
        {
            Equipment? e = availableEquipment.FirstOrDefault(eq => eq.Id == equipment);
            if (e != null)
            {
                forRentEquipment.Add(e);
            }
        }
        foreach (var equipment in forRentEquipment)
        {
            existing.Equipments.Add(equipment);
        }

        await context.SaveChangesAsync();

    }
   


        

}