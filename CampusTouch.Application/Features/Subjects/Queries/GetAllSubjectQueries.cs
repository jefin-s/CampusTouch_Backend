using MediatR;

using CampusTouch.Domain.Entities;

public record GetAllSubjectQuery() : IRequest<IEnumerable<Subject>>;