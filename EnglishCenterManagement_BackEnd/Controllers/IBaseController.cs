using Microsoft.AspNetCore.Mvc;

namespace EnglishCenterManagement_BackEnd.Controllers
{
    public interface IBaseController<T>
    {
        // Lấy tất cả
        Task<IActionResult> GetAll();

        // Lấy 1 theo id
        Task<IActionResult> GetById(int id);

        // Thêm mới
        Task<IActionResult> Create(T entity);

        // Cập nhật
        Task<IActionResult> Update(int id, T entity);

        // Xóa
        Task<IActionResult> Delete(int id);
    }
}
