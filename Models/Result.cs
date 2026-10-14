namespace RaceDay.Models
{//start of namespace
    public class Result
    {//start of class


        public int result_id { get; set; }
        public int enrolment_id { get; set; }
        public int event_id { get; set; }
        public int user_id { get; set; }
        public int organiser_id { get; set; }
        public string event_name { get; set; } = "";
        public DateTime event_date { get; set; }
        public string category_name { get; set; } = "";
        public string participant_name { get; set; } = "";
        public TimeSpan? finish_time { get; set; }
        public int? position { get; set; }
        public string result_status { get; set; } = "";


    }//end of class
}//end of namespace
