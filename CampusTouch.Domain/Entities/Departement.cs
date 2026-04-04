using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Domain.Entities
{
    public class Departement
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public  string CreatedBy { get; set; }
        public  string? UpdatedBy { get; set; }
        public  string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
     
        public bool isDeleted { get; set; }

        public string code { get; set; }
    }
}
