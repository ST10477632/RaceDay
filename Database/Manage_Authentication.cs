using RaceDay.Models;
using System.Data.SqlClient;
using RaceDay.Controllers;

namespace RaceDay.Database
{
    public class Manage_Authentication
    {


        //connection string
        private string connection_string = @"Data source=(localdb)\prog2;database=RaceDay;";

        //method to register a new user
        public string add_user(string role_name, string first_name, string last_name, string username,
            string email, string? phone_number, string password)
        {//start of method
            string message = "User registration failed...";

            //hash the user's password
            string hashed_password = BCrypt.Net.BCrypt.HashPassword(password);

            string query = @"INSERT INTO users (role_id, first_name, last_name, username, email, phone_number, password_hash)
                SELECT role_id, @first_name, @last_name, @username, @email, @phone_number, @password_hash FROM roles
                where role_name = @role_name;";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@role_name", role_name);
                        run_query.Parameters.AddWithValue("@first_name", first_name);
                        run_query.Parameters.AddWithValue("@last_name", last_name);
                        run_query.Parameters.AddWithValue("@username", username);
                        run_query.Parameters.AddWithValue("@email", email);
                        run_query.Parameters.AddWithValue("@phone_number", string.IsNullOrWhiteSpace(phone_number)
                            ? (object)DBNull.Value : phone_number);
                        run_query.Parameters.AddWithValue("@password_hash", hashed_password);

                        int affected_row = run_query.ExecuteNonQuery();
                        if (affected_row > 0)
                        {
                            message = "User registered successfully...";
                        }
                        else
                        {
                            message = "Registration failed: role not found...";
                        }
                    }
                }
            }//end of try method
            catch (SqlException error)
            {
                if (error.Number == 2627 || error.Number == 2601)
                {
                    message = "Username or email already exists...";
                }
                else
                {
                    message = error.Message;
                }
            }
            catch (Exception error)
            {
                message = error.Message;
            }
            return message;
        }//end of method

        //method to check the login details, returns the user or null when the details are wrong
        public Authentication? check_login(string email, string password, out string message)
        {//start of method
            Authentication? user = null;
            message = "Incorrect email or password...";

            string query = @"select u.user_id, u.password_hash, r.role_name
                from users u join roles r on r.role_id = u.role_id
                where u.email = @email";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@email", email);
                        using (SqlDataReader reader = run_query.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string stored_hash = reader.GetString(1);

                                //compare the password with the stored hash
                                if (BCrypt.Net.BCrypt.Verify(password, stored_hash))
                                {
                                    user = new Authentication();
                                    user.user_id = reader.GetInt32(0);
                                    user.role_name = reader.GetString(2);
                                    user.email = email;
                                    message = "Login successful...";
                                }
                            }
                        }
                    }
                }
            }//end of try method
            catch (Exception error)
            {
                user = null;
                message = error.Message;
            }
            return user;
        }//end of method

    }//end of class

    //session checks used by every protected endpoint in the other controllers
    public class check_access
    {//start of class

        //returns "" if the user may continue, otherwise sets 401 or 403 and returns the message
        public static string check(HttpContext context, params string[] roles)
        {//start of method
            int? user_id = context.Session.GetInt32("user_id");
            if (user_id == null)
            {
                context.Response.StatusCode = 401;
                return "You must be logged in...";
            }

            string role_name = context.Session.GetString("role") ?? "";
            if (roles.Length > 0 && !roles.Contains(role_name))
            {
                context.Response.StatusCode = 403;
                return "Access denied for your role...";
            }
            return "";
        }//end of method

        //id of the logged-in user (0 if nobody is logged in)
        public static int user_id(HttpContext context)
        {
            return context.Session.GetInt32("user_id") ?? 0;
        }

        //role of the logged-in user
        public static string role(HttpContext context)
        {
            return context.Session.GetString("role") ?? "";
        }

    }
}
