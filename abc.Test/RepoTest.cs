using System;
using Xunit;
using Npgsql;
using HabitTracker.Database.Postgres;

using System.Collections.Generic;

using HabitTracker;

namespace abc.Test
{
  public class RepoTest
  {
    private string connString;

    private Days[] dayoff = new Days[] { new Days("Mon"), new Days("Tue") };

    public RepoTest()
    {
      connString = "Host=localhost;Username=postgres;Password=test;Database=habittracker;Port=5432";
    }

    // [Fact]
    public void GiveDominating()
    {

      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();

      IHabitRepository repo = new PostgresHabitRepository(_connection, null);
      IBadgeRepository bRepo = new PostgresBadgeRepository(_connection, null);

      Guid id = new Guid("41684bad-554b-4194-8fc2-d192273b9aa8");
      Guid userID = new Guid("e28c8034-f90d-48aa-8e55-2aad1282f3bd");
      Habit habit = repo.FindHabitByID(id, userID);

      for (int i = 0; i < 4; i++)
      {
        DateTime now = DateTime.Now.AddDays(i);
        habit.DoHabit(now);
        repo.DoHabit(habit, now);

      }
      // Habit habit2 = repo.FindHabitByID(id, userID);
      // Badge badge = bRepo.FindByUserID(userID);
      // Assert.Equal("Dominating", badge.Name);

      _connection.Close();
    }

    // [Fact]
    public void GiveEpicComeback()
    {

      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();

     IHabitRepository repo = new PostgresHabitRepository(_connection, null);
     
      Guid id = new Guid("41684bad-554b-4194-8fc2-d192273b9aa8");
      Guid userID = new Guid("e28c8034-f90d-48aa-8e55-2aad1282f3bd");
    
      Habit habit = repo.FindHabitByID(id, userID);
      DateTime now = DateTime.Now;
      habit.DoHabit(now);
      repo.DoHabit(habit, now);

      for (int i = 10; i < 20; i++)
      {
        now = DateTime.Now.AddDays(i);
        habit.DoHabit(now);
        repo.DoHabit(habit, now);
      }

      Habit habit2 = repo.FindHabitByID(id, userID);
      _connection.Close();
      HabitLogMemento hm = (HabitLogMemento)habit2.HabitLog.GetMemento();
      // Habit habit2 = repo.FindHabitByID(id, userID);
      // Badge badge = bRepo.FindByUserID(userID);
      // Assert.Equal("Dominating", badge.Name);

      _connection.Close();
    }

    // [Fact]
    public void GiveWorkaholic()
    {

    //  IHabitRepository repo = new PostgresHabitRepository(_connection, null);
    //   Habit habit = repo.FindHabitByID(id, userID);
    //   //DayOff : Fri - Wed

    //   for (int i = 0; i < 6; i++)
    //   {
    //     DateTime now = DateTime.Now.AddDays(i);
    //     habit.DoHabit(now);
    //     repo.DoHabit(habit, now);
    //   }

    //    for (int i = 7; i < 12; i++)
    //   {
    //     DateTime now = DateTime.Now.AddDays(i);
    //     habit.DoHabit(now);
    //     repo.DoHabit(habit, now);
    //   }

    //   Habit habit2 = repo.FindHabitByID(id, userID);
    //   _connection.Close();
    //   HabitLogMemento hm = (HabitLogMemento)habit2.HabitLog.GetMemento();

    //   return new HabitAPI
    //   {
    //     ID = hm.id,
    //     Name = habit.Name,
    //     DaysOff = hm.days_off,
    //     LogCount = hm.log_count,
    //     CurrentStreak = hm.current_streak,
    //     LongestStreak = hm.longest_streak,
    //     Logs = hm.logs,
    //     UserID = hm.user_id,
    //     CreatedAt = habit.CreatedAt
    //   };
    }
  }
}
