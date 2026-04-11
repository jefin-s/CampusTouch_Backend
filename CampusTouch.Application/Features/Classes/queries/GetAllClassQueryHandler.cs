using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;

public class GetAllClassesHandler : IRequestHandler<GetAllClassesQuery, IEnumerable<Classes>>
{
    private readonly IClassesRepository _repo;

    public GetAllClassesHandler(IClassesRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Classes>> Handle(GetAllClassesQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetAllAsync();
    }
}