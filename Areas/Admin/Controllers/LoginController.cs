using System.Security.Claims;
using BookStore.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly BookStoreContext _context;
        public LoginController(BookStoreContext context) => _context = context;

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Index(string username, string password)
        {
            var acc = await _context.Accounts.FirstOrDefaultAsync(x => x.Username == username && x.Status == 1);

            // Lưu ý: Trong thực tế bạn nên mã hóa password (MD5/BCrypt). Ở đây so sánh trực tiếp để test.
            if (acc == null || acc.Password != password)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            if (acc.RoleId == 3) // Role 3 là khách hàng, không cho vào admin
            {
                ViewBag.Error = "Bạn không có quyền truy cập";
                return View();
            }

            // Tạo session đăng nhập
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, acc.Username),
                new Claim(ClaimTypes.Role, acc.RoleId.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync("AdminAuth", new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminAuth");
            return RedirectToAction("Index");
        }
    }
}