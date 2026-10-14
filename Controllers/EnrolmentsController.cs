using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Database;
using RaceDay.Models;
using System.Text.Json;

namespace RaceDay.Controllers
{//start of namespace
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {//start of class



        [HttpPost("api/events/{event_id:int}/enrolments")]
        public string enrol(int event_id, int event_category_id)
        {//start of enrol
            string denied = check_access.check(HttpContext, "Participant");
            if (denied != "") return denied;

            Manage_Events event_data = new Manage_Events();
            Manage_EventCategories ec_data = new Manage_EventCategories();
            Manage_Enrolments enrolment_data = new Manage_Enrolments();

            //the category must exist and belong to this event
            EventCategory? ec = ec_data.get_event_category(event_category_id);
            if (event_data.get_event(event_id) == null || ec == null || ec.event_id != event_id)
            {
                Response.StatusCode = 404;
                return "Event or category does not exist...";
            }

            int user_id = check_access.user_id(HttpContext);
            if (enrolment_data.already_enrolled(user_id, event_id) > 0)
            {
                Response.StatusCode = 409;
                return "You are already enrolled in this event...";
            }

            if (ec_data.has_enrolments(event_category_id) >= ec.max_participants)
            {
                Response.StatusCode = 409;
                return "This category is full...";
            }

            string message = enrolment_data.add_enrolment(user_id, event_id, event_category_id);

            if (message.Contains("successfully")) Response.StatusCode = 201;
            else if (message.Contains("full") || message.Contains("already")) Response.StatusCode = 409;
            else Response.StatusCode = 400;

            return message;
        }//end of enrol

        
        [HttpGet("api/enrolments/my")]
        public string get_my_enrolments()
        {//start of get_my_enrolments
            string denied = check_access.check(HttpContext, "Participant");
            if (denied != "") return denied;

            Manage_Enrolments enrolment_data = new Manage_Enrolments();
            return JsonSerializer.Serialize(enrolment_data.get_by_user(check_access.user_id(HttpContext)));
        }//end of get_my_enrolments

        
        [HttpGet("api/events/{event_id:int}/enrolments")]
        public string get_event_enrolments(int event_id)
        {//start of get_event_enrolments
            string denied = check_access.check(HttpContext, "Organiser");
            if (denied != "") return denied;

            Manage_Events event_data = new Manage_Events();
            Event? ev = event_data.get_event(event_id);
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

            Manage_Enrolments enrolment_data = new Manage_Enrolments();
            return JsonSerializer.Serialize(enrolment_data.get_by_event(event_id));
        }//end of get_event_enrolments

        
        [HttpGet("api/enrolments/{id:int}")]
        public string get_enrolment(int id)
        {//start of get_enrolment
            string denied = check_access.check(HttpContext, "Participant", "Organiser");
            if (denied != "") return denied;

            Manage_Enrolments enrolment_data = new Manage_Enrolments();
            Enrolment? enrolment = enrolment_data.get_enrolment(id);
            if (enrolment == null)
            {
                Response.StatusCode = 404;
                return "Enrolment does not exist...";
            }

            int user_id = check_access.user_id(HttpContext);
            bool allowed;
            if (check_access.role(HttpContext) == "Organiser")
            {
                allowed = enrolment.organiser_id == user_id;
            }
            else
            {
                allowed = enrolment.user_id == user_id;
            }

            if (!allowed)
            {
                Response.StatusCode = 403;
                return "Access denied...";
            }
            return JsonSerializer.Serialize(enrolment);
        }//end of get_enrolment

        
        [HttpDelete("api/enrolments/{id:int}")]
        public string cancel_enrolment(int id)
        {//start of cancel_enrolment
            string denied = check_access.check(HttpContext, "Participant");
            if (denied != "") return denied;

            Manage_Enrolments enrolment_data = new Manage_Enrolments();
            Enrolment? enrolment = enrolment_data.get_enrolment(id);
            if (enrolment == null)
            {
                Response.StatusCode = 404;
                return "Enrolment does not exist...";
            }
            if (enrolment.user_id != check_access.user_id(HttpContext))
            {
                Response.StatusCode = 403;
                return "Access denied...";
            }

            string message = enrolment_data.delete_enrolment(id);

            if (message.Contains("successfully"))
            {
                Response.StatusCode = 204;
                return "";
            }
            Response.StatusCode = 400;
            return message;
        }//end of cancel_enrolment


    }//end of class
}//end of namespace
