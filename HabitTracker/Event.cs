using System;

namespace HabitTracker
{
  public abstract class StreakResult
  {
    public Guid _user_id;

    public StreakResult(Guid user_id)
    {
      this._user_id = user_id;
    }
  }

  public class Dominating : StreakResult
  {
    public Dominating(Guid user_id) : base(user_id) {  }
  }
  public class Workaholic : StreakResult
  {
    public Workaholic(Guid user_id) : base(user_id) {  }
  }
  public class EpicComeback : StreakResult
  {
    public EpicComeback(Guid user_id) : base(user_id) {  }
  }
}