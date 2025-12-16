using BookStore.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BookStoreContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddAuthentication("AdminAuth") // Tên scheme phải khớp với LoginController
    .AddCookie("AdminAuth", options =>
    {
        options.Cookie.Name = "AdminSession"; // Tên Cookie lưu trên trình duyệt
        options.LoginPath = "/Admin/Login";   // Nếu chưa đăng nhập thì đá về đây
        options.AccessDeniedPath = "/Admin/Login"; // Không có quyền cũng đá về đây
        options.ExpireTimeSpan = TimeSpan.FromHours(8); // Cookie sống trong 8 tiếng
    });
// ============================================================


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 2. Kích hoạt Authentication (Phải có và nằm TRƯỚC Authorization)
app.UseAuthentication();
app.UseAuthorization();

// 3. Cấu hình Route cho Area (Admin)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// 4. Route mặc định cho khách hàng
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();