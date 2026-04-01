using MediatR;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;

public class CreateSubjectHandler : IRequestHandler<CreateSubjectCommand, int>
{
    private readonly ISubjectRepository _repository;

    public CreateSubjectHandler(ISubjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = new Subject
        {
            Name = request.Name,
            Code = request.Code,
            Credits = request.Credits,
            SemesterId = request.SemesterId
        };

        return await _repository.CreateAsync(subject);
    }
}