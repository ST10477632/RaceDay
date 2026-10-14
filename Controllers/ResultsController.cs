using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Database;
using RaceDay.Models;
using System.Text.Json;

namespace RaceDay.Controllers
{//start of namespace
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {//start of class


        //allowed result statuses
        private static readonly string[] valid_statuses = { "Finished", "DNF", "DNS", "Disqualified" };

        //returns the status with correct capital letters, or null when it is wrong
        private static string? clean_status(string result_status)
        {
            return valid_statuses.FirstOrDefault(s => s.Equals(result_status.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        //turns hh:mm:ss text into a TimeSpan (empty text is allowed and means no time)
        private static bool read_time(string? text, out TimeSpan? time)
        {//start of method
            time = null;
            if (string.IsNullOrWhiteSpace(text)) return true;

            if (TimeSpan.TryParseExact(text.Trim(), @"hh\:mm\:ss", null, out TimeSpan parsed))
            {
                time = parsed;
                return true;
            }
            return false;
        }//end of method

        



        [HttpPost("api/enrolments/{enrolment_id:int}/results")]
        public string record_result(int enrolment_id, string result_status, string? finish_time = null, int? position = null)
        {//start of record_result
            string denied = check_access.check(HttpContext, "Organiser");
            if (denied != "") return denied;

            Manage_Enrolments enrolment_data = new Manage_Enrolments();
            Enrolment? enrolment = enrolment_data.get_enrolment(enrolment_id);
            if (enrolment == null)
            {
                Response.StatusCode = 404;
                return "Enrolment does not exist...";
            }
            if (enrolment.organiser_id != check_access.user_id(HttpContext))
            {
                Response.StatusCode = 403;
                return "Access denied...";
            }

            string? status = clean_status(result_status);
            TimeSpan? time;
            if (status == null || !read_time(finish_time, out time) || (status == "Finished" && time == null)
                || (position != null && position < 1))
            {
                Response.StatusCode = 400;
                return "Status must be Finished, DNF, DNS or Disqualified, finish time must be hh:mm:ss (required when Finished) and position must be 1 or more...";
            }

            Manage_Results result_data = new Manage_Results();
            if (result_data.result_exists(enrolment_id) > 0)
            {
                Response.StatusCode = 409;
                return "Result already exists...";
            }

            string message = result_data.add_result(enrolment_id, time, position, status);

            if (message.Contains("successfully")) Response.StatusCode = 201;
            else if (message.Contains("already exists")) Response.StatusCode = 409;
            else Response.StatusCode = 400;

            return message;
        }//end of record_result

        


        [HttpPut("api/results/{id:int}")]
        public string update_result(int id, string result_status, string? finish_time = null, int? position = null)
        {//start of update_result
            string denied = check_access.check(HttpContext, "Organiser");
            if (denied != "") return denied;

            Manage_Results result_data = new Manage_Results();
            Result? result = result_data.get_result(id);
            if (result == null)
            {
                Response.StatusCode = 404;
                return "Result does not exist...";
            }
            if (result.organiser_id != check_access.user_id(HttpContext))
            {
                Response.StatusCode = 403;
                return "Access denied...";
            }

            string? status = clean_status(result_status);
            TimeSpan? time;
            if (status == null || !read_time(finish_time, out time) || (status == "Finished" && time == null)
                || (position != null && position < 1))
            {
                Response.StatusCode = 400;
                return "Status must be Finished, DNF, DNS or Disqualified, finish time must be hh:mm:ss (required when Finished) and position must be 1 or more...";
            }

            string message = result_data.update_result(id, time, position, status);

            if (message.Contains("successfully")) Response.StatusCode = 200;
            else Response.StatusCode = 400;

            return message;
        }//end of update_result

        

        [HttpGet("api/results/my")]
        public string get_my_results()
        {//start of get_my_results
            string denied = check_access.check(HttpContext, "Participant");
            if (denied != "") return denied;

            Manage_Results result_data = new Manage_Results();
            return JsonSerializer.Serialize(result_data.get_by_user(check_access.user_id(HttpContext)));
        }//end of get_my_results

        

        [HttpGet("api/events/{event_id:int}/results")]
        public string get_event_results(int event_id)
        {//start of get_event_results
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

            Manage_Results result_data = new Manage_Results();
            return JsonSerializer.Serialize(result_data.get_by_event(event_id));
        }//end of get_event_results

        
        [HttpGet("api/results/{id:int}")]
        public string get_result(int id)
        {//start of get_result
            string denied = check_access.check(HttpContext, "Participant", "Organiser");
            if (denied != "") return denied;

            Manage_Results result_data = new Manage_Results();
            Result? result = result_data.get_result(id);
            if (result == null)
            {
                Response.StatusCode = 404;
                return "Result does not exist...";
            }

            int user_id = check_access.user_id(HttpContext);
            bool allowed;
            if (check_access.role(HttpContext) == "Organiser")
            {
                allowed = result.organiser_id == user_id;
            }
            else
            {
                allowed = result.user_id == user_id;
            }

            if (!allowed)
            {
                Response.StatusCode = 403;
                return "Access denied...";
            }
            return JsonSerializer.Serialize(result);
        }//end of get_result



    }//end of class
}//end of namespace
