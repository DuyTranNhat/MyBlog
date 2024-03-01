using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Migration_EF.Models;
using Migration_EF.Security.Requirements;
using Migration_EF.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions();
var configuration = builder.Configuration;
var mailSettings = configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSettings);
builder.Services.AddSingleton<IEmailSender, SendMailService>();

builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDbContext<BlogContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnect")));

builder.Services.ConfigureApplicationCookie(options => {
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    // Trên 30 giây truy cập lại sẽ nạp lại thông tin User (Role)
    // SecurityStamp trong bảng User đổi -> nạp lại thông tinn Security
    options.ValidationInterval = TimeSpan.FromSeconds(3);
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddGoogle(option =>
        {
            var ggConfig = configuration.GetSection("Authentication:Google");
            option.ClientId = ggConfig["ClientId"];
            option.ClientSecret = ggConfig["ClientSecret"];
            option.CallbackPath = "/login-from-google";
        })
    .AddFacebook(option =>
    {
        var fbConfig = configuration.GetSection("Authentication:Facebook");
        option.AppId = fbConfig["AppId"];
        option.AppSecret = fbConfig["AppSecret"];
        //option.CallbackPath = "/login-facebook";
    });


builder.Services.AddTransient<IAuthorizationHandler, AppAuthorizationHandler>();

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("AllowEditRole", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        //policyBuilder.RequireRole("Admin");
        //policyBuilder.RequireRole("Edit");
        policyBuilder.RequireClaim("role type1", "role value1");
    });

    option.AddPolicy("InGenZ", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.Requirements.Add(new GenZRequirement());
    });

    option.AddPolicy("ShowMenuAdmin", policyBuilder =>
    {
        policyBuilder.RequireRole("Admin");
    });

    option.AddPolicy("CanupdateArticle", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.Requirements.Add(new ArticleUpdateRequirement());
    });
});


//////register Identity (*)
builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddErrorDescriber<AppIdentityErrorDescriber>()
                .AddEntityFrameworkStores<BlogContext>()
                .AddDefaultTokenProviders();

//builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<BlogContext>();


//This register service is the same register (*) above.However,They supporting IdentityUI
//builder.Services.AddDefaultIdentity<AppUser>()
//        .AddEntityFrameworkStores<BlogContext>()
//        .AddDefaultTokenProviders();



// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại

});





var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<BlogContext>();
    context.Database.EnsureCreated();
    // DbInitializer.Initialize(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//app.userAu
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

//SignInManager<AppUser> s;
//UserManager<AppUser> u;

app.Run();
    
