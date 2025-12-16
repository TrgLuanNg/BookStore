using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly BookStoreContext _context;
        public RoleController(BookStoreContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            // Giả sử bạn có class Role trong Model (nếu chưa có thì tạo: Id, Name)
            // return View(await _context.Roles.ToListAsync());
            return View(); // Trả về View tạm nếu chưa có bảng Role
        }

        // Cập nhật chi tiết quyền cho 1 Role
        [HttpPost]
        public async Task<IActionResult> UpdatePermission(int roleId, List<int> functionIds)
        {
            // 1. Xóa quyền cũ
            var old = _context.FunctionDetails.Where(x => x.RoleId == roleId);
            _context.FunctionDetails.RemoveRange(old);

            // 2. Thêm quyền mới
            foreach (var fid in functionIds)
            {
                _context.FunctionDetails.Add(new FunctionDetail { RoleId = roleId, FunctionId = fid, Action = true });
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}