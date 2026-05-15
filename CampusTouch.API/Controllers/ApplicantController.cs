using CampusTouch.Application.Features.Applicants.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace CampusTouch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicantController : ControllerBase
    {


        private readonly IMediator _mediator;
        public ApplicantController(IMediator mediator)
        {
            _mediator=mediator;
        }
        [HttpGet("applicants")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetApplicants()
        {
            var result =
                await _mediator.Send(
                    new GetApplicantQuery());

            return Ok(result);
        }
    }
}
