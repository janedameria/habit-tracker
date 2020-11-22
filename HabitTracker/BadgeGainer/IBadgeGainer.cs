using System;
using HabitTracker;

namespace HabitTracker.BadgeGainer
{
  public interface IBadgeGainer
  {
    Badge Gain(Guid UserID);
  }
}