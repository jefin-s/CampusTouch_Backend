using CampusTouch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Interfaces
{
    public interface IProgramRepository
    {
        Task<int> CreateAsync(AcademicProgram course);
        Task<IEnumerable<AcademicProgram>> GetAllAsync();
        Task<AcademicProgram?> GetByIdAsync(int id);
        Task<int> UpdateAsync(AcademicProgram course);
        Task<int> DeleteAsync(int id);
        Task<bool> ProgramIsExist(string course, int deptId);
    }
}
