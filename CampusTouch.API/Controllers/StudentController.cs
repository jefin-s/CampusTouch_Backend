using CampusTouch.API.Models.Students;
using CampusTouch.Application.Common;
using CampusTouch.Application.Features.Students.Commands;
using CampusTouch.Application.Features.Students.DTOs;
    using CampusTouch.Application.Features.Students.Queries.GetAllStudents;
using CampusTouch.Application.Features.Students.Queries.GetStudentsById;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Identity.Client;

namespace CampusTouch.API.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
    [Authorize]
        public class StudentController : ControllerBase
        {

            private readonly IMediator _mediator;
        private readonly ICloudinaryService _cloudinaryService;
            public StudentController(IMediator mediator,ICloudinaryService cloudinaryService)
            {
                    _mediator = mediator;
                    _cloudinaryService = cloudinaryService;
            }
            [HttpGet]

            public async Task<ActionResult<ApiResponse<IEnumerable<StudentResponseDTO>>>> GetAllStudents([FromQuery] int pageNumber=1, [FromQuery]  int pageSize = 10, [FromQuery] string?Search=null)
            {
                var result = await _mediator.Send(new GetAllStudentsQuery(pageNumber,pageSize,Search));
                
               return Ok( new ApiResponse <IEnumerable <StudentResponseDTO>>
               {
                     Success=true,
                   Data = result,
                   Message= "Students fetched successfully"
               });
            }
        [HttpGet("{id}")]
        public async Task <ActionResult<ApiResponse<StudentResponseDTO>>> GetStudentById(int id )
        {
            var result = await _mediator.Send(new GetStudentByIdQuery(id));
            

            return Ok(new ApiResponse<StudentResponseDTO>
            {
                Success=true,
                Message="Student detailed fetched succefully",
                Data=result
            });
        }
            [HttpPost]
            public async Task<IActionResult> CreateStudent([FromForm] CreateStudentRequest request)
            {
                try
                {
                    // 🔹 Handle Image Upload
                    string imageUrl = "";

                    if (request.ProfileImage != null && request.ProfileImage.Length > 0)
                    {
                        using var stream = request.ProfileImage.OpenReadStream();

                        imageUrl = await _cloudinaryService.UploadImageAsync(
                            stream,
                            request.ProfileImage.FileName
                        );
                    }

                    // 🔹 Create Command
                    var command = new CreateStudentCommand(
                    
                        request.AdmissionNumber,
                        request.CourseId,
                        request.DepartmentId,
                        request.AdmissionDate,
                        request.FirstName,
                        request.LastName,
                        request.DateOfBirth,
                        request.Gender,
                        request.PhoneNumber,
                        request.Email,
                        request.Address,
                        request.GuardianName,
                        request.GuardianPhone,
                        request.BloodGroup,
                        imageUrl // ✅ always safe (never null)
                    );

                    // 🔹 Send to Handler
                    var result = await _mediator.Send(command);

                    // 🔹 Response
                    return Ok(new ApiResponse<bool>
                    {
                        Success = result,
                        Message = result ? "Student created successfully" : "Failed to create student",
                        Data = result
                    });
                }
                catch (Exception ex)
                {
                    // 🔥 Shows actual error (no more hidden 500)
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = ex.Message
                    });
                }
            }

        [HttpPut]
        public async Task<IActionResult> UpdateStudent([FromForm] UpdateStudentRequest request)
        {
            try
            {
                string? imageUrl = null;

                // 🔥 Upload only if new file provided
                if (request.ProfileImage != null && request.ProfileImage.Length > 0)
                {
                    using var stream = request.ProfileImage.OpenReadStream();

                    imageUrl = await _cloudinaryService.UploadImageAsync(
                        stream,
                        request.ProfileImage.FileName
                    );
                }

                // 🔹 Convert to Command
                var command = new UpdateStudentCommand(
                    request.Id,
                    request.AdmissionNumber,
                    request.CourseId,
                    request.DepartmentId,
                    request.AdmissionDate,
                    request.FirstName,
                    request.LastName,
                    request.DateOfBirth,
                    request.Gender,
                    request.PhoneNumber,
                    request.Email,
                    request.Address,
                    request.GuardianName,
                    request.GuardianPhone,
                    request.BloodGroup,
                    imageUrl // may be null
                );

                var result = await _mediator.Send(command);

                return Ok(new ApiResponse<bool>
                {
                    Success = result,
                    Message = result ? "Student updated successfully" : "Failed to update student",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete]

        public async Task <ActionResult<string>> DeleteStudents(int id)
        {
            var  result= await _mediator.Send(new DeleteStudentCommand(id));
             return NoContent();
        }
    }
    }
