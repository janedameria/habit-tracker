using System;
using System.Collections.Generic;

using Npgsql;
using NpgsqlTypes;

using System.Text.Json;
using System.Text.Json.Serialization;
using HabitTracker;

namespace HabitTracker.Database.Postgres
{
  public class PostgresUnitOfWork : UnitOfWork
  {
    private NpgsqlConnection _connection;
    private NpgsqlTransaction _transaction;

    private IBadgeRepository badgeRepo;
    private IHabitRepository habitRepo;

    public IBadgeRepository BadgeRepo
    {
      get
      {
        if (badgeRepo == null)
        {
          badgeRepo = new PostgresBadgeRepository(_connection, _transaction);
        }
        return badgeRepo;
      }
    }

    public IHabitRepository HabitRepo
    {
      get
      {
        if (habitRepo == null)
        {
          habitRepo = new PostgresHabitRepository(_connection, _transaction);
        }
        return habitRepo;
      }
    }

    public PostgresUnitOfWork(string connectionString)
    {
      _connection = new NpgsqlConnection(connectionString);
      _connection.Open();
      _transaction = _connection.BeginTransaction();
    }

    public void Commit()
    {
      _transaction.Commit();
    }

    public void Rollback()
    {
      _transaction.Rollback();
    }

    private bool disposed = false;
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        if (disposing)
        {
          _connection.Close();
        }

        disposed = true;
      }
    }


  }
}