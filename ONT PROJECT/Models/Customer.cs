using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ONT_PROJECT.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

    }
}
