﻿using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using HttpClients.ClientInterfaces;
using Shared.Dtos.EquipmentDto;
using Shared.Model;

namespace HttpClients.Implementations;

public class EquipmentHttpClient: IEquipmentService
{
    private readonly HttpClient client;

    public EquipmentHttpClient(HttpClient client)
    {
        this.client = client;
    }

    public async Task<ICollection<Equipment?>> getAllEquipmentAsync(string? name)
    {
        string query = ConstructQuery(name);
        HttpResponseMessage response = await client.GetAsync("/Equipment"+query);
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        ICollection<Equipment?> equipments = JsonSerializer.Deserialize<ICollection<Equipment>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
        return equipments; 
    }
    
    private static string ConstructQuery(string? name)
    {
        string query = "";
        if (!string.IsNullOrEmpty(name))
        {
            query += $"?Name={name}";
        }

       
        if (!string.IsNullOrEmpty(name))
        {
            query += string.IsNullOrEmpty(query) ? "?" : "&";
            query += $"namecontains={name}";
        }
        

        return query;
    }

    public async Task CreateEquipmentAsync(IEnumerable<EquipmentBasicDto>  equipment, int amount)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("/Equipment", equipment);  
        
            
            Console.WriteLine("in the http ");  
            if (!response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                throw new Exception(content);
            }


    }

    public async Task UpdateEquipmentAmount(string name, int amount)
    { 
        string requestUrl = $"/Equipment/{name}/{amount}";
        HttpResponseMessage response = await client.DeleteAsync(requestUrl);
        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            throw new Exception(content);
        }
        /*HttpResponseMessage response = await client.DeleteAsync($"Equipment/{name}/{amount}");
        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            throw new Exception(content);
        }
         string requestUrl = $"/Equipment",name,amount;
         HttpResponseMessage response = await client.PatchAsync(requestUrl, null);
         if (!response.IsSuccessStatusCode)
         {
             string content = await response.Content.ReadAsStringAsync();
             throw new Exception(content);
         }
         HttpResponseMessage response = await client.PatchAsync("/Equipment",name);
         if (!response.IsSuccessStatusCode)
         {
             string content = await response.Content.ReadAsStringAsync();
             throw new Exception(content);
         }*/
    }

    public async Task<Equipment?> GetEquipmentByNameAsync(string name)
    {
        HttpResponseMessage response = await client.GetAsync($"/Equipment/{name}");
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        Equipment equipment = JsonSerializer.Deserialize<Equipment>(content, 
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        )!;
        return equipment;
    }

    public async Task DeleteEquipmentAsync(string? name)
    {
        HttpResponseMessage response = await client.DeleteAsync($"Equipment/{name}");
        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            throw new Exception(content);
        }
    }

    public async Task DeleteAllEquipmentByGameIdAsync(int gameId)
    {
        HttpResponseMessage response = await client.DeleteAsync("Equipment/");
        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            throw new Exception(content);
        } 
    }

    public async Task<int> CountOfEquipment(string name)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("/Equipment",name);
        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
             throw new Exception(content);
        } 
        int n= JsonSerializer.Deserialize<int>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
        return n; 
    }

    public async Task RentEquipment(RentEquipmentDto dto)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("/RentEquipment", dto);
        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            throw new Exception(content);
        }
    }
    
    public async Task<ICollection<Equipment>?> GetAvailableEquipment()
    {
        var response = await client.GetAsync("/Equipments");
       // response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        ICollection<Equipment> result = JsonSerializer.Deserialize<ICollection<Equipment>>(content,
        new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
        return result;

    }

    public async Task<List<int>?> GetAvailableEquipmentIds()
    {
        var response = await client.GetAsync("/RentEquipment");
        response.EnsureSuccessStatusCode();
        var responseStream = await response.Content.ReadAsStreamAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = await JsonSerializer.DeserializeAsync<List<int>>(responseStream, options);
        return result;
    }

    public async Task<List<int>> GetGameEquipmentIds(int gameId)
    {
        var response = await client.GetAsync($"/RentEquipment/{gameId}");
        response.EnsureSuccessStatusCode();
        var responseStream = await response.Content.ReadAsStreamAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = await JsonSerializer.DeserializeAsync<List<int>>(responseStream, options);
        return result;
    }

    public async Task<ICollection<Equipment>?> GetEquipmentByGameIdAsync(int gameId)
    {
        var response = await client.GetAsync($"/Equipments/{gameId}");
        // response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        ICollection<Equipment> result = JsonSerializer.Deserialize<ICollection<Equipment>>(content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
        return result;
    }
}