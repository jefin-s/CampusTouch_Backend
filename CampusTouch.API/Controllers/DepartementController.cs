using CampusTouch.Application.Common;
using CampusTouch.Application.Features.Departments.Commands;
using CampusTouch.Application.Features.Departments.DTOs;
using CampusTouch.Application.Features.Departments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles ="Admin")]
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
        public async Task <ActionResult<IEnumerable<Deparetment_Response_DTO>>> GetAllDepartements()
        {
            var result = await _mediator.Send(new GetAllDepartementQuery());
            return Ok(new ApiResponse< IEnumerable< Deparetment_Response_DTO>>
            {
                Success = true,
                Message = "Departements fetched successfully",
                Data = result
            });
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Deparetment_Response_DTO>> GetDeptByID(int id)
        {
              var result=  await _mediator.Send(new GetDepartementByIdQuery(id));
             if(result==null)
                return NotFound();
            return Ok(new ApiResponse<Deparetment_Response_DTO>
            {
                Success = true,
                Message="Departement Fetched successfully",
                Data = result
            });

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteDepartmentCommand(id));

            if (result == 0)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message="Department is not found"
                });

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Department deleted successfully",
                Data = "Deleted"
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(int id, UpdateDepartmentCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "ID mismatch"
                });
            }

            var result = await _mediator.Send(command);

            if (result == 0)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Department not found"
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Department updated successfully",
                Data = "Updated"
            });
        }
    }
}
