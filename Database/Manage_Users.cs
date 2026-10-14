using RaceDay.Models;
using System.Data.SqlClient;

namespace RaceDay.Database
{
    public class Manage_Users
    {

        //connection string
        private string connection_string = @"Data source=(localdb)\api_both;database=raceday;";

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

        //method to check the login details, sends back the user id and role when correct
        public string check_login(string email, string password, out int user_id, out string role_name)
        {//start of method
            user_id = 0;
            role_name = "";
            string message = "Incorrect email or password...";

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
                                    user_id = reader.GetInt32(0);
                                    role_name = reader.GetString(2);
                                    message = "Login successful...";
                                }
                            }
                        }
                    }
                }
            }//end of try method
            catch (Exception error)
            {
                message = error.Message;
            }
            return message;
        }//end of method

        //method to get one user (null if not found)
        public User? get_user(int user_id)
        {//start of method
            User? user = null;

            string query = @"select u.user_id, r.role_name, u.first_name, u.last_name, u.username, u.email, u.phone_number
                from users u join roles r on r.role_id = u.role_id
                where u.user_id = @user_id";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@user_id", user_id);
                        using (SqlDataReader reader = run_query.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new User();
                                user.user_id = reader.GetInt32(0);
                                user.role_name = reader.GetString(1);
                                user.first_name = reader.GetString(2);
                                user.last_name = reader.GetString(3);
                                user.username = reader.GetString(4);
                                user.email = reader.GetString(5);
                                user.phone_number = reader.IsDBNull(6) ? null : reader.GetString(6);
                            }
                        }
                    }
                }
            }//end of try method
            catch (Exception)
            {
                user = null;
            }
            return user;
        }//end of method

        //method to update the profile of the logged-in user
        public string update_profile(int user_id, string first_name, string last_name, string username,
            string email, string? phone_number)
        {//start of method
            string message = "Profile update failed...";

            string query = @"update users set first_name = @first_name, last_name = @last_name,
                username = @username, email = @email, phone_number = @phone_number
                where user_id = @user_id";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@user_id", user_id);
                        run_query.Parameters.AddWithValue("@first_name", first_name);
                        run_query.Parameters.AddWithValue("@last_name", last_name);
                        run_query.Parameters.AddWithValue("@username", username);
                        run_query.Parameters.AddWithValue("@email", email);
                        run_query.Parameters.AddWithValue("@phone_number", string.IsNullOrWhiteSpace(phone_number)
                            ? (object)DBNull.Value : phone_number);

                        int affected_row = run_query.ExecuteNonQuery();
                        if (affected_row > 0)
                        {
                            message = "Profile updated successfully...";
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

    }
}
