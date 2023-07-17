﻿namespace Shared.Model;

public class Lesson
{
    public int Id { get; set; }
    public DateTime DateAndTime { get; set; }
    public User Player { get; set; }
    public string Instructor { get; set; }  // Instructor name

    public Lesson(DateTime dateAndTime, User player, string instructor)
    {
        DateAndTime = dateAndTime;
        Player = player;
        Instructor = instructor;
    }
    private Lesson() {}
}