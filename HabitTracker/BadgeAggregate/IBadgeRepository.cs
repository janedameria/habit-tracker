using System;
using System.Collections.Generic;


namespace HabitTracker 
{
  public interface IBadgeRepository
  {
    List<Badge> FindByUserID(Guid user_id);
    void Create(Badge badge);
    bool CheckBadge(Guid user_id, string badge);
  }
}