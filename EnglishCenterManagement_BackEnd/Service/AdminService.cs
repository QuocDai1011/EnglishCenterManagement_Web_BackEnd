using EnglishCenterManagement_BackEnd.Models;

namespace EnglishCenterManagement_BackEnd.Service
{
    public class AdminService
    {
        public static Admin Mapper(Admin admin, Admin updatedAdmin)
        {
            admin.UserName = updatedAdmin.UserName;

            admin.Password = (updatedAdmin.Password == null) ? admin.Password : updatedAdmin.Password;

            admin.FullName = updatedAdmin.FullName;
            admin.Email = updatedAdmin.Email;
            admin.Gender = updatedAdmin.Gender;
            admin.Address = updatedAdmin.Address;
            admin.DateOfBirth = updatedAdmin.DateOfBirth;
            admin.PhoneNumber = updatedAdmin.PhoneNumber;
            admin.Salary = updatedAdmin.Salary;
            admin.CreateAt = updatedAdmin.CreateAt;
            admin.IsActive = updatedAdmin.IsActive;
            admin.IsSuper = updatedAdmin.IsSuper;
            admin.IsStudentManager = updatedAdmin.IsStudentManager;
            admin.IsTeacherManager = updatedAdmin.IsTeacherManager;
            admin.UpdateAt = DateOnly.FromDateTime(DateTime.Now);

            return admin;
        }
    }
}
