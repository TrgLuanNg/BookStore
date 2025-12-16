using System.Security.Claims;
using BookStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PermissionController : Controller
    {
        private readonly BookStoreContext _context;
        public PermissionController(BookStoreContext context) => _context = context;

        // API lấy danh sách quyền của user đang đăng nhập
        // Thay thế hàm getAllFunctionDetailsByUserRoleId trong PHP
        [HttpGet]
        public async Task<IActionResult> GetMyPermissions()
        {
            // Lấy Username từ Cookie đăng nhập
            var username = User.Identity.Name;

            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return Unauthorized();

            // Join bảng FunctionDetails và Functions
            var permissions = await (from fd in _context.FunctionDetails
                                     join f in _context.Functions on fd.FunctionId equals f.Id
                                     where fd.RoleId == user.RoleId && fd.Action == true
                                     select new
                                     {
                                         FunctionId = f.Id,
                                         FunctionName = f.Name,
                                         Action = fd.Action
                                     }).ToListAsync();

            return Json(permissions);
        }
    }
}