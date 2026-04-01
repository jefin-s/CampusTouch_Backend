using MediatR;

public record CreateSubjectCommand(string Name, string Code, int Credits, int SemesterId) : IRequest<int>;