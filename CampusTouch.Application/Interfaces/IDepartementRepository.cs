using CampusTouch.Domain.Entities;


namespace CampusTouch.Application.Interfaces
{
    public interface IDepartementRepository
    {
        Task<int> CreateAsync(Departement department);
        Task<Departement> GetByIdAsync(int id);
        Task<IEnumerable<Departement>> GetAllAsync();
        Task<int> UpdateAsync(Departement department);
        Task<int> DeleteAsync(int id);
        Task<bool> DepartemnetExist(string name);
    }
}
