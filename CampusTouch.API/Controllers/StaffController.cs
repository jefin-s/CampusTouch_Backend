using CampusTouch.Application.Common;
using CampusTouch.Application.Features.Staffs.Commands;
using CampusTouch.Application.Features.Staffs.DTOs;
using CampusTouch.Application.Features.Staffs.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace CampusTouch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {

        private readonly IMediator _mediator;
        public StaffController(IMediator mediator)
        {
            _mediator = mediator;
        }

        
        [HttpPost]
        [Authorize(Roles ="Admin")]

        public async Task< ActionResult<ApiResponse<bool>>> AddStaff([FromBody]CreateStaffCommand command)
        {
            var result =await _mediator.Send(command);
             return Ok(new ApiResponse<bool>
             {
                 Success = true,
               
                 Message = result ? "Staff added successfully" : "Failed to add staff",
                 Data = result
             });

        }
            [HttpPut("{id}")]
            [Authorize(Roles ="Admin")]

            public async Task<ActionResult<bool>> UpdateStaff(int id,[FromBody]UpdateStaffCommand command)
            {
                 command.Id = id;
            var result = await _mediator.Send(command);   
                  return Ok(new ApiResponse<bool>
                  {
                        Success = result,
                        Message = result ? "Staff updated successfully" : "Failed to update staff",
                        Data = result
                  });
            }

        [HttpGet("Getallstaff")]

        public async Task<ActionResult<ApiResponse<IEnumerable<StaffDto>>>> GetAllStaff()
        {
            var result = await _mediator.Send(new GetAllStaffQueries());
            return Ok(new ApiResponse<IEnumerable<StaffDto>>
            {
                Success = true,
                Message = "Staffs fetched successfully",
                Data = result
            });
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<StaffDto>>> GetById(int id)
        {
            var result = await _mediator.Send(new GetStaffByIdQueries  { Id = id });

            return Ok(new ApiResponse<StaffDto>
            {
                Success = true,
                Message = "Staff fetched successfully",
                Data = result
            });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteStaff(int id)
        {
            var result = await _mediator.Send(new DeleteStafffCommand { Id = id });

            return Ok(new ApiResponse<bool>
            {
                Success = result,
                Message = result ? "Staff deleted successfully" : "Failed to delete staff",
                Data = result
            });
        }
    }   
}
