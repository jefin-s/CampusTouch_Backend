using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Students.DTOs
{
    public class StudentResponseDTO
    {
        public int Id { get; set; }
        public string AdmissionNumber { get; set; }
        public string FullName { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
    }
}
