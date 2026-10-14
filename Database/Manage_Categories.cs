using RaceDay.Models;
using System.Data.SqlClient;

namespace RaceDay.Database
{//start of namspace
    public class Manage_Categories
    {//start of class



        //connection string
        private string connection_string = @"Data source=(localdb)\api_both;database=raceday;";

        //method to get all categories
        public List<Category> get_all()
        {//start of method
            List<Category> categories = new List<Category>();

            string query = "select category_id, category_name, description from categories order by category_name";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = run_query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Category category = new Category();
                                category.category_id = reader.GetInt32(0);
                                category.category_name = reader.GetString(1);
                                category.description = reader.IsDBNull(2) ? null : reader.GetString(2);
                                categories.Add(category);
                            }
                        }
                    }
                }
            }//end of try method
            catch (Exception)
            {
                categories.Clear();
            }
            return categories;
        }//end of method

        //method to get one category (null if not found)
        public Category? get_category(int category_id)
        {//start of method
            Category? category = null;

            string query = "select category_id, category_name, description from categories where category_id = @category_id";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@category_id", category_id);
                        using (SqlDataReader reader = run_query.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                category = new Category();
                                category.category_id = reader.GetInt32(0);
                                category.category_name = reader.GetString(1);
                                category.description = reader.IsDBNull(2) ? null : reader.GetString(2);
                            }
                        }
                    }
                }
            }//end of try method
            catch (Exception)
            {
                category = null;
            }
            return category;
        }//end of method

        //method to check how many events use this category
        public int in_use(int category_id)
        {//start of method
            int count = 0;

            string query = "select count(*) from event_categories where category_id = @category_id";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@category_id", category_id);
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

        //method to add a category
        public string add_category(string category_name, string? description)
        {//start of method
            string message = "Category creation failed...";

            string query = "insert into categories (category_name, description) values (@category_name, @description)";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@category_name", category_name);
                        run_query.Parameters.AddWithValue("@description", string.IsNullOrWhiteSpace(description)
                            ? (object)DBNull.Value : description);

                        int affected_row = run_query.ExecuteNonQuery();
                        if (affected_row > 0)
                        {
                            message = "Category created successfully...";
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

        //method to update a category
        public string update_category(int category_id, string category_name, string? description)
        {//start of method
            string message = "Category update failed...";

            string query = @"update categories set category_name = @category_name, description = @description
                where category_id = @category_id";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@category_id", category_id);
                        run_query.Parameters.AddWithValue("@category_name", category_name);
                        run_query.Parameters.AddWithValue("@description", string.IsNullOrWhiteSpace(description)
                            ? (object)DBNull.Value : description);

                        int affected_row = run_query.ExecuteNonQuery();
                        if (affected_row > 0)
                        {
                            message = "Category updated successfully...";
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

        //method to delete a category
        public string delete_category(int category_id)
        {//start of method
            string message = "Category deletion failed...";

            string query = "delete from categories where category_id = @category_id";

            try
            {//start of try method
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (SqlCommand run_query = new SqlCommand(query, connection))
                    {
                        run_query.Parameters.AddWithValue("@category_id", category_id);

                        int affected_row = run_query.ExecuteNonQuery();
                        if (affected_row > 0)
                        {
                            message = "Category deleted successfully...";
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
}//end of namspace
