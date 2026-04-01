using MediatR;
using CampusTouch.Domain.Entities;

public record GetSemesterByIdQuery(int Id) : IRequest<Semesters?>;