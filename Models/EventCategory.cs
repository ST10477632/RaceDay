namespace RaceDay.Models
{//start of namespace
    public class EventCategory
    {//start of class



        public int event_category_id { get; set; }
        public int event_id { get; set; }
        public int category_id { get; set; }
        public string category_name { get; set; } = "";
        public decimal entry_fee { get; set; }
        public int max_participants { get; set; }



    }//end of class
}//end of namespace
