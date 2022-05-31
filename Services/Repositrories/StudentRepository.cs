
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
        Student GetStudentByUId(string uid);
        Student InserStudent(Student student);
        List<Student> GetAllStudents();
    }


    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly ICashServices _cashServices;
        private readonly ILogger _logger;

        public StudentRepository(AppDbContext appDbContext, IMapper mapper, ICashServices cashServices, ILogger<StudentRepository> logger)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _cashServices = cashServices;
            _logger = logger;
        }
        public List<Student> GetAllStudents()
        {
            var students = _appDbContext.Students.ToList();
            return _mapper.Map<List<Student>>(students);
        }

        public Student GetStudentByUId(string uid)
        {
            var studentFromCache = _cashServices.GetCachValueAsync(uid.ToString());
            if (studentFromCache.Result != null)
            {
                StudentData? studentData = JsonSerializer.Deserialize<StudentData>(studentFromCache.Result);
                _logger.LogInformation("Student " + uid + " Retrived from REDIS");
                return _mapper.Map<Student>(studentData);
            }

            var student = _appDbContext.Students.Where(s => s.UniversityID == uid).FirstOrDefault();
            if (student.GPA > 85)
                _cashServices.SetCachValueAsync(student.UniversityID, JsonSerializer.Serialize(student));
            _logger.LogInformation("Student " + uid + " Retrived from DB");

            return _mapper.Map<Student>(student); ;
        }

        public Student InserStudent(Student student)
        {
            //int _UniversityID = 1170000;

            //for (int i = 0; i < 100000; i++)
            //{
            //    _appDbContext.Students.Add(new StudentData() { UniversityID = _UniversityID++.ToString(), GPA = new Random().Next(65, 98) + new Random().NextDouble(), Name = "TestStudent" }); ;
            //}
            var studentData = _mapper.Map<StudentData>(student);
            _appDbContext.Students.Add(studentData);
            student.Id = studentData.Id;
            _appDbContext.SaveChanges();
            return student;
        }
    }
}
