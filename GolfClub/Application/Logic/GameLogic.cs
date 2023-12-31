﻿using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Shared.Dtos.GameDto;
using Shared.Model;

namespace Application.Logic;

/// <summary>
/// This class takes care of the business logic for Game-related responsibilities
/// </summary>
public class GameLogic : IGameLogic
{
    private readonly IGameDao gameDao;
    private readonly IUserDao userDao;

    public GameLogic(IGameDao gameDao, IUserDao userDao)
    {
        this.gameDao = gameDao;
        this.userDao = userDao;
    }

    /// <summary>
    /// Business Logic for Game creation
    /// It returns Exception if any player in the Game has unfinished Games (game with Scorecard that has 0 values)
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Task<Game></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Game> CreateAsync(GameBasicDto dto)
    {
        // If there is a player with unfinished game, the game will not be created and exception is thrown
        foreach (string playerUsername in dto.PlayerUsernames)
        {
            IEnumerable<Game> games = await gameDao.GetGamesByUsername(playerUsername);
            if (games.Any())
            {
                foreach (Game game in games.ToList())
                {
                    foreach (Score score in game.Scores!)
                    {
                        if (score.Strokes == 0)
                            throw new Exception($"User with username {playerUsername} has an unfinished game. Cannot create a new game with this user.");
                    }
                }
            }
        }

        Game created = await gameDao.CreateAsync(dto);
        return created;
    }

    /// <summary>
    /// This method returns a Game By Username that has a Score with Strokes = 0
    /// Each Game has 18 Scores per player, and if even a single Score has Strokes = 0
    /// Then this Game gets returned
    /// It returns Exception if no user with this username is found
    /// </summary>
    /// <param name="username"></param>
    /// <returns>Task<Game?></returns>
    /// <exception cref="Exception"></exception>
    public Task<Game?> GetActiveGameByUsernameAsync(string username)
    {
        User? user = userDao.GetByUsernameAsync(username).Result;
        if (user == null)
            throw new Exception("No user found");
        
        Task<IEnumerable<Game>> games = gameDao.GetGamesByUsername(username);
        Game gameToBeReturned = null;
        foreach (Game game in games.Result)
        {
            if (game.Scores == null || game.Scores.Count == 0)
            {
                gameToBeReturned = game;
                break;
            }
            foreach (Score score in game.Scores!)
            {
                if (score.PlayerUsername == username && score.Strokes == 0)
                {
                    gameToBeReturned = game;
                    break;
                }
            }
        }

        return Task.FromResult(gameToBeReturned);
    }

    /// <summary>
    /// This method fetches all Games of a player by their username
    /// </summary>
    /// <param name="username"></param>
    /// <returns>Task<IEnumerable<Game>></returns>
    /// <exception cref="Exception"></exception>
    public Task<IEnumerable<Game>> GetAllGamesByUsernameAsync(string username)
    {
        User? user = userDao.GetByUsernameAsync(username).Result;
        if (user == null)
            throw new Exception("No user found");
        return gameDao.GetGamesByUsername(username);
    }
    
    /// <summary>
    /// Method that fetches a Game by its Id
    /// It returns Exception, if there is no Game found
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Task<Game?></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Game?> GetGameByIdAsync(int id)
    {
        Game? game = await gameDao.GetGameByIdAsync(id);
        if (game == null)
        {
            throw new Exception($"Game with id {id} not found");
        }
        return game;
    }
}