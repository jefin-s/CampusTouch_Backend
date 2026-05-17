using CampusTouch.Application.Interfaces;
using MediatR;
using System.Data;

public class UpdateAttendanceHandler : IRequestHandler<UpdateAttendanceCommand, bool>
{
    private readonly IAttendenceRepository _repo;
    private readonly IDbConnection _connection;

    public UpdateAttendanceHandler(
        IAttendenceRepository repo,
        IDbConnection connection)
    {
        _repo = repo;
        _connection = connection;
    }

    public async Task<bool> Handle(
        UpdateAttendanceCommand request,
        CancellationToken cancellationToken)
    {
        if (_connection.State == ConnectionState.Closed)
            _connection.Open();

        using var transaction = _connection.BeginTransaction();

        try
        {
            await _repo.UpdateAttendanceDetailsAsync(
                request.Students,
                request.AttendanceId,
                transaction);

            transaction.Commit();

            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}