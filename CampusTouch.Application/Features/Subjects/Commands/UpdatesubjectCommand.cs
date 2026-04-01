using MediatR;

public record UpdateSubjectCommand(int Id, string Name, string Code, int Credits, int SemesterId) : IRequest<bool>;