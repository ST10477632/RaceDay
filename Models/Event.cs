namespace RaceDay.Models
{//start of namspace
    public class Event
    {//start of class



        public int event_id { get; set; }
        public string organiser_id { get; set; }
        public string event_name { get; set; } = "";
        public DateTime event_date { get; set; }
        public string location { get; set; } = "";
        public string description { get; set; } = "";
        public string? route_info { get; set; }
        public double distance { get; set; }
        public string event_type { get; set; } = "Run";
        public string? banner_image_url { get; set; }



    }//end of class
}//end of namspace
