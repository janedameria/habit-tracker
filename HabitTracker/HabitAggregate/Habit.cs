using System;
using System.Collections.Generic;

namespace HabitTracker
{

  public class Habit
  {
    private Guid id;
    private String name;
    private Days[] days_off;
    private DateTime created_at;
    private Guid user_id;
    private HabitLog log;


    public Habit(Guid id, String name, Days[] days_off, Guid user_id, DateTime created_at, HabitLog log = null)
    {
      if (isDaysDuplicateAnd7(days_off))
      {
        throw new Exception("Days Off Shouldnt be Duplicate and all weekdays");
      }
      if (name.Length < 2 || name.Length > 100)
      {
        throw new Exception("Habit name must be between 2 until 100 characters");
      }
      this.id = id;
      this.name = name;
      this.days_off = days_off;
      this.user_id = user_id;
      this.created_at = created_at;
      this.log = log;
    
    }
    private bool Validate(string days)
    {
      if (days.Length != 3) return false;

      for (int i = 0; i < 7; i++)
      {
        if (days == DateTime.Now.AddDays(i).ToString("ddd"))
        {
          return true;
        }
      }

      return false;
    }
    public Guid ID
    {
      get
      {
        return id;
      }
    }

    public string Name
    {
      get
      {
        return name;
      }
    }

    public Days[] DaysOff
    {
      get
      {
        return days_off;
      }
    }

    public DateTime CreatedAt
    {
      get
      {
        return created_at;
      }
    }

    public HabitLog HabitLog
    {
      get
      {
        return log;
      }
    }

    public Guid UserID
    {
      get
      {
        return user_id;
      }
    }


    public void DoHabit(DateTime now)
    {
        
      if(log == null) log = HabitLogFactory.Create(id, user_id);
      log.doHabit(now, this.days_off);

    }

    
    private bool isDaysDuplicateAnd7(Days[] days_off)
    {
      List<string> list = new List<string>();

      if (days_off.Length >= 7) return true;

      foreach (Days day in days_off)
      {
        if (list.Contains(day.Day)) return true;
        list.Add(day.Day);
      }
      return false;
    }



    public override bool Equals(object obj)
    {
      var h = obj as Habit;
      if (h == null) return false;

      return this.id == h.ID;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(id, name, days_off, user_id, created_at);
    }
  }

}