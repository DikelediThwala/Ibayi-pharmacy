using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Models.Domain
{
    public class PrescriptionViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName {  get; set; }
        [Required(ErrorMessage = "Doctor Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        public string FullName { get; set; }
        [Required(ErrorMessage = "Medicine is required")]
        public string MedicineName { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }
        public int PrescriptionLineID { get; set; }
        public int Schedule { get; set; }
        public int NoOfReadyOrders { get; set; }
        public int NumberOfUnprocessedPresc { get; set; }
        public int MedicineID { get; set; }
        [ForeignKey("PrescriptionID")]
        public int PrescriptionID { get; set; }
        public int UnprocessedPrescriptionID { get; set; }
        [Required(ErrorMessage = "Instructions are required")]
        public string Instructions { get; set; }
        [Required(ErrorMessage = "IDNumber is required")]
        public string IDNumber { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }
        public string Ingredients { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Repeats are required")]
        public int Repeats { get; set; }
        public int RepeatsLeft { get; set; }

        public DateTime ?Date { get; set; }      
        public int CustomerID { get; set; }   
        public int PharmacistID { get; set; }
        public byte[] PrescriptionPhoto { get; set; }
        public string Status { get; set; }
        [NotMapped]
        public IFormFile? PescriptionFile { get; set; }
        [ForeignKey("DoctorID")]
        public int DoctorID { get; set; }
        public List<PrescriptionViewModel> MedicationList { get; set; } = new();

    }
}
