using System;
using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;
using System.Text.Json;
namespace HabitTracker.Database.Postgres
{
  public class PostgresHabitRepository : IHabitRepository
  {
    private NpgsqlConnection _connection;
    private NpgsqlTransaction _transaction;

    public PostgresHabitRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
      _connection = connection;
      _transaction = transaction;
    }

    public void Create(Habit habit)
    {

      string query = "INSERT INTO habit (id, name, user_id, created_at) VALUES(@id, @name, @user_id, @created_at)";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("id", habit.ID);
        cmd.Parameters.AddWithValue("name", habit.Name);
        cmd.Parameters.AddWithValue("user_id", habit.UserID);
        cmd.Parameters.AddWithValue("created_at", habit.CreatedAt);
        cmd.ExecuteNonQuery();
      }

      CreateHabitDaysOff(habit.ID, habit.DaysOff);
    }

    private void CreateHabitDaysOff(Guid habitid, Days[] daysoff)
    {
      for (int i = 0; i < daysoff.Length; i++)
      {
        string query = "INSERT INTO days_off (habit_id, off_days) VALUES(@habit_id, @off_days)";
        using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
        {
          cmd.Parameters.AddWithValue("habit_id", habitid);
          cmd.Parameters.AddWithValue("off_days", daysoff[i].Day);
          cmd.ExecuteNonQuery();
        }
      }
    }
    private void DeleteDaysOff(Guid habit_id)
    {
      String query = "UPDATE days_off SET deleted_at = @deleted_at where habit_id = @habit_id AND deleted_at is null";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("habit_id", habit_id);
        cmd.Parameters.AddWithValue("deleted_at", DateTime.Now);
        cmd.ExecuteNonQuery();
      }
    }
    public void Delete(Guid habit_id, Guid user_id)
    {

      string query = "UPDATE logs SET deleted_at = @deleted_at where habit_id = @habit_id AND deleted_at is null ";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("habit_id", habit_id);
        cmd.Parameters.AddWithValue("deleted_at", DateTime.Now);
        cmd.ExecuteNonQuery();
      }

      DeleteDaysOff(habit_id);

      query = "UPDATE habit SET deleted_at = @deleted_at where user_id = @user_id AND id = @habit_id AND deleted_at is null";

      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {

        cmd.Parameters.AddWithValue("user_id", user_id);
        cmd.Parameters.AddWithValue("habit_id", habit_id);
        cmd.Parameters.AddWithValue("deleted_at", DateTime.Now);
        cmd.ExecuteNonQuery();
      }

    }

    public void DoHabit(Habit habit, DateTime now)
    {
      string query = "INSERT INTO logs(id, habit_id, user_id, logdate, state) VALUES(@id, @habit_id,  @user_id, @logdate, @state)";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {

        cmd.Parameters.AddWithValue("id", Guid.NewGuid());
        cmd.Parameters.AddWithValue("habit_id", habit.ID);
        cmd.Parameters.AddWithValue("user_id", habit.UserID);
        cmd.Parameters.AddWithValue("logdate", now);

        string jsonString = JsonSerializer.Serialize(habit.HabitLog.GetMemento());
        cmd.Parameters.Add(new NpgsqlParameter("state", NpgsqlDbType.Jsonb) { Value = jsonString });
        cmd.ExecuteNonQuery();
      }

      query = "SELECT count(1) FROM logs WHERE habit_id = @habit_id";
      int count = 0;
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("habit_id", habit.ID);
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
          if (reader.Read())
          {
            count = reader.GetInt32(0);
          }

          reader.Close();
        }
      }
    
      if (count % 50 == 0)
      {
        createHabitSnapshot(habit.ID, count, habit.UserID);
      }

    }

    private void createHabitSnapshot(Guid habit_id, int count, Guid userid)
    {
      string query = "SELECT id, logdate FROM logs WHERE habit_id = @habit_id ORDER BY logdate DESC LIMIT 1";

      Guid lastlogid;
      DateTime lastLogCreatedAt;

      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("habit_id", habit_id);
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
          if (reader.Read())
          {
            lastlogid = reader.GetGuid(0);
            lastLogCreatedAt = reader.GetDateTime(1);
          }
          else
          {
            throw new Exception("last log not found");
          }
        }
      }

      query = "INSERT INTO habit_snapshot (id, habit_id, log_count, user_id, last_log_id, last_log_created_at) VALUES(@id, @habit_id, @log_count, @user_id, @last_log_id, @last_log_created_at)";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("id", Guid.NewGuid());
        cmd.Parameters.AddWithValue("habit_id", habit_id);
        cmd.Parameters.AddWithValue("log_count", count);
        cmd.Parameters.AddWithValue("last_log_id", lastlogid);
        cmd.Parameters.AddWithValue("user_id", userid);
        cmd.Parameters.AddWithValue("last_log_created_at", lastLogCreatedAt);

        cmd.ExecuteNonQuery();
      }
    }
    public Habit FindHabitByID(Guid habit_id, Guid user_id)
    {
      string query = "SELECT name, created_at FROM habit WHERE user_id = @user_id AND id = @id AND deleted_at is null";
      string name = "";
      DateTime timestamp = DateTime.Now;

      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("user_id", user_id);
        cmd.Parameters.AddWithValue("id", habit_id);

        NpgsqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
          name = reader.GetString(0);
          timestamp = reader.GetDateTime(1);
        }
        reader.Close();
      }
      if (name == "") throw new Exception("Habit doesnt exist");
      List<Days> offDays = new List<Days>();

      query = "SELECT off_days FROM days_off WHERE habit_id = @habit_id AND deleted_at is null";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("habit_id", habit_id);

        NpgsqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          offDays.Add(new Days(reader.GetString(0)));
        }
        reader.DisposeAsync();
      }
      HabitLog log = FindByID(habit_id, user_id);
      return new Habit(habit_id, name, offDays.ToArray(), user_id, timestamp, log);

    }
    private HabitLog FindByID(Guid habit_id, Guid user_id)
    {

      List<DateTime> logdays = new List<DateTime>();
      string lastState = "";

      string query = "SELECT state FROM logs WHERE habit_id = @habit_id AND deleted_at is null ORDER BY logDate DESC limit 1";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("habit_id", habit_id);

        NpgsqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
          lastState = reader.GetString(0);
        }
        reader.Close();
      }
      return HabitLogFactory.Create(habit_id, user_id, lastState, new PostgresBadgeRepository(_connection, _transaction));
    }

    public List<Habit> FindByUserID(Guid user_id)
    {
      string query = "SELECT id FROM habit WHERE user_id = @user_id AND deleted_at is null";
      List<Habit> habits = new List<Habit>();
      List<Guid> ids = new List<Guid>();
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("user_id", user_id);
        NpgsqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          ids.Add(reader.GetGuid(0));
        }

        reader.DisposeAsync();
      }

      foreach (Guid id in ids)
      {
        habits.Add(FindHabitByID(id, user_id));
      }

      return habits;
    }

    public void Update(Habit habit, string name, Days[] daysoff)
    {
      if(habit == null)
      {
        throw new Exception("Habit doesnt exists");
      }
      string query = "UPDATE habit SET name = @name WHERE id = @habit_id AND user_id = @user_id AND deleted_at is null";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("habit_id", habit.ID);
        cmd.Parameters.AddWithValue("user_id", habit.UserID);
        cmd.ExecuteNonQuery();

      }
      DeleteDaysOff(habit.ID);
      CreateHabitDaysOff(habit.ID, daysoff);
    }
  }
}