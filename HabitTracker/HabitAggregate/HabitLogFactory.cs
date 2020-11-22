using System;
using HabitTracker.BadgeGainer;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HabitTracker
{
  public class HabitLogFactory
  {

    public static HabitLog Create(Guid id, Guid user_id, string lastState = "", IBadgeRepository badgeRepository = null)
    {
      BadgeHandler dominating = new DominatingHandler(badgeRepository, new BadgeDominating());
      BadgeHandler Workaholic = new WorkaholicHandler(badgeRepository, new BadgeWorkaholic());
      BadgeHandler EpicComeback = new EpicComebackHandler(badgeRepository, new BadgeEpicComeback());

      HabitLog habitlog;
      habitlog = new HabitLog(id, user_id);
      habitlog.Attach(dominating);
      habitlog.Attach(Workaholic);
      habitlog.Attach(EpicComeback);

      if (lastState == null || lastState == "")
      {
        return habitlog;
      }
      
      HabitLogMemento memento = JsonSerializer.Deserialize<HabitLogMemento>(lastState);
      habitlog.LoadMemento(memento);
      return habitlog;
    }

  }
}