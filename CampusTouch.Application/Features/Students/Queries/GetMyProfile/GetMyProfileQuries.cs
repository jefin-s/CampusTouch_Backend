using MediatR;
using CampusTouch.Application.Features.Students.DTOs;

namespace CampusTouch.Application.Features.Students.Queries.GetMyProfile
{
    public record GetMyProfileQuery() : IRequest<StudentMyProfileDTO>;
}