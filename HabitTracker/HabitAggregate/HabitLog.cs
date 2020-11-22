using System;
using System.Collections.Generic;
namespace HabitTracker
{
  public class HabitLogMemento
  {

    public Guid id { set; get; }
    public Guid user_id { set; get; }
    public int current_streak { set; get; }
    public int longest_streak { set; get; }
    public int log_count { set; get; }
    public List<DateTime> logs { set; get; }

    public HabitLogMemento() { }
    public HabitLogMemento(Guid id, Guid user_id, int current_streak, int longest_streak, int log_count, List<DateTime> logs)
    {
      this.id = id;
      this.user_id = user_id;

      this.current_streak = current_streak;
      this.longest_streak = longest_streak;
      this.logs = logs;
      this.log_count = log_count;
    }

  }
  public class HabitLog : ISubject<StreakResult>
  {
    private Guid id;
    private int current_streak;
    private int longest_streak;
    private int log_count;
    private List<DateTime> logs = new List<DateTime>();
    private Guid user_id;
    private bool dominatingBadge;
    private bool epicomebackBadge;
    private bool workaholicBadge;
    private int count_db;

    public HabitLog(Guid id, Guid user_id)
    {
      this.id = id;
      this.user_id = user_id;

      this.current_streak = 0;
      this.longest_streak = 0;
      this.log_count = 0;
      this.count_db = 0;

      this.dominatingBadge = false;
      this.workaholicBadge = false;
      this.epicomebackBadge = false;
    }

    protected List<IObserver<StreakResult>> _observers = new List<IObserver<StreakResult>>();
    public void Attach(IObserver<StreakResult> obs)
    {
      _observers.Add(obs);
    }

    public void Broadcast(StreakResult e)
    {
      foreach (var obs in _observers)
      {
        obs.Update(e);
      }
    }
    private void GiveWorkaholic()
    {
      Broadcast(new Workaholic(this.user_id));
      workaholicBadge = true;

    }
    private bool checkIfDateinDaysOff(DateTime LogDate, Days[] days_off)
    {

      foreach (Days days in days_off)
      {
        if (days.Day == LogDate.ToString("ddd"))
        {
          return true;
        }
      }

      return false;
    }
    public void doHabit(DateTime loggingdate, Days[] days_off)
    {
      if (log_count == 0)
      {
        logs.Add(loggingdate);
        log_count++;
        current_streak++;
        longest_streak = current_streak;
        return;
      }
      if (checkIfDateinDaysOff(loggingdate, days_off))
      {
        logs.Add(loggingdate);
        log_count++;
        count_db++;
        if (count_db == 10 && !workaholicBadge) GiveWorkaholic();
        current_streak++;
        longest_streak = current_streak > longest_streak ? current_streak : longest_streak;
        return;
      }

      TimeSpan diff = loggingdate - logs[log_count - 1];
      int d = diff.Days;
      bool check;

      for (int i = 1; i < d; i++)
      {
        check = checkIfDateinDaysOff(logs[log_count - 1].AddDays(i), days_off);
        if (!check)
        {
          current_streak = 0;
          break;
        }
      }

      logs.Add(loggingdate);
      log_count++;
      current_streak++;
      longest_streak = current_streak > longest_streak ? current_streak : longest_streak;

      if (logs.Count >= 4 && !dominatingBadge)
      {
        GiveDominatingBadge();
      }

      if (logs.Count >= 11 && !epicomebackBadge)
      {
        GiveEpicComeback();
      }
    }

    private void CheckComeBack(int index)
    {
      int count = 0;
      for (int i = index; i < logs.Count; i++)
      {
        if (i + 1 != logs.Count && (int)(logs[i] - logs[i + 1]).Days == -1)
        {
          count++;
          if (count == 9)
          {
            Broadcast(new EpicComeback(this.user_id));
            epicomebackBadge = true;
            return;
          }
        }
        else count = 0;
      }
    }

    private void GiveEpicComeback()
    {
      for (int i = 0; i < logs.Count; i++)
      {
        if (i + 1 != logs.Count && (int)(logs[i] - logs[i + 1]).Days == -10)
        {
          CheckComeBack(i + 1);
        }
      }
    }


    private void GiveDominatingBadge()
    {
      int count = 0;
      for (int i = 0; i < logs.Count; i++)
      {
        if (i + 1 != logs.Count && (int)(logs[i] - logs[i + 1]).Days == -1)
        {
          count++;
          if (count == 3)
          {
            Broadcast(new Dominating(this.user_id));
            dominatingBadge = true;
            return;
          }
        }
        else count = 0;
      }

    }

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      return base.ToString();
    }

    public object GetMemento()
    {
      return new HabitLogMemento(id, user_id, current_streak, longest_streak, log_count, logs);
    }

    public void LoadMemento(object memento)
    {
      var m = memento as HabitLogMemento;
      if (m == null) throw new Exception("wrong memento");
      this.id = m.id;
      this.user_id = m.user_id;
      this.current_streak = m.current_streak;
      this.longest_streak = m.longest_streak;
      this.log_count = m.log_count;
      this.logs = m.logs;
    }
  }
}

