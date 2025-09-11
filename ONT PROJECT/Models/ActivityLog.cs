namespace ONT_PROJECT.Models
{
    public class ActivityLog
    {
        public int ActivityLogId { get; set; }       
        public string ActivityType { get; set; }     
        public string Description { get; set; }      
        public DateTime DatePerformed { get; set; }  
    }
}
