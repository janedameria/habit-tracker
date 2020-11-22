using System;

namespace HabitTracker.BadgeGainer
{
  public class BadgeDominating : IBadgeGainer
  {
    public Badge Gain(Guid UserID)
    {
      return BadgeFactory.CreateDominating(UserID);
    }
   
  }
  
  public class BadgeWorkaholic : IBadgeGainer
  {
    public Badge Gain(Guid UserID)
    {
      return BadgeFactory.CreateWorkaholic(UserID);
    }
  }
  
  public class BadgeEpicComeback : IBadgeGainer
  {
    public Badge Gain(Guid UserID)
    {
     return BadgeFactory.CreateEpicComeback(UserID);
    }
  }
}