using CampusTouch.Application.Features.Students.DTOs;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;


namespace CampusTouch.Application.Features.Students.Queries.GetAllStudents
{
    public class GetAllStudentsHandler:IRequestHandler<GetAllStudentsQuery,IEnumerable<StudentResponseDTO>>
    {

        private readonly IStudentRepository _StudentRepository;

        public GetAllStudentsHandler(IStudentRepository studentRepository)
        {
            _StudentRepository=studentRepository;

        }

        public async Task <IEnumerable<StudentResponseDTO>> Handle(GetAllStudentsQuery request,CancellationToken token)
        {
            var students= await _StudentRepository.GetAllStudents(request.pageNumber,request.pageSize,request.Search);

            var result = students.Select(s => new StudentResponseDTO
            {
               Id= s.Id,
               AdmissionNumber= s.AdmissionNumber,
               FullName=s.FirstName+""+s.LastName,
               Email=s.Email,
               IsActive=s.IsActive,
               ProfileImageUrl=s.ProfileImageUrl
            });
            return result;
        }
    }
}
