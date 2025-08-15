using Microsoft.AspNetCore.Http;

namespace ONT_PROJECT.Models
{
    public class UploadPrescriptionViewModel
    {
        public IFormFile PrescriptionFile { get; set; } = null!;
        public bool RequestDispense { get; set; }
    }
}
