
using AutoMapper;
using DAL;
using DAL.Entiites;
using DAL.Helpers;
using Microsoft.Extensions.Logging;
using Modeles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Services.Repositories
{
    public interface IStudentRepository
    {
        StudentData GetStudentByUId(string uid);
        StudentData InserStudent(StudentData student);
        List<StudentData> GetAllStudents();
        public StudentData GetHighestStudentGPA();
    }


    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly ICasheServices _cashServices;
        private readonly ILogger _logger;

        public StudentRepository(AppDbContext appDbContext, IMapper mapper, ICasheServices cashServices, ILogger<StudentRepository> logger)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _cashServices = cashServices;
            _logger = logger;
        }
        public List<StudentData> GetAllStudents()
        {
            var students = _appDbContext.Students.ToList();
            return _mapper.Map<List<StudentData>>(students);
        }

        public StudentData GetStudentByUId(string uid)
        {
            var studentFromCache = _cashServices.GetCachValueAsync(uid.ToString());
            if (studentFromCache.Result != null)
            {
                Student? studentData = JsonSerializer.Deserialize<Student>(studentFromCache.Result);
                _logger.LogInformation("Student " + uid + " Retrived from REDIS");
                return _mapper.Map<StudentData>(studentData);
            }

            var student = _appDbContext.Students.Where(s => s.UniversityID == uid).FirstOrDefault();
            if (student.GPA > 85)
                _cashServices.SetCachValueAsync(student.UniversityID, JsonSerializer.Serialize(student));
            _logger.LogInformation("Student " + uid + " Retrived from DB");

            return _mapper.Map<StudentData>(student); ;
        }

        public StudentData InserStudent(StudentData student)
        {

            var studentData = _mapper.Map<Student>(student);
            _appDbContext.Students.Add(studentData);
            student.Id = studentData.Id;
            _appDbContext.SaveChanges();
            return student;
        }

        public StudentData GetHighestStudentGPA()
        {

            var studentFromCache = _cashServices.GetCachValueAsync("HighestGPA");
            if (studentFromCache.Result != null)
            {
                Student? studentData = JsonSerializer.Deserialize<Student>(studentFromCache.Result);
                _logger.LogInformation("Highest Student GPA  Retrived from REDIS");
                return _mapper.Map<StudentData>(studentData);
            }

            var student = _appDbContext.Students.OrderByDescending(x => x.GPA).First();

            //var maxValue = _appDbContext.Students.Max(s => s.GPA);
            //var student = _appDbContext.Students.First(x => x.GPA == maxValue);

            _cashServices.SetCachValueAsync("HighestGPA", JsonSerializer.Serialize(student));
            _logger.LogInformation("Highest Student GPA  Retrived from DB");

            return _mapper.Map<StudentData>(student); ;
        }
    }
}
