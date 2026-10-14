namespace RaceDay.Models
{//start of namespace
    public class Enrolment
    {//start of class


        public int enrolment_id { get; set; }
        public int user_id { get; set; }
        public string participant_name { get; set; } = "";
        public int event_id { get; set; }
        public int organiser_id { get; set; }
        public string event_name { get; set; } = "";
        public DateTime event_date { get; set; }
        public int event_category_id { get; set; }
        public string category_name { get; set; } = "";
        public string status { get; set; } = "";
        public DateTime enrolment_date { get; set; }



    }//end of class
}//end of namespace
