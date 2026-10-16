using RaceDay.Models;
using System.Data.SqlClient;

namespace RaceDay.Database
{//start of namespace
    public class Manage_Results
    {//start of class



        //connection string
        private string connection_string = Environment.GetEnvironmentVariable("RACEDAY_CONNECTION")
            ?? @"Data source=(localdb)\prog2;database=RaceDay;";

        //select used by the get methods
        private string select_query = @"select r.result_id, r.enrolment_id, e.event_id, e.user_id, ev.organiser_id,
            ev.event_name, ev.event_date, c.category_name,
            u.first_name + ' ' + u.last_name as participant_name,
            r.finish_time, r.position, r.result_status
            from results r
            join enrolments e on e.enrolment_id = r.enrolment_id
            join users u on u.user_id = e.user_id
            join events ev on ev.event_id = e.event_id
            join event_categories ec on ec.event_category_id = e.event_category_id
            join categories c on c.category_id = ec.category_id ";

        //turns the current row of the reader into a Result
        private Result read_row(SqlDataReader reader)
        {//start of method
            Result result = new Result();
            result.result_id = reader.GetInt32(0);
            result.enrolment_id = reader.GetInt32(1);
            result.event_id = reader.GetInt32(2);
            result.user_id = reader.GetInt32(3);
            result.organiser_id = reader.GetInt32(4);
            result.event_name = reader.GetString(5);
            result.event_date = reader.GetDateTime(6);
            result.category_name = reader.GetString(7);
            result.participant_name = reader.GetString(8);
            result.finish_time = reader.IsDBNull(9) ? null : reader.GetTimeSpan(9);
            result.position = reader.IsDBNull(10) ? null : reader.GetInt32(10);
            result.result_status = reader.GetString(11);
            return result;
        }//end of method

        //method to get one result (null if not found)
        public Result? get_result(int result_id)
        {//start of method
            Result? result = null;

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(select_query + "where r.result_id = @id", connection))
                    {
                        run_query.Parameters.AddWithValue("@id", result_id);
                        using (SqlDataReader reader = run_query.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = read_row(reader);
                            }
                        }
                    }
                }
            }//end of try method
            catch (Exception)
            {
                result = null;
            }
            return result;
        }//end of method

        //method to get all results of one participant
        public List<Result> get_by_user(int user_id)
        {//start of method
            List<Result> list = new List<Result>();

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(select_query + "where e.user_id = @user_id order by ev.event_date desc", connection))
                    {
                        run_query.Parameters.AddWithValue("@user_id", user_id);
                        using (SqlDataReader reader = run_query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(read_row(reader));
                            }
                        }
                    }
                }
            }//end of try method
            catch (Exception)
            {
                list.Clear();
            }
            return list;
        }//end of method

        //method to get all results of one event
        public List<Result> get_by_event(int event_id)
        {//start of method
            List<Result> list = new List<Result>();

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(select_query + "where e.event_id = @event_id order by r.position", connection))
                    {
                        run_query.Parameters.AddWithValue("@event_id", event_id);
                        using (SqlDataReader reader = run_query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(read_row(reader));
                            }
                        }
                    }
                }
            }//end of try method
            catch (Exception)
            {
                list.Clear();
            }
            return list;
        }//end of method

        //method to check if the enrolment already has a result
        public int result_exists(int enrolment_id)
        {//start of method
            int count = 0;

            string query = "select count(*) from results where enrolment_id = @enrolment_id";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@enrolment_id", enrolment_id);
                        count = Convert.ToInt32(run_query.ExecuteScalar());
                    }
                }
            }//end of try method
            catch (Exception)
            {
                count = 1;   //if the check fails, do not allow the insert
            }
            return count;
        }//end of method

        //method to record a result
        public string add_result(int enrolment_id, TimeSpan? finish_time, int? position, string result_status)
        {//start of method
            string message = "Recording result failed...";

            string query = @"insert into results (enrolment_id, finish_time, position, result_status)
                values (@enrolment_id, @finish_time, @position, @result_status)";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@enrolment_id", enrolment_id);
                        run_query.Parameters.AddWithValue("@finish_time", finish_time.HasValue
                            ? (object)finish_time.Value : DBNull.Value);
                        run_query.Parameters.AddWithValue("@position", position.HasValue
                            ? (object)position.Value : DBNull.Value);
                        run_query.Parameters.AddWithValue("@result_status", result_status);

                        int affected_row = run_query.ExecuteNonQuery();
                        if (affected_row > 0)
                        {
                            message = "Result recorded successfully...";
                        }
                    }
                }
            }//end of try method
            catch (SqlException error)
            {
                if (error.Number == 2627 || error.Number == 2601)
                {
                    message = "Result already exists...";
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

        //method to update a result
        public string update_result(int result_id, TimeSpan? finish_time, int? position, string result_status)
        {//start of method
            string message = "Updating result failed...";

            string query = @"update results set finish_time = @finish_time, position = @position,
                result_status = @result_status where result_id = @result_id";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@result_id", result_id);
                        run_query.Parameters.AddWithValue("@finish_time", finish_time.HasValue
                            ? (object)finish_time.Value : DBNull.Value);
                        run_query.Parameters.AddWithValue("@position", position.HasValue
                            ? (object)position.Value : DBNull.Value);
                        run_query.Parameters.AddWithValue("@result_status", result_status);

                        int affected_row = run_query.ExecuteNonQuery();
                        if (affected_row > 0)
                        {
                            message = "Result updated successfully...";
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



    }//end of class
}//end of namespace
