using MediatR;
using CampusTouch.Application.Interfaces;

using CampusTouch.Domain.Entities;

public class GetAllSubjectHandler : IRequestHandler<GetAllSubjectQuery, IEnumerable<Subject>>
{
    private readonly ISubjectRepository _repository;

    public GetAllSubjectHandler(ISubjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Subject>> Handle(GetAllSubjectQuery request, CancellationToken cancellationToken)
    {
        var data = await _repository.GetAllAsync();

        return data;
    }
}