using CampusTouch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Interfaces
{
    public interface ISubjectRepository
    {
        Task<int> CreateAsync(Subject subject);
        Task<IEnumerable<Subject>> GetAllAsync();
        Task<Subject?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Subject subject);
        Task<bool> DeleteAsync(int id);
    }
}
