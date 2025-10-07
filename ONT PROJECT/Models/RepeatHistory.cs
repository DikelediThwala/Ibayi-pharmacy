namespace ONT_PROJECT.Models
{
    public class RepeatHistory
    {
        public int RepeatHistoryId { get; set; }
        public int PrescriptionLineId { get; set; }
        public PrescriptionLine PrescriptionLine { get; set; }

        public int RepeatsDecremented { get; set; } // Usually 1
        public DateTime DateUsed { get; set; }
    }
}
