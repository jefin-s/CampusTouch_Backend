using CampusTouch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Interfaces
{
    public interface IClassesRepository
    {
        Task<int> CreateAsync(Classes model);
        Task<IEnumerable<Classes>> GetAllAsync();
        Task<Classes> GetByIdAsync(int id);
        Task UpdateAsync(int id, Classes model);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int courseId, int year, int semester);
    }
}
