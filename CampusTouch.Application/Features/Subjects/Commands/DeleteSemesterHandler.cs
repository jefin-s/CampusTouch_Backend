
using CampusTouch.Application.Interfaces;
using MediatR;

public class DeleteSubjectHandler : IRequestHandler<DeleteSubjectCommand, bool>
{
    private readonly ISubjectRepository _repository;

    public DeleteSubjectHandler(ISubjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(request.Id);
    }
}