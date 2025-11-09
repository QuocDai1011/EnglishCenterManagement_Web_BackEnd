namespace EnglishCenterManagement_BackEnd.Utils
{
    // Cấu trúc dữ liệu để trả về vai trò và email đã chuẩn hóa
    public class LoginInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Student"; // Mặc định là Student
    }

    public static class AuthUtils
    {
        public static LoginInfo ParseUsername(string inputUsername)
        {
            var loginInfo = new LoginInfo();

            loginInfo.Email = inputUsername.Trim();

            if (loginInfo.Email.Contains("@admin"))
                loginInfo.Role = "Admin";
            else if (loginInfo.Email.Contains("@teacher"))
                loginInfo.Role = "Teacher";
            else
                loginInfo.Role = "Student";


            return loginInfo;
        }
    }
}