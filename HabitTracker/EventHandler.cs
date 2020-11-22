using System;
using HabitTracker.BadgeGainer;


namespace HabitTracker
{
  public abstract class BadgeHandler : IObserver<StreakResult>
  {
    protected IBadgeGainer _gainer;
    protected IBadgeRepository _badgerepo;

    public abstract void Update(StreakResult result);
    public BadgeHandler(IBadgeRepository badgerepo, IBadgeGainer gainer)
    {
      this._gainer = gainer;
      this._badgerepo = badgerepo;
    }

  }
  public class DominatingHandler : BadgeHandler
  {
    public DominatingHandler(IBadgeRepository badgerepo, IBadgeGainer gainer) : base(badgerepo, gainer) { }

    public override void Update(StreakResult r)
    {
      if (_badgerepo == null) return;

      Dominating dom = r as Dominating;
      if (dom == null) return;

      if(_badgerepo.CheckBadge(dom._user_id, "Dominating")) return;
      _badgerepo.Create(_gainer.Gain(dom._user_id));
      
    }
  }
  public class WorkaholicHandler : BadgeHandler
  {
    public WorkaholicHandler(IBadgeRepository badgerepo, IBadgeGainer gainer) : base(badgerepo, gainer) { }

    public override void Update(StreakResult r)
    {
      Workaholic work = r as Workaholic;
      if (work == null) return;
      
      if(_badgerepo.CheckBadge(work._user_id, "Workaholic")) return;
      _badgerepo.Create(_gainer.Gain(work._user_id));
    }
  }
  public class EpicComebackHandler : BadgeHandler
  {
    public EpicComebackHandler(IBadgeRepository badgerepo, IBadgeGainer gainer) : base(badgerepo, gainer) { }

    public override void Update(StreakResult r)
    {
      EpicComeback ec = r as EpicComeback;
      if (ec == null) return;
      if(_badgerepo.CheckBadge(ec._user_id, "Epic Comeback")) return;
           
      _badgerepo.Create(_gainer.Gain(ec._user_id));
    }
  }


}