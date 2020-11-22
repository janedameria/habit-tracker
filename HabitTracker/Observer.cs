using System;

namespace HabitTracker
{
  public interface IObserver<T> 
  {
    void Update(T e);
  }
    public interface ISubject<T>
  {
    void Attach(IObserver<T> obs);
    void Broadcast(T e);
  }
}