using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Database;
using RaceDay.Models;
using Microsoft.Extensions.Configuration;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticatonController : ControllerBase
    {

        private readonly IConfiguration _config;

        public AuthenticatonController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("register")]
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

            Manage_Authentication auth_data = new Manage_Authentication();
            string message = auth_data.add_user(role_name, first_name.Trim(), last_name.Trim(),
                username.Trim(), email.Trim().ToLower(), phone_number, password);

            if (message.Contains("successfully")) Response.StatusCode = 201;
            else if (message.Contains("already exists")) Response.StatusCode = 409;
            else Response.StatusCode = 400;

            return message;
        }//end of register

        /// <summary>Logs in and starts a session that stores the user id and role.</summary>
        /// <response code="200">Login successful.</response>
        /// <response code="401">Incorrect email or password.</response>
        [HttpPost("login")]
        public string login(string email, string password)
        {//start of login
            Manage_Authentication auth_data = new Manage_Authentication();
            string message;
            Authentication? user = auth_data.check_login(email.Trim().ToLower(), password, out message);

            if (user == null)
            {
                Response.StatusCode = 401;
                return message;
            }

            //store the user id and role in the session
            HttpContext.Session.Clear();
            HttpContext.Session.SetInt32("user_id", user.user_id);
            HttpContext.Session.SetString("role", user.role_name);
            return message;
        }//end of login

        /// <summary>Ends the current session.</summary>
        /// <response code="200">Logged out.</response>
        [HttpPost("logout")]
        public string logout()
        {//start of logout
            HttpContext.Session.Clear();
            return "Logged out successfully...";
        }//end of logout

    }
}
