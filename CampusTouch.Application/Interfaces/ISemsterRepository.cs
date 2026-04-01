using CampusTouch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Interfaces
{
    public interface ISemsterRepository
    {
        Task<int> CreateAsync(Semesters semester);
        Task<IEnumerable<Semesters>> GetAllAsync();
        Task<Semesters?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Semesters semester);
        Task<bool> DeleteAsync(int id,string userid);
        Task<bool> SemExist(int courseid,int orderNuber);
    }
}
