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

        public async Task<ActionResult<int>> Create(CreateDepartementCommand command)
        {
            var result = await _mediator.Send(command);
        
            return CreatedAtAction( nameof(GetDeptByID),new { id=result},new ApiResponse<int>
            {
                Success = true,
                Data=result,
                Message="Department Added successfully"

            });
        }
            
        [HttpGet]
        public async Task <ActionResult<ApiResponse<IEnumerable<Deparetment_Response_DTO>>>> GetAllDepartements()
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
        public async Task<ActionResult<ApiResponse<Deparetment_Response_DTO>>> GetDeptByID(int id)
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
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteDepartmentCommand(id));
             return NoContent();
           
        }

            [HttpPut("{id}")]
        [Authorize(Roles ="Admin")]
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

            return NoContent();
            }
        }
}
