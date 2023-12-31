﻿using Application.LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.ScoreDto;
using Shared.Model;

namespace WebAPI.Controllers;

/// <summary>
/// Controller definition for Score-related requests, it communicates on request-reply basis
/// with GameHttpClient in the BlazorApp
/// It calls methods in the ScoreLogic
/// </summary>
[ApiController]
[Route("[controller]")]
public class ScoreController : ControllerBase
{
    private readonly IScoreLogic scoreLogic;

    public ScoreController(IScoreLogic scoreLogic)
    {
        this.scoreLogic = scoreLogic;
    }
    
    /// <summary>
    /// PATCH endpoint that accepts ScoreBasicDto as a parameter and calls the ScoreLogic
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPatch("/Score")]
    public async Task<ActionResult> UpdateFromMemberAsync([FromBody]ScoreBasicDto dto)
    {
        try
        {
            await scoreLogic.UpdateFromMemberAsync(dto);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
    
    /// <summary>
    /// PATCH endpoint that accepts ScoreUpdateDto as a parameter and calls the ScoreLogic
    /// To Update the Scores 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>

    [HttpPatch("/Scores")]
    public async Task<ActionResult> UpdateFromEmployeeAsync([FromBody] ScoreUpdateDto dto)
    {
        try
        {
            await scoreLogic.UpdateFromEmployeeAsync(dto);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
    

}