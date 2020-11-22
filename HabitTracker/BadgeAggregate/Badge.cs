using System;

namespace HabitTracker
{

  public class Badge
  {
    private Guid id;
    private String name;
    private String description;
    private Guid user_id;
    private DateTime datetime;

    public Badge(Guid id, String name, String description, Guid user_id, DateTime datetime)
    {
      this.id = id;
      this.name = name;
      this.description = description;
      this.user_id = user_id;
      this.datetime = datetime;
    }
    public Guid ID
    {
      get
      {
        return id;
      }
    }

    public String Name
    {
      get
      {
        return name;
      }
    }
    public String Description
    {
      get
      {
        return description;
      }
    }
    public Guid User_id
    {
      get
      {
        return user_id;
      }
    }
    public DateTime DateTimeBadge
    {
      get
      {
        return datetime;
      }
    }
    public override bool Equals(object obj)
    {
      return obj is Badge badge &&
             id.Equals(badge.id) &&
             name == badge.name &&
             description == badge.description &&
             user_id.Equals(badge.user_id) &&
             datetime == badge.datetime;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(id, name, description, user_id, datetime);
    }
  }
}