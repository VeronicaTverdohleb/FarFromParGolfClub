﻿using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using HttpClients.ClientInterfaces;
using Shared.Dtos.ScoreDto;

namespace HttpClients.Implementations;

/// <summary>
/// Class responsible for making REST requests towards Web API
/// </summary>
public class ScoreHttpClient : IScoreService
{
    private readonly HttpClient client;

    public ScoreHttpClient(HttpClient client)
    {
        this.client = client;
    }
    
    /// <summary>
    /// PATCH HTTP request to Update Scores from Member side (only updates all scores in a Scorecard at a time)
    /// </summary>
    /// <param name="dto"></param>
    /// <exception cref="Exception"></exception>
    public async Task UpdateFromMemberAsync(ScoreBasicDto dto)
    {
        string dtoAsJson = JsonSerializer.Serialize(dto);
        StringContent body = new StringContent(dtoAsJson, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PatchAsync("/Score", body);
        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            throw new Exception(content);
        }
    }

    /// <summary>
    /// PATCH HTTP request to update Scores from Employee (updates any amount of Scores)
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task UpdateFromEmployeeAsync(ScoreUpdateDto dto)
    {
        string dtoAsJson = JsonSerializer.Serialize(dto);
        StringContent body = new StringContent(dtoAsJson, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PatchAsync("/Scores", body);
        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            throw new Exception(content);
        }
    }
}