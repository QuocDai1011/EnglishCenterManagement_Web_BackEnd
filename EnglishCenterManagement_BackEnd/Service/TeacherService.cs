using EnglishCenterManagement_BackEnd.Models;

namespace EnglishCenterManagement_BackEnd.Service
{
    public class TeacherService
    {
        public static Teacher Mapper(Teacher teacher, Teacher updatedTeacher) {
            if (teacher == null) return updatedTeacher;
            if (updatedTeacher == null) return teacher;

            teacher.UserName = updatedTeacher.UserName;

            teacher.Password = (updatedTeacher.Password == null) ? teacher.Password : updatedTeacher.Password;

            teacher.FullName = updatedTeacher.FullName;
            teacher.Email = updatedTeacher.Email;
            teacher.Gender = updatedTeacher.Gender;
            teacher.Address = updatedTeacher.Address;
            teacher.DateOfBirth = updatedTeacher.DateOfBirth;
            teacher.PhoneNumber = updatedTeacher.PhoneNumber;
            teacher.Salary = teacher.Salary;
            teacher.CreateAt = updatedTeacher.CreateAt;
            teacher.IsActive = updatedTeacher.IsActive;
            teacher.UpdateAt = DateOnly.FromDateTime(DateTime.Now);

            return teacher;
        }
    }
}
