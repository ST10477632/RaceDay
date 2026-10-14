using RaceDay.Models;
using System.Data.SqlClient;

namespace RaceDay.Database
{
    public class Manage_Events
    {



        //connection string
        private string connection_string = @"Data source=(localdb)\prog2;database=RaceDay;";

        //columns used by every select
        private string columns = @"event_id, organiser_id, event_name, event_date, location,
            description, route_info, distance, event_type, banner_image_url";

        //turns the current row of the reader into an Event
        private Event read_event(SqlDataReader reader)
        {//start of method
            Event ev = new Event();
            ev.event_id = reader.GetInt32(0);
            ev.organiser_id = reader.GetInt32(1);
            ev.event_name = reader.GetString(2);
            ev.event_date = reader.GetDateTime(3);
            ev.location = reader.GetString(4);
            ev.description = reader.GetString(5);
            ev.route_info = reader.IsDBNull(6) ? null : reader.GetString(6);
            ev.distance = reader.GetDouble(7);
            ev.event_type = reader.GetString(8);
            ev.banner_image_url = reader.IsDBNull(9) ? null : reader.GetString(9);
            return ev;
        }//end of method

        //method to get all upcoming events
        public List<Event> get_upcoming()
        {//start of method
            List<Event> events = new List<Event>();

            string query = "select " + columns + " from events where event_date >= @today order by event_date";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@today", DateTime.Today);
                        using (SqlDataReader reader = run_query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                events.Add(read_event(reader));
                            }
                        }
                    }
                }
            }//end of try method
            catch (Exception)
            {
                events.Clear();
            }
            return events;
        }//end of method

        //method to get the events of one organiser
        public List<Event> get_by_organiser(int organiser_id)
        {//start of method
            List<Event> events = new List<Event>();

            string query = "select " + columns + " from events where organiser_id = @organiser_id order by event_date";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@organiser_id", organiser_id);
                        using (SqlDataReader reader = run_query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                events.Add(read_event(reader));
                            }
                        }
                    }
                }
            }//end of try method
            catch (Exception)
            {
                events.Clear();
            }
            return events;
        }//end of method

        //method to get one event (null if not found)
        public Event? get_event(int event_id)
        {//start of method
            Event? ev = null;

            string query = "select " + columns + " from events where event_id = @event_id";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@event_id", event_id);
                        using (SqlDataReader reader = run_query.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ev = read_event(reader);
                            }
                        }
                    }
                }
            }//end of try method
            catch (Exception)
            {
                ev = null;
            }
            return ev;
        }//end of method

        //method to add a new event
        public string add_event(int organiser_id, string event_name, DateTime event_date, string location,
            string description, string? route_info, double distance, string event_type)
        {//start of method
            string message = "Event creation failed...";

            string query = @"insert into events (organiser_id, event_name, event_date, location, description,
                route_info, distance, event_type)
                values (@organiser_id, @event_name, @event_date, @location, @description,
                @route_info, @distance, @event_type)";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@organiser_id", organiser_id);
                        run_query.Parameters.AddWithValue("@event_name", event_name);
                        run_query.Parameters.AddWithValue("@event_date", event_date);
                        run_query.Parameters.AddWithValue("@location", location);
                        run_query.Parameters.AddWithValue("@description", description);
                        run_query.Parameters.AddWithValue("@route_info", string.IsNullOrWhiteSpace(route_info)
                            ? (object)DBNull.Value : route_info);
                        run_query.Parameters.AddWithValue("@distance", distance);
                        run_query.Parameters.AddWithValue("@event_type", event_type);

                        int affected_row = run_query.ExecuteNonQuery();
                        if (affected_row > 0)
                        {
                            message = "Event created successfully...";
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

        //method to update an event
        public string update_event(int event_id, string event_name, DateTime event_date, string location,
            string description, string? route_info, double distance, string event_type)
        {//start of method
            string message = "Event update failed...";

            string query = @"update events set event_name = @event_name, event_date = @event_date,
                location = @location, description = @description, route_info = @route_info,
                distance = @distance, event_type = @event_type
                where event_id = @event_id";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@event_id", event_id);
                        run_query.Parameters.AddWithValue("@event_name", event_name);
                        run_query.Parameters.AddWithValue("@event_date", event_date);
                        run_query.Parameters.AddWithValue("@location", location);
                        run_query.Parameters.AddWithValue("@description", description);
                        run_query.Parameters.AddWithValue("@route_info", string.IsNullOrWhiteSpace(route_info)
                            ? (object)DBNull.Value : route_info);
                        run_query.Parameters.AddWithValue("@distance", distance);
                        run_query.Parameters.AddWithValue("@event_type", event_type);

                        int affected_row = run_query.ExecuteNonQuery();
                        if (affected_row > 0)
                        {
                            message = "Event updated successfully...";
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

        //method to delete an event together with everything linked to it (one transaction)
        public string delete_event(int event_id)
        {//start of method
            string message = "Event deletion failed...";

            string query = @"set xact_abort on;
                begin transaction;
                delete from results where enrolment_id in (select enrolment_id from enrolments where event_id = @event_id);
                delete from enrolments where event_id = @event_id;
                delete from event_categories where event_id = @event_id;
                delete from events where event_id = @event_id;
                commit;";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@event_id", event_id);
                        run_query.ExecuteNonQuery();
                        message = "Event deleted successfully...";
                    }
                }
            }//end of try method
            catch (Exception error)
            {
                message = error.Message;
            }
            return message;
        }//end of method

    }
}
