using MediatR;

using CampusTouch.Domain.Entities;

public record GetSubjectByIdQuery(int Id) : IRequest<Subject?>;