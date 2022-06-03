using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entiites
{
    public class Student
    {
        [Key]
        public Guid Id { get; set; }

      
        public string UniversityID { get; set; }

        public string Name { get; set; }

        public double GPA { get; set; }



    }
}
