using CampusTouch.Application.Features.Students.DTOs;

using MediatR;


namespace CampusTouch.Application.Features.Students.Queries.GetAllStudents
{
    public  record GetAllStudentsQuery(int pageNumber,int pageSize,string? Search):IRequest<IEnumerable<StudentResponseDTO>>;
    
}
