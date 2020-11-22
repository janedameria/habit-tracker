using System;

using System.Text.Json.Serialization;

namespace HabitTracker
{
  public class Days 
  {
    private string day;

    public Days(string days)
    {
      if(!Validate(days))
      {
        throw new Exception("Please input in ddd days format. Ex: Mon, Tue, etc.");
      }
      this.day = days;
    }
    
    public String Day
    {
      get{
        return this.day;
      }
    }

    public override bool Equals(object obj)
    {
      var o = obj as Days;
      if (o == null) return false;
      return o.day == this.day;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(day);
    }

    private bool Validate(string days)
    {
      if(days.Length != 3) return false;
      
      for(int i = 0; i<7 ; i++)
      {
        if(days == DateTime.Now.AddDays(i).ToString("ddd"))
        {
          return true;
        }
      }
      return false;
    }
  }
}