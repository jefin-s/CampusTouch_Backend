using MediatR;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;


public class GetSubjectByIdHandler : IRequestHandler<GetSubjectByIdQuery, Subject>
{
    private readonly ISubjectRepository _repository;

    public GetSubjectByIdHandler(ISubjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Subject?> Handle(GetSubjectByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);

       
    }
}