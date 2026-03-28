

    using CampusTouch.Domain.Entities;

    namespace CampusTouch.Application.Interfaces
    {
        public interface IStudentRepository
        {

            Task <IEnumerable<Student>> GetAllStudents(int pageNumber,int pageSize,string Search);
        Task<Student> GetStudentsById(int id );
        Task<int> CreateStudentAsync(Student student);
    }
    }
