namespace EnglishCenterManagement_BackEnd.Utils
{
    public class ErrorEnums
    {
        public int ErrorCode { get; }
        public string Message { get; }

        public string? Model { get; }

        private ErrorEnums(int code, string message, string model = "")
        {
            ErrorCode = code;
            Model = model;
            Message = model == "" ? message : $"{message} {model}";
        }

        public static readonly ErrorEnums NOT_FOUND = new(404, "Không tìm thấy dữ liệu.");
        public static ErrorEnums NOT_FOUND_WITH_MODEL(string model = "")
       => new ErrorEnums(404, "Không tìm thấy dữ liệu:", model);

        public static readonly ErrorEnums FORMAT_MISTAKE = new(400, "Dữ liệu truyền vào không đúng định dạng.");
        public static readonly ErrorEnums LACK_OF_FIELD = new(400, "Dữ liệu bị thiếu. Vui lòng nhập dữ liệu còn thiếu!");
        public static readonly ErrorEnums DATA_REMOVED = new(400, "Dữ liệu đã bị xóa trước đó.");
        public static readonly ErrorEnums SERVER_ERROR = new(500, "Lỗi server.");
        public static readonly ErrorEnums TYPE_OF_DATA_MISTAKE = new(400, "Kiểu dữ liệu không hợp lệ.");
        public static readonly ErrorEnums VALIDATION_FAIL = new(400, "Xác thực thất bại.");
        public static readonly ErrorEnums JWT_NOT_FOUND = new(401, "JWT chưa được gửi đến.");
        public static readonly ErrorEnums JWT_EXPIRED = new(401, "JWT đã hết hạn sử dụng.");
        public static readonly ErrorEnums AUTHORIZATION_FORMAT_MISTAKE = new(401, "Authorization header không đúng định dạng.");
        public static readonly ErrorEnums TOKERN_NOT_ENOUGH_RIGHTS = new(403, "Token của bạn không đủ quyền truy cập vào đường dẫn này.");
        public static readonly ErrorEnums USERNAME_EXIST = new(409, "Tên đăng nhập đã tồn tại.");
        public static readonly ErrorEnums CONTENT_TYP_MISTAKE = new(415, "Content-Type không đúng định dạng.");
        public static readonly ErrorEnums LOGIC_DATA_MISTAKE = new(422, "Dữ liệu bị sai về mặt logic. Vui lòng kiểm tra lại!");

        public static readonly ErrorEnums ACCOUNT_DISABLE_OR_NOTEXIST = new(401, "Tài khoản không tồn tại hoặc bị vô hiệu hóa.");
    }
}
