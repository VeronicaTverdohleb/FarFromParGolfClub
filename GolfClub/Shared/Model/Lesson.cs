﻿namespace Shared.Model;
/// <summary>
/// Model Class used in Lesson-related use cases and for DB creation
/// </summary>
public class Lesson
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string Time { get; set; }
    public User Player { get; set; }
    public string Instructor { get; set; }  // Instructor name

    public Lesson(DateOnly date, string time, User player, string instructor)
    {
        Date = date;
        Time = time;
        Player = player;
        Instructor = instructor;
    }
    private Lesson() {}
}