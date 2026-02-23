using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCC.Class
{
    public static class CurrentUser
    {
        public static int? PatientId { get; set; }
        public static int? StaffId { get; set; }
        public static string Role { get; set; } // "Patient", "Doctor", "Admin"
    }
}
