using CampusTouch.Application.Common;
using CampusTouch.Application.Features.Departments.Commands;
using CampusTouch.Application.Features.Departments.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CampusTouch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartementController : ControllerBase
    {

        private readonly IMediator _mediator;
        public DepartementController(IMediator mediator)
        {
            _mediator = mediator;
            
        }

        [HttpPost]

        public async Task<ActionResult> Create(CreateDepartementCommand command)
        {
            var result = await _mediator.Send(command);
        
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Data=null,
                Message="Department Added successfully"

            });
        }
            
        [HttpGet]
        public async Task<ActionResult> GetAllDepartements()
        {
            var result = await _mediator.Send(new GetAllDepartementQuery());
            return Ok(result);
        }
    }
}
