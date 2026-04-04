using CampusTouch.Application.Common;
using CampusTouch.Application.Features.Program.Commands;
using CampusTouch.Application.Features.Program.Queries;
using CampusTouch.Domain.Entities;
using CloudinaryDotNet;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<ApiResponse<int>>> CreateProgram(CreateCourseCommand command)
        {
            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetById),new { id=result},
            new ApiResponse<int>
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
        [Authorize(Roles ="Admin")]

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateCourseCommand command)
        {
            var updatedcommand = command with { Id = id };

             await _mediator.Send(updatedcommand);



            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> Delete(int id)
        {
             await _mediator.Send(new DeleteCourseCommand(id));

            
             return  NoContent();
        }
    }
}
