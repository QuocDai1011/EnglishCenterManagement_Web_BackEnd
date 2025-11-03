using EnglishCenterManagement_BackEnd.Models;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EnglishCenterManagement_BackEnd.Service
{
    public class StudentService
    {
        public static Student Mapper (Student student, Student updatedStudent)
        {
            if(student == null) return updatedStudent;
            else if(updatedStudent == null) return student;
            else
            {
                student.FullName = updatedStudent.FullName;
                student.UserName = updatedStudent.UserName;

                student.Password = (updatedStudent.Password != null)
                        ? updatedStudent.Password
                        : student.Password;

                student.Email = updatedStudent.Email;
                student.Gender = updatedStudent.Gender;
                student.Address = updatedStudent.Address;
                student.DateOfBirth = updatedStudent.DateOfBirth;
                student.PhoneNumber = updatedStudent.PhoneNumber;
                student.PhoneNumberOfParents = updatedStudent.PhoneNumberOfParents;
                student.UpdateAt = DateOnly.FromDateTime(DateTime.Now);
                student.IsActive = updatedStudent.IsActive;

                return student;
            }

        }
    }
}
