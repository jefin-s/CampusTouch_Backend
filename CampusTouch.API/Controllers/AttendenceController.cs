using CampusTouch.Application.Features.Attendence.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CampusTouch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendenceController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AttendenceController(IMediator mediator )
        {
            _mediator = mediator;

        }
        [HttpPost("mark")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> MarkAttendance(CreateAttendanceCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]

        public async Task<IActionResult> GetAttendence([FromQuery]GetAttendenceByClassQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyAttendance()
        {
            var result = await _mediator.Send(new GetMyattendencequery());
            return Ok(result);
        }
    }
}
