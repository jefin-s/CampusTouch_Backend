using CampusTouch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Interfaces
{
    public  interface IStaffSubjectRepository
    {
        Task<int> AddAsync(StaffSubject entity);

        Task<bool> Exists(int staffId, int subjectId);

        Task<List<int>> GetSubjectsByStaffId(int staffId);

        Task<int> RemoveAsync(int staffId, int subjectId);

        Task<int> RemoveAllByStaffId(int staffId);

    }
}
