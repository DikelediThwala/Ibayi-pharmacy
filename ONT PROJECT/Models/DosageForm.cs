using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models
{
    public class DosageForm
    {
        [Key]
        public int FormID { get; set; }

        [Required(ErrorMessage = "Form name is required.")]
        [Display(Name = "Form Name")]
        public string FormName { get; set; }
    }
}
