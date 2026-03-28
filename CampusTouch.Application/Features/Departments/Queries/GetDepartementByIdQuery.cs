using CampusTouch.Application.Features.Departments.DTOs;
using MediatR;


namespace CampusTouch.Application.Features.Departments.Queries
{
    public record GetDepartementByIdQuery(int Id):IRequest<Deparetment_Response_DTO?>;
    
}
