using System;
using Xunit;
using Npgsql;
using HabitTracker;
using HabitTracker.BadgeGainer;

namespace abc.Test
{
  public class abcTest
  {
    public abcTest()
    {
    }
    [Fact]
    public void CheckHabit()
    {
      Guid habit_id = Guid.NewGuid();
      Guid user_id = Guid.NewGuid();
      DateTime now = DateTime.Now;

      Days[] dayoff = new Days[] { new Days("Mon"), new Days("Tue") };
      Habit h = new Habit(habit_id, "Nyanyi", dayoff, user_id, now);
      for (int i = 0; i < 2; i++)
      {
        h.DoHabit(DateTime.Now.AddDays(i));
      }

      for (int i = 4; i < 5; i++)
      {
        h.DoHabit(DateTime.Now.AddDays(i));
      }
      HabitLogMemento hm = (HabitLogMemento)h.HabitLog.GetMemento();
      Assert.Equal(2, hm.longest_streak);
      Assert.Equal(1, hm.current_streak);
      Assert.Equal(3, hm.log_count);
    }
    [Fact]
    public void CheckHabit2()
    {
      Guid habit_id = Guid.NewGuid();
      Guid user_id = Guid.NewGuid();
      DateTime now = DateTime.Now;

      Days[] dayoff = new Days[] { new Days("Mon"), new Days("Sun")};
      Habit h = new Habit(habit_id, "Nyanyi", dayoff, user_id, now);

      
      h.DoHabit(DateTime.Now.AddDays(0)); //Sun

      // h.DoHabit(DateTime.Now.AddDays(1)); //Mon
      h.DoHabit(DateTime.Now.AddDays(3)); //Wed
      h.DoHabit(DateTime.Now.AddDays(4)); //Thu

      h.DoHabit(DateTime.Now.AddDays(6)); //Sun
      h.DoHabit(DateTime.Now.AddDays(7)); //Sun

      HabitLogMemento hm = (HabitLogMemento)h.HabitLog.GetMemento();
      Assert.Equal(2, hm.longest_streak); 
      Assert.Equal(2, hm.current_streak);
      Assert.Equal(5, hm.log_count);
    }
    // [Fact]
    // public void FindByUserID()
    // {

    //   NpgsqlConnection _connection = new NpgsqlConnection(connString);
    //   _connection.Open();

    //   IHabitRepository repo = new PostgresHabitRepository(_connection, null);
    //   IBadgeRepository bRepo = new PostgresBadgeRepository(_connection, null);

    //   Guid habit_id = Guid.NewGuid();
    //   Guid user_id = Guid.NewGuid();
    //   repo.Create(habit_id, user_id, "Dancing", dayoff);
    //   Habit dancing = HabitFactory.Create(habit_id, "Dancing", dayoff, user_id);


    //   Guid habit_id2 = Guid.NewGuid();
    //   repo.Create(habit_id2, user_id, "Running", dayoff);
    //   Habit running = HabitFactory.Create(habit_id2, "Running", dayoff, user_id);

    //   List<Guid> list = repo.FindByUserID(user_id);
    //   List<Habit> h = new List<Habit>();
    //   for(int i = 0 ; i< list.Count ; i ++)
    //   {
    //     h.Add(repo.FindByID(list[0], user_id));
    //     if(i == 0)
    //     {
    //         Assert.Equal(dancing, h[0]);
    //     }
    //     if(i == 1)
    //     {
    //         Assert.Equal(dancing, h[0]);
    //     }
    //   }


    //   _connection.Close();
    // }
  }
}
