using MediatR;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;

public class UpdateSubjectHandler : IRequestHandler<UpdateSubjectCommand, bool>
{
    private readonly ISubjectRepository _repository;

    public UpdateSubjectHandler(ISubjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = new Subject
        {
            Id = request.Id,
            Name = request.Name,
            Code = request.Code,
            Credits = request.Credits,
            SemesterId = request.SemesterId
        };

        return await _repository.UpdateAsync(subject);
    }
}