using DAL.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Modeles;
using Services.Repositories;
using System;
using System.Threading.Tasks;

namespace RedisCachingSampleProject.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger _logger;

        public StudentController(ICashServices cashServices, IStudentRepository studentRepository, ILogger<StudentController> logger)
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }

        [HttpGet("{uid}")]
        public async Task<IActionResult> getStudentByUId([FromRoute] string uid)
        {
            var student = _studentRepository.GetStudentByUId(uid);
            if (student != null)
                return Ok(student);
            else
                return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {

            var students = _studentRepository.GetAllStudents();
            if (students != null)
                return Ok(students);
            else
                return BadRequest();
        }

        [HttpGet("highestGrade")]
        public async Task<IActionResult> GetStudentWithHighestGrade()
        {

            var students = _studentRepository.GetHighestStudentGPA();
            if (students != null)
                return Ok(students);
            else
                return BadRequest();
        }


        [HttpPost]
        public async Task<IActionResult> AddStudent([FromBody] StudentData request)
        {

            var student = _studentRepository.InserStudent(request);

            return Ok(student);


        }
    }
}
//var value = await _cashServices.GetCachValueAsync(key);
//return string.IsNullOrEmpty(value) ? NotFound() : Ok(value);
//    await _cashServices.SetCachValueAsync(request.Key, request.Value);
//    return Ok();