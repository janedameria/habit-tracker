using System;
using System.Collections.Generic;


namespace HabitTracker 
{
  public interface IHabitRepository
  {
    
    List<Habit> FindByUserID(Guid user_id);
    void Create(Habit habit);
    void Update(Habit habit, string name, Days[] daysoff);
    Habit FindHabitByID(Guid habit_id, Guid user_id);
    void Delete(Guid habit_id, Guid user_id);
    void DoHabit(Habit habit, DateTime now);
    
  }
}