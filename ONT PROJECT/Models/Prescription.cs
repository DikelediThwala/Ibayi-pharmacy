namespace ONT_PROJECT.Models
{
    public class Prescription
    {
        public int PrescriptionID { get; set; }
        public DateOnly Date {  get; set; }

        public int CustomerID { get; set; }

        public int PharmacistID { get; set; }

        public int DoctorID { get; set; }

        public byte[] PrescriptionPhoto { get; set; }
    }
}
