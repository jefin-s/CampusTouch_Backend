using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Domain.Entities
{
    public  class Staff
    {
        public int Id { get; set; }

        // 🔥 Identity Link
        public string UserId { get; set; }

        // 🧑 Basic Info
        public string FirstName { get; set; }
        public string? LastName { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // 🏫 Work Info
        public int DepartmentId { get; set; }
        public string Designation { get; set; }

        public string EmployeeCode { get; set; } // 🔥 Unique like AdmissionNumber

        public DateTime JoiningDate { get; set; }

        // ⚙️ System Fields
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
