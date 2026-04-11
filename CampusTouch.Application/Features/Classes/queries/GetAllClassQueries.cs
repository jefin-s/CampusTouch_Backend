using CampusTouch.Domain.Entities;
using MediatR;

public class GetAllClassesQuery : IRequest<IEnumerable<Classes>>
{
}