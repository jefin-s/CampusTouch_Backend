using MediatR;
using CampusTouch.Domain.Entities;

public record GetAllSemesterQuery() : IRequest<IEnumerable<Semesters>>;