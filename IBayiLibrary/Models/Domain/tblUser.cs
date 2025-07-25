using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBayiLibrary.Models.Domain
{
    public class tblUser
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string IDNumber { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Title { get; set; }
    }
}
