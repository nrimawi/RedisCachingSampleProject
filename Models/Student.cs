using System;
using System.ComponentModel.DataAnnotations;

namespace Modeles
{
    public class Student
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "UniversityID is required")]

        public string UniversityID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "GPA is required")]
        public double GPA { get; set; }
    }

}