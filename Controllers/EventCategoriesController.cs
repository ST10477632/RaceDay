using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Database;
using RaceDay.Models;
using System.Text.Json;

namespace RaceDay.Controllers
{//start of namespace
    [Route("api/[controller]")]
    [ApiController]
    public class EventCategoriesController : ControllerBase
    {//start of class


        
        [HttpGet("api/events/{event_id:int}/categories")]
        public string get_event_categories(int event_id)
        {//start of get_event_categories
            Manage_Events event_data = new Manage_Events();
            if (event_data.get_event(event_id) == null)
            {
                Response.StatusCode = 404;
                return "Event does not exist...";
            }

            Manage_EventCategories ec_data = new Manage_EventCategories();
            return JsonSerializer.Serialize(ec_data.get_for_event(event_id));
        }//end of get_event_categories

        
        [HttpPost("api/events/{event_id:int}/categories")]
        public string add_event_category(int event_id, int category_id, decimal entry_fee, int max_participants)
        {//start of add_event_category
            string denied = check_access.check(HttpContext, "Organiser");
            if (denied != "") return denied;

            Manage_Events event_data = new Manage_Events();
            Manage_Categories category_data = new Manage_Categories();
            Manage_EventCategories ec_data = new Manage_EventCategories();

            Event? ev = event_data.get_event(event_id);
            if (ev == null || category_data.get_category(category_id) == null)
            {
                Response.StatusCode = 404;
                return "Event or category does not exist...";
            }

            if (ev.organiser_id != check_access.user_id(HttpContext))
            {
                Response.StatusCode = 403;
                return "Access denied...";
            }

            if (entry_fee < 0 || max_participants < 1)
            {
                Response.StatusCode = 400;
                return "Entry fee cannot be negative and the maximum must be at least 1...";
            }

            if (ec_data.already_added(event_id, category_id) > 0)
            {
                Response.StatusCode = 409;
                return "Category already exists for the event...";
            }

            string message = ec_data.add_event_category(event_id, category_id, entry_fee, max_participants);

            if (message.Contains("successfully")) Response.StatusCode = 201;
            else if (message.Contains("already exists")) Response.StatusCode = 409;
            else Response.StatusCode = 400;

            return message;
        }//end of add_event_category

        
        [HttpPut("api/event-categories/{id:int}")]
        public string update_event_category(int id, decimal entry_fee, int max_participants)
        {//start of update_event_category
            string denied = check_access.check(HttpContext, "Organiser");
            if (denied != "") return denied;

            Manage_EventCategories ec_data = new Manage_EventCategories();
            EventCategory? ec = ec_data.get_event_category(id);
            if (ec == null)
            {
                Response.StatusCode = 404;
                return "Event category does not exist...";
            }

            Manage_Events event_data = new Manage_Events();
            Event? ev = event_data.get_event(ec.event_id);
            if (ev == null || ev.organiser_id != check_access.user_id(HttpContext))
            {
                Response.StatusCode = 403;
                return "Access denied...";
            }

            if (entry_fee < 0 || max_participants < 1)
            {
                Response.StatusCode = 400;
                return "Entry fee cannot be negative and the maximum must be at least 1...";
            }

            string message = ec_data.update_event_category(id, entry_fee, max_participants);

            if (message.Contains("successfully")) Response.StatusCode = 200;
            else Response.StatusCode = 400;

            return message;
        }//end of update_event_category

        
        [HttpDelete("api/event-categories/{id:int}")]
        public string remove_event_category(int id)
        {//start of remove_event_category
            string denied = check_access.check(HttpContext, "Organiser");
            if (denied != "") return denied;

            Manage_EventCategories ec_data = new Manage_EventCategories();
            EventCategory? ec = ec_data.get_event_category(id);
            if (ec == null)
            {
                Response.StatusCode = 404;
                return "Record does not exist...";
            }

            Manage_Events event_data = new Manage_Events();
            Event? ev = event_data.get_event(ec.event_id);
            if (ev == null || ev.organiser_id != check_access.user_id(HttpContext))
            {
                Response.StatusCode = 403;
                return "Access denied...";
            }

            if (ec_data.has_enrolments(id) > 0)
            {
                Response.StatusCode = 409;
                return "Category has existing enrolments...";
            }

            string message = ec_data.remove_event_category(id);

            if (message.Contains("successfully"))
            {
                Response.StatusCode = 204;
                return "";
            }
            Response.StatusCode = 400;
            return message;
        }//end of remove_event_category


    }//end of class
}//end of namespace
