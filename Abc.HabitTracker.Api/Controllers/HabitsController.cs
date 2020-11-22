using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HabitTracker.Database.Postgres;
using HabitTracker;
using Npgsql;

namespace Abc.HabitTracker.Api.Controllers
{
  [ApiController]
  public class HabitsController : ControllerBase
  {
    private readonly ILogger<HabitsController> _logger;

    private string connString;
    public HabitsController(ILogger<HabitsController> logger)
    {
      _logger = logger;
      connString = "Host=localhost;Username=postgres;Password=test;Database=habittracker;Port=5432";
    }

    [HttpPost("api/v1/users/{userID}/habits")]
    public ActionResult<HabitAPI> AddNewHabit(Guid userID, [FromBody] RequestData data)
    {

      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();

      IHabitRepository repo = new PostgresHabitRepository(_connection, null);

      Guid habit_id = Guid.NewGuid();
      DateTime now = DateTime.Now;


      Habit habit = new Habit(habit_id, data.Name, changeToDays(data.DaysOff), userID, now);
      repo.Create(habit);

      Habit habit3 = repo.FindHabitByID(habit_id, userID);
      HabitLogMemento hm = (HabitLogMemento)habit3.HabitLog.GetMemento();

      _connection.Close();


      return new HabitAPI
      {
        ID = habit3.ID,
        Name = habit3.Name,
        DaysOff = changeToString(habit3.DaysOff),
        LogCount = hm.log_count,
        CurrentStreak = hm.current_streak,
        LongestStreak = hm.longest_streak,
        Logs = hm.logs,
        UserID = hm.user_id,
        CreatedAt = habit3.CreatedAt,
      };


    }
    [HttpGet("api/v1/users/{userID}/habits/{id}")]
    public ActionResult<HabitAPI> Get(Guid userID, Guid id)
    {
      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();

      IHabitRepository repo = new PostgresHabitRepository(_connection, null);
      Habit h = repo.FindHabitByID(id, userID);
      HabitLogMemento hm = (HabitLogMemento)h.HabitLog.GetMemento();
      _connection.Close();

      return new HabitAPI
      {
        ID = h.ID,
        Name = h.Name,
        DaysOff = changeToString(h.DaysOff),
        LogCount = hm.log_count,
        CurrentStreak = hm.current_streak,
        LongestStreak = hm.longest_streak,
        Logs = hm.logs,
        UserID = h.UserID,
        CreatedAt = h.CreatedAt
      };

    }

    [HttpGet("api/v1/users/{userID}/habits")]
    public ActionResult<List<HabitAPI>> All(Guid userID)
    {
      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();

      IHabitRepository repo = new PostgresHabitRepository(_connection, null);
      List<Habit> list = repo.FindByUserID(userID);

      _connection.Close();
      List<HabitAPI> listHabit = new List<HabitAPI>();


      for (int i = 0; i < list.Count; i++)
      {
        HabitLogMemento hm = (HabitLogMemento)list[i].HabitLog.GetMemento();
        HabitAPI habitAPI = new HabitAPI
        {
          ID = list[i].ID,
          Name = list[i].Name,
          DaysOff = changeToString(list[i].DaysOff),
          LogCount = hm.log_count,
          CurrentStreak = hm.current_streak,
          LongestStreak = hm.longest_streak,
          Logs = hm.logs,
          UserID = list[i].UserID,
          CreatedAt = list[i].CreatedAt
        };
        listHabit.Add(habitAPI);
      }
      return listHabit;

    }

    [HttpPost("api/v1/users/{userID}/habits/{id}/logs")]
    public ActionResult<HabitAPI> Log(Guid userID, Guid id)
    {
      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();

      IHabitRepository repo = new PostgresHabitRepository(_connection, null);
      Habit habit = repo.FindHabitByID(id, userID);
      DateTime now = DateTime.Now;

      habit.DoHabit(now);
      repo.DoHabit(habit, now);


      Habit habit2 = repo.FindHabitByID(id, userID);
      _connection.Close();
      HabitLogMemento hm = (HabitLogMemento)habit2.HabitLog.GetMemento();
      return new HabitAPI
      {
        ID = hm.id,
        Name = habit2.Name,
        DaysOff = changeToString(habit2.DaysOff),
        LogCount = hm.log_count,
        CurrentStreak = hm.current_streak,
        LongestStreak = hm.longest_streak,
        Logs = hm.logs,
        UserID = hm.user_id,
        CreatedAt = habit2.CreatedAt
      };
    }
    [HttpPut("api/v1/users/{userID}/habits/{id}")]
    public ActionResult<HabitAPI> UpdateHabit(Guid userID, Guid id, [FromBody] RequestData data)
    {
      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();
      IHabitRepository repo = new PostgresHabitRepository(_connection, null);

      Habit h = repo.FindHabitByID(id, userID);
      Habit hn = new Habit(id, data.Name, changeToDays(data.DaysOff), userID, h.CreatedAt);
      repo.Update(h, hn.Name, hn.DaysOff);
      h = repo.FindHabitByID(id, userID);
      _connection.Close();

      HabitLogMemento hm = (HabitLogMemento)h.HabitLog.GetMemento();

      return new HabitAPI
      {
        ID = hm.id,
        Name = h.Name,
        DaysOff = changeToString(h.DaysOff),
        LogCount = hm.log_count,
        CurrentStreak = hm.current_streak,
        LongestStreak = hm.longest_streak,
        Logs = hm.logs,
        UserID = hm.user_id,
        CreatedAt = h.CreatedAt
      };
    }

    [HttpDelete("api/v1/users/{userID}/habits/{id}")]
    public ActionResult<HabitAPI> DeleteHabit(Guid userID, Guid id)
    {
      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();

      IHabitRepository repo = new PostgresHabitRepository(_connection, null);

      Habit h = repo.FindHabitByID(id, userID);
      HabitLogMemento hm = (HabitLogMemento)h.HabitLog.GetMemento();
      repo.Delete(id, userID);
      _connection.Close();
      return new HabitAPI
      {
        ID = hm.id,
        Name = h.Name,
        DaysOff = changeToString(h.DaysOff),
        LogCount = hm.log_count,
        CurrentStreak = hm.current_streak,
        LongestStreak = hm.longest_streak,
        Logs = hm.logs,
        UserID = hm.user_id,
        CreatedAt = h.CreatedAt
      };
    }

    private Days[] changeToDays(string[] days)
    {
      List<Days> _days = new List<Days>();
      for (int i = 0; i < days.Length; i++)
      {
        _days.Add(new Days(days[i]));
      }
      return _days.ToArray();
    }
    private string[] changeToString(Days[] days)
    {
      List<string> _days = new List<string>();
      for (int i = 0; i < days.Length; i++)
      {
        _days.Add(days[i].Day);
      }
      return _days.ToArray();
    }
  }
}
