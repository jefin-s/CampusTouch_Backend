using MediatR;

public record DeleteSubjectCommand(int Id) : IRequest<bool>;