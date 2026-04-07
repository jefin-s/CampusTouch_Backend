using CampusTouch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Interfaces
{
    public  interface IStaffRepository
    {

        Task<int> CreateNewStaff(Staff newStaff);
        Task <IEnumerable<Staff>> GetAllStaffs();
        Task<Staff?> GetStaffById(int id);

        Task<int> UpdateStaff(Staff staff);

        Task<int> DeactivateStaff(int id);
        Task<Staff?> GetByUserId(string userId);
    }
}
