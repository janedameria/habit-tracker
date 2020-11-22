using System;

namespace HabitTracker
{
  public class BadgeFactory
  {
    
    public static Badge CreateDominating(Guid user_id)
    {
      Badge badge = new Badge(Guid.NewGuid(), "Dominating", "4+Streak", user_id, DateTime.Now);


      return badge;
    }
    
    public static Badge CreateWorkaholic(Guid user_id)
    {
      Badge badge =  new Badge(Guid.NewGuid(), "Workaholic", "4+Doing some works on daysoff ", user_id, DateTime.Now);

      return badge;
    }
    
    public static Badge CreateEpicComeback(Guid user_id)
    {
      Badge badge = new Badge(Guid.NewGuid(), "Epic Comeback", "10 streak after 10 days without logging ", user_id, DateTime.Now);
      return badge;
    }
  }
}