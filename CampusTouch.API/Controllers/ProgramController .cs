using CampusTouch.Application.Common;
using CampusTouch.Application.Features.Program.Commands;
using CampusTouch.Application.Features.Program.Queries;
using CampusTouch.Domain.Entities;
using CloudinaryDotNet;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CampusTouch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProgramController(IMediator mediator)
        {
            _mediator = mediator;

        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> Create(CreateCourseCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Course created successfully",
                Data = result
            });
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<AcademicProgram>>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllCoursesQuery());

            return Ok(new ApiResponse<IEnumerable<AcademicProgram>>
            {
                Success = true,
                Message = "Courses fetched successfully",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AcademicProgram>>> GetById(int id)
        {
            var result = await _mediator.Send(new GetCourseByIdQuery(id));

            if (result == null)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Course not found"
                });

            return Ok(new ApiResponse<AcademicProgram>
            {
                Success = true,
                Message = "Course fetched successfully",
                Data = result
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(int id, UpdateCourseCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            var result = await _mediator.Send(command);

            if (result == 0)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Course not found"
                });

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Course updated successfully",
                Data = "Updated"
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteCourseCommand(id));

            if (result == 0)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Course not found"
                });

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Course deleted successfully",
                Data = "Deleted"
            });
        }
    }
}
