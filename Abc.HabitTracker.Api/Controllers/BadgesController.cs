using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HabitTracker.Database.Postgres;
using HabitTracker;
using Npgsql;

namespace Abc.HabitTracker.Api.Controllers
{
  [ApiController]
  public class BadgesController : ControllerBase
  {
    private readonly ILogger<BadgesController> _logger;
    private string connString;


    public BadgesController(ILogger<BadgesController> logger)
    {
      _logger = logger;
      connString = "Host=localhost;Username=postgres;Password=test;Database=habittracker;Port=5432";
    }

    [HttpGet("api/v1/users/{userID}/badges")]
    public ActionResult<IEnumerable<BadgeAPI>> All(Guid userID)
    {
      
      NpgsqlConnection _connection = new NpgsqlConnection(connString);
      _connection.Open();

      IBadgeRepository repo = new PostgresBadgeRepository(_connection, null);
      List<Badge> badge = repo.FindByUserID(userID);
      List<BadgeAPI> badgeApi = new List<BadgeAPI>();

      for(int i = 0; i<badge.Count; i++)
      {
        BadgeAPI ba = new BadgeAPI
        {
          ID = badge[i].ID,
          Name = badge[i].Name,
          Description = badge[i].Description,
          CreatedAt = badge[i].DateTimeBadge,
          UserID = badge[i].User_id

        };
        badgeApi.Add(ba);
      }
      return badgeApi;
    }
  }
}
