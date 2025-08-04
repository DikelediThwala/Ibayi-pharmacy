using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Models.Domain
{
    public class Doctor
    {
        
        public int DoctorID { get; set; }
        [Key]
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PracticeNo { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }

    }
}
