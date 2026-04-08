using CampusTouch.Application.Common;
using CampusTouch.Application.Features.AssignSubject.Commands;
using CampusTouch.Application.Features.AssignSubject.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/staff")]
public class StaffSubjectController : ControllerBase
{
    private readonly IMediator _mediator;

    public StaffSubjectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // 🔥 1️⃣ Assign Subjects to Staff
    [HttpPost("{staffId}/subjects")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> AssignSubjects(
        int staffId,
        [FromBody] AssignSubjectCommand command)
    {
        command.StaffId = staffId;
        var result = await _mediator.Send(command);

        return Ok(new ApiResponse<bool>
        {
            Success = result,
            Message = "Subjects assigned successfully",
            Data = result
        });
    }

    // 🔥 2️⃣ Get Subjects by Staff
    [HttpGet("{staffId}/subjects")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<int>>>> GetSubjects(int staffId)
    {
        var result = await _mediator.Send(new GetSubjectByStaffQueries
        {
            StaffId = staffId
        });

        return Ok(new ApiResponse<List<int>>
        {
            Success = true,
            Message = "Subjects fetched successfully",
            Data = result
        });
    }

    // 🔥 3️⃣ Remove Subject from Staff
    [HttpDelete("{staffId}/subjects/{subjectId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveSubject(int staffId, int subjectId)
    {
        var result = await _mediator.Send(new RemoveSubjectFromStaffCommand
        {
            StaffId = staffId,
            SubjectId = subjectId
        });

        return Ok(new ApiResponse<bool>
        {
            Success = result,
            Message = result ? "Subject removed successfully" : "Failed to remove subject",
            Data = result
        });
    }
}