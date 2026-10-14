using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Database;
using RaceDay.Models;
using System.Text.Json;

namespace RaceDay.Controllers
{//start of namespace
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {//start of class

        private readonly IConfiguration _config;

        public UsersController(IConfiguration config)
        {
            _config = config;
        }

        
        [HttpPost("auth/register")]
        public string register(string first_name, string last_name, string username, string email,
            string password, string role_name = "Participant", string? phone_number = null,
            string? organiser_code = null)
        {//start of register
            if (role_name != "Participant" && role_name != "Organiser")
            {
                Response.StatusCode = 400;
                return "Role must be Participant or Organiser...";
            }

            if (!email.Contains('@') || password.Length < 8)
            {
                Response.StatusCode = 400;
                return "Invalid email or the password is shorter than 8 characters...";
            }

            //only people with the secret code may become organisers
            if (role_name == "Organiser" && organiser_code != _config["OrganiserRegistrationCode"])
            {
                Response.StatusCode = 403;
                return "A valid organiser code is required to register as an Organiser...";
            }

            Manage_Users user_data = new Manage_Users();
            string message = user_data.add_user(role_name, first_name.Trim(), last_name.Trim(),
                username.Trim(), email.Trim().ToLower(), phone_number, password);

            if (message.Contains("successfully")) Response.StatusCode = 201;
            else if (message.Contains("already exists")) Response.StatusCode = 409;
            else Response.StatusCode = 400;

            return message;
        }//end of register

       
        [HttpPost("auth/login")]
        public string login(string email, string password)
        {//start of login
            Manage_Users user_data = new Manage_Users();
            int user_id;
            string role_name;
            string message = user_data.check_login(email.Trim().ToLower(), password, out user_id, out role_name);

            if (user_id == 0)
            {
                Response.StatusCode = 401;
                return message;
            }

            //store the user in the session
            HttpContext.Session.Clear();
            HttpContext.Session.SetInt32("user_id", user_id);
            HttpContext.Session.SetString("role", role_name);
            return message;
        }//end of login

        
        [HttpPost("auth/logout")]
        public string logout()
        {//start of logout
            HttpContext.Session.Clear();
            return "Logged out successfully...";
        }//end of logout

        
        [HttpGet("users/profile")]
        public string get_profile()
        {//start of get_profile
            int? user_id = HttpContext.Session.GetInt32("user_id");
            if (user_id == null)
            {
                Response.StatusCode = 401;
                return "You must be logged in...";
            }

            Manage_Users user_data = new Manage_Users();
            User? user = user_data.get_user(user_id.Value);
            if (user == null)
            {
                Response.StatusCode = 401;
                return "User no longer exists...";
            }
            return JsonSerializer.Serialize(user);
        }//end of get_profile

        
        [HttpPut("users/profile")]
        public string update_profile(string first_name, string last_name, string username,
            string email, string? phone_number = null)
        {//start of update_profile
            int? user_id = HttpContext.Session.GetInt32("user_id");
            if (user_id == null)
            {
                Response.StatusCode = 401;
                return "You must be logged in...";
            }

            if (!email.Contains('@'))
            {
                Response.StatusCode = 400;
                return "Invalid email...";
            }

            Manage_Users user_data = new Manage_Users();
            string message = user_data.update_profile(user_id.Value, first_name.Trim(), last_name.Trim(),
                username.Trim(), email.Trim().ToLower(), phone_number);

            if (message.Contains("successfully")) Response.StatusCode = 200;
            else if (message.Contains("already exists")) Response.StatusCode = 409;
            else Response.StatusCode = 400;

            return message;
        }//end of update_profile

    }//end of class
}//end of namespace
