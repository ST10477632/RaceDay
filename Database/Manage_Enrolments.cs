using RaceDay.Models;
using System.Data.SqlClient;

namespace RaceDay.Database
{//start of namespace
    public class Manage_Enrolments
    {//start of class



        //connection string
        private string connection_string = Environment.GetEnvironmentVariable("RACEDAY_CONNECTION")
            ?? @"Data source=(localdb)\prog2;database=RaceDay;";

        //select used by the get methods
        private string select_query = @"select e.enrolment_id, e.user_id,
            u.first_name + ' ' + u.last_name as participant_name,
            e.event_id, ev.organiser_id, ev.event_name, ev.event_date,
            e.event_category_id, c.category_name, e.status, e.enrolment_date
            from enrolments e
            join users u on u.user_id = e.user_id
            join events ev on ev.event_id = e.event_id
            join event_categories ec on ec.event_category_id = e.event_category_id
            join categories c on c.category_id = ec.category_id ";

        //turns the current row of the reader into an Enrolment
        private Enrolment read_row(SqlDataReader reader)
        {//start of method
            Enrolment enrolment = new Enrolment();
            enrolment.enrolment_id = reader.GetInt32(0);
            enrolment.user_id = reader.GetInt32(1);
            enrolment.participant_name = reader.GetString(2);
            enrolment.event_id = reader.GetInt32(3);
            enrolment.organiser_id = reader.GetInt32(4);
            enrolment.event_name = reader.GetString(5);
            enrolment.event_date = reader.GetDateTime(6);
            enrolment.event_category_id = reader.GetInt32(7);
            enrolment.category_name = reader.GetString(8);
            enrolment.status = reader.GetString(9);
            enrolment.enrolment_date = reader.GetDateTime(10);
            return enrolment;
        }//end of method

        //method to get one enrolment (null if not found)
        public Enrolment? get_enrolment(int enrolment_id)
        {//start of method
            Enrolment? enrolment = null;

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(select_query + "where e.enrolment_id = @id", connection))
                    {
                        run_query.Parameters.AddWithValue("@id", enrolment_id);
                        using (SqlDataReader reader = run_query.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                enrolment = read_row(reader);
                            }
                        }
                    }
                }
            }//end of try method
            catch (Exception)
            {
                enrolment = null;
            }
            return enrolment;
        }//end of method

        //method to get all enrolments of one participant
        public List<Enrolment> get_by_user(int user_id)
        {//start of method
            List<Enrolment> list = new List<Enrolment>();

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(select_query + "where e.user_id = @user_id order by ev.event_date", connection))
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

        //method to get all enrolments of one event
        public List<Enrolment> get_by_event(int event_id)
        {//start of method
            List<Enrolment> list = new List<Enrolment>();

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(select_query + "where e.event_id = @event_id order by e.enrolment_date", connection))
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

        //method to check if the participant is already enrolled in the event
        public int already_enrolled(int user_id, int event_id)
        {//start of method
            int count = 0;

            string query = "select count(*) from enrolments where user_id = @user_id and event_id = @event_id";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@user_id", user_id);
                        run_query.Parameters.AddWithValue("@event_id", event_id);
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

        //method to enrol a participant (the insert only happens while the category still has space)
        public string add_enrolment(int user_id, int event_id, int event_category_id)
        {//start of method
            string message = "Enrolment failed...";

            string query = @"insert into enrolments (user_id, event_id, event_category_id)
                select @user_id, @event_id, @event_category_id
                from event_categories
                where event_category_id = @event_category_id
                and (select count(*) from enrolments where event_category_id = @event_category_id) < max_participants";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@user_id", user_id);
                        run_query.Parameters.AddWithValue("@event_id", event_id);
                        run_query.Parameters.AddWithValue("@event_category_id", event_category_id);

                        int affected_row = run_query.ExecuteNonQuery();
                        if (affected_row > 0)
                        {
                            message = "Enrolment created successfully...";
                        }
                        else
                        {
                            message = "This category is full...";
                        }
                    }
                }
            }//end of try method
            catch (SqlException error)
            {
                if (error.Number == 2627 || error.Number == 2601)
                {
                    message = "You are already enrolled in this event...";
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

        //method to cancel an enrolment (its result is deleted with it, in one transaction)
        public string delete_enrolment(int enrolment_id)
        {//start of method
            string message = "Cancelling enrolment failed...";

            string query = @"set xact_abort on;
                begin transaction;
                delete from results where enrolment_id = @id;
                delete from enrolments where enrolment_id = @id;
                commit;";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@id", enrolment_id);
                        run_query.ExecuteNonQuery();
                        message = "Enrolment cancelled successfully...";
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
