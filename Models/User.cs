namespace RaceDay.Models
{
    public class User
    {


        public int user_id { get; set; }
        public string role_name { get; set; } = "";
        public string first_name { get; set; } = "";
        public string last_name { get; set; } = "";
        public string username { get; set; } = "";
        public string email { get; set; } = "";
        public string? phone_number { get; set; }

    }
}
