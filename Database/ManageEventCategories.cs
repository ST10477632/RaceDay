using RaceDay.Models;

namespace RaceDay.Database
{//start of namespace
    public class ManageEventCategories
    {//start of class



        /connection string
        private string connection_string = @"Data source=(localdb)\api_both;database=raceday;";

        //select used by the get methods
        private string select_query = @"select ec.event_category_id, ec.event_id, ec.category_id, c.category_name,
            ec.entry_fee, ec.max_participants
            from event_categories ec
            join categories c on c.category_id = ec.category_id ";

        //turns the current row of the reader into an EventCategory
        private EventCategory read_row(SqlDataReader reader)
        {//start of method
            EventCategory ec = new EventCategory();
            ec.event_category_id = reader.GetInt32(0);
            ec.event_id = reader.GetInt32(1);
            ec.category_id = reader.GetInt32(2);
            ec.category_name = reader.GetString(3);
            ec.entry_fee = reader.GetDecimal(4);
            ec.max_participants = reader.GetInt32(5);
            return ec;
        }//end of method

        //method to get all categories of one event
        public List<EventCategory> get_for_event(int event_id)
        {//start of method
            List<EventCategory> list = new List<EventCategory>();

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(select_query + "where ec.event_id = @event_id order by ec.event_category_id", connection))
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

        //method to get one event category (null if not found)
        public EventCategory? get_event_category(int event_category_id)
        {//start of method
            EventCategory? ec = null;

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(select_query + "where ec.event_category_id = @id", connection))
                    {
                        run_query.Parameters.AddWithValue("@id", event_category_id);
                        using (SqlDataReader reader = run_query.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ec = read_row(reader);
                            }
                        }
                    }
                }
            }//end of try method
            catch (Exception)
            {
                ec = null;
            }
            return ec;
        }//end of method

        //method to check if the category is already added to the event
        public int already_added(int event_id, int category_id)
        {//start of method
            int count = 0;

            string query = "select count(*) from event_categories where event_id = @event_id and category_id = @category_id";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@event_id", event_id);
                        run_query.Parameters.AddWithValue("@category_id", category_id);
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

        //method to count the enrolments of an event category
        public int has_enrolments(int event_category_id)
        {//start of method
            int count = 0;

            string query = "select count(*) from enrolments where event_category_id = @id";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@id", event_category_id);
                        count = Convert.ToInt32(run_query.ExecuteScalar());
                    }
                }
            }//end of try method
            catch (Exception)
            {
                count = 1;   //if the check fails, do not allow the delete
            }
            return count;
        }//end of method

        //method to add a category to an event
        public string add_event_category(int event_id, int category_id, decimal entry_fee, int max_participants)
        {//start of method
            string message = "Adding category to event failed...";

            string query = @"insert into event_categories (event_id, category_id, entry_fee, max_participants)
                values (@event_id, @category_id, @entry_fee, @max_participants)";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@event_id", event_id);
                        run_query.Parameters.AddWithValue("@category_id", category_id);
                        run_query.Parameters.AddWithValue("@entry_fee", entry_fee);
                        run_query.Parameters.AddWithValue("@max_participants", max_participants);

                        int affected_row = run_query.ExecuteNonQuery();
                        if (affected_row > 0)
                        {
                            message = "Category added to event successfully...";
                        }
                    }
                }
            }//end of try method
            catch (SqlException error)
            {
                if (error.Number == 2627 || error.Number == 2601)
                {
                    message = "Category already exists for the event...";
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

        //method to update the entry fee and maximum participants
        public string update_event_category(int event_category_id, decimal entry_fee, int max_participants)
        {//start of method
            string message = "Event category update failed...";

            string query = @"update event_categories set entry_fee = @entry_fee, max_participants = @max_participants
                where event_category_id = @id";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@id", event_category_id);
                        run_query.Parameters.AddWithValue("@entry_fee", entry_fee);
                        run_query.Parameters.AddWithValue("@max_participants", max_participants);

                        int affected_row = run_query.ExecuteNonQuery();
                        if (affected_row > 0)
                        {
                            message = "Event category updated successfully...";
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

        //method to remove a category from an event
        public string remove_event_category(int event_category_id)
        {//start of method
            string message = "Removing category failed...";

            string query = "delete from event_categories where event_category_id = @id";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@id", event_category_id);

                        int affected_row = run_query.ExecuteNonQuery();
                        if (affected_row > 0)
                        {
                            message = "Category removed successfully...";
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
