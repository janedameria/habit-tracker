using System;
using System.Collections.Generic;

using Npgsql;

namespace HabitTracker.Database.Postgres
{
  public class PostgresBadgeRepository : IBadgeRepository
  {
    private NpgsqlConnection _connection;
    private NpgsqlTransaction _transaction;

    public PostgresBadgeRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
      _connection = connection;
      _transaction = transaction;
    }
    public bool CheckBadge(Guid user_id, string badge)
    {
      List<string> name = new List<string>();
      string query = "select name from badge where user_id =  @user_id";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("user_id", user_id);
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
          while(reader.Read())
          {
            name.Add(reader.GetString(0));
          }
        }
      }
      foreach(string _name in name)
      {
        if(_name == badge) return true;
      }
      return false;
    }
    public void Create(Badge badge)
    {
      string query = "INSERT INTO Badge (id, name, description, user_id, created_at) VALUES(@id, @name, @description, @user_id, @created_at)";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("id", Guid.NewGuid());
        cmd.Parameters.AddWithValue("name", badge.Name);
        cmd.Parameters.AddWithValue("description", badge.Description);
        cmd.Parameters.AddWithValue("user_id", badge.User_id);
        cmd.Parameters.AddWithValue("created_at", badge.DateTimeBadge);
        cmd.ExecuteNonQuery();
      }
    }

    public List<Badge> FindByUserID(Guid user_id)
    {
      List<Badge> list = new List<Badge>();
      Badge b;
      string query = @"SELECT id, name, description, created_at FROM Badge WHERE user_id = @user_id";
      using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
      {
        cmd.Parameters.AddWithValue("user_id", user_id);
        NpgsqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          Guid id = reader.GetGuid(0);
          string name = reader.GetString(1);
          string desc = reader.GetString(2);
          DateTime date = reader.GetDateTime(3);
          b = new Badge(id, name, desc, user_id, date);
          list.Add(b);
        }
        reader.Close();
      }
      return list;
    }
  }
}