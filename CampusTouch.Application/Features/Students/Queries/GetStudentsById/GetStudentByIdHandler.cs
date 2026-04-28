


using CampusTouch.Application.Features.Students.DTOs;
using CampusTouch.Application.Interfaces;
using MediatR;

namespace CampusTouch.Application.Features.Students.Queries.GetStudentsById
{
    public  class GetStudentByIdHandler:IRequestHandler<GetStudentByIdQuery,StudentResponseDTO>
    {

        private readonly IStudentRepository _studentRepository;
        public GetStudentByIdHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }
        public  async Task<StudentResponseDTO> Handle(GetStudentByIdQuery idrequest,CancellationToken cancellationToken)
        {
            var students = await _studentRepository.GetStudentsById(idrequest.id);
            if (students == null) { return null; }
            return new StudentResponseDTO
            {
                Id = students.Id,
                AdmissionNumber = students.AdmissionNumber,
                Email = students.Email,
                FullName = students.FirstName + "" + students.LastName,
                IsActive = students.IsActive,
                ProfileImageUrl = students.ProfileImageUrl
                
            };
        }
    }
}
