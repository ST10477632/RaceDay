using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Database;
using RaceDay.Models;
using System.Text.Json;

namespace RaceDay.Controllers
{//start of namespace
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {//start of class



        //allowed event types
        private static readonly string[] valid_types = { "Run", "Walk", "Cycle" };

        //returns Run, Walk or Cycle (any capital letters) or null when the type is wrong
        private static string? clean_type(string event_type)
        {
            return valid_types.FirstOrDefault(t => t.Equals(event_type.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        
        [HttpGet]
        public string get_events()
        {//start of get_events
            Manage_Events event_data = new Manage_Events();
            return JsonSerializer.Serialize(event_data.get_upcoming());
        }//end of get_events

        
        [HttpGet("{id:int}")]
        public string get_event(int id)
        {//start of get_event
            Manage_Events event_data = new Manage_Events();
            Event? ev = event_data.get_event(id);

            if (ev == null)
            {
                Response.StatusCode = 404;
                return "Event does not exist...";
            }
            return JsonSerializer.Serialize(ev);
        }//end of get_event

        
        [HttpGet("my")]
        public string get_my_events()
        {//start of get_my_events
            string denied = check_access.check(HttpContext, "Organiser");
            if (denied != "") return denied;

            Manage_Events event_data = new Manage_Events();
            return JsonSerializer.Serialize(event_data.get_by_organiser(check_access.user_id(HttpContext)));
        }//end of get_my_events

        
        [HttpPost]
        public string create_event(string event_name, DateTime event_date, string location, string description,
            double distance, string event_type, string? route_info = null)
        {//start of create_event
            string denied = check_access.check(HttpContext, "Organiser");
            if (denied != "") return denied;

            string? type = clean_type(event_type);
            if (type == null || distance <= 0 || event_name.Trim() == "" || location.Trim() == "")
            {
                Response.StatusCode = 400;
                return "Name and location are required, distance must be above 0 and type must be Run, Walk or Cycle...";
            }

            Manage_Events event_data = new Manage_Events();
            string message = event_data.add_event(check_access.user_id(HttpContext), event_name.Trim(), event_date,
                location.Trim(), description.Trim(), route_info, distance, type);

            if (message.Contains("successfully")) Response.StatusCode = 201;
            else Response.StatusCode = 400;

            return message;
        }//end of create_event

        
        [HttpPut("{id:int}")]
        public string update_event(int id, string event_name, DateTime event_date, string location,
            string description, double distance, string event_type, string? route_info = null)
        {//start of update_event
            string denied = check_access.check(HttpContext, "Organiser");
            if (denied != "") return denied;

            Manage_Events event_data = new Manage_Events();
            Event? ev = event_data.get_event(id);
            if (ev == null)
            {
                Response.StatusCode = 404;
                return "Event does not exist...";
            }
            if (ev.organiser_id != check_access.user_id(HttpContext))
            {
                Response.StatusCode = 403;
                return "Access denied...";
            }

            string? type = clean_type(event_type);
            if (type == null || distance <= 0 || event_name.Trim() == "" || location.Trim() == "")
            {
                Response.StatusCode = 400;
                return "Name and location are required, distance must be above 0 and type must be Run, Walk or Cycle...";
            }

            string message = event_data.update_event(id, event_name.Trim(), event_date, location.Trim(),
                description.Trim(), route_info, distance, type);

            if (message.Contains("successfully")) Response.StatusCode = 200;
            else Response.StatusCode = 400;

            return message;
        }//end of update_event

        
        [HttpDelete("{id:int}")]
        public string delete_event(int id)
        {//start of delete_event
            string denied = check_access.check(HttpContext, "Organiser");
            if (denied != "") return denied;

            Manage_Events event_data = new Manage_Events();
            Event? ev = event_data.get_event(id);
            if (ev == null)
            {
                Response.StatusCode = 404;
                return "Event does not exist...";
            }
            if (ev.organiser_id != check_access.user_id(HttpContext))
            {
                Response.StatusCode = 403;
                return "Access denied...";
            }

            string message = event_data.delete_event(id);

            if (message.Contains("successfully"))
            {
                Response.StatusCode = 204;
                return "";
            }
            Response.StatusCode = 400;
            return message;
        }//end of delete_event




    }//end of class
}//end of namespace
