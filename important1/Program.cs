using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using important1.Areas.Identity.Data;
using important1.Models;
using important1.Email;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UserContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserContextConnection")));
/*builder.Services.AddDbContext<important1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserContextConnection")));
*/
builder.Services.AddDefaultIdentity<important1User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<UserContext>();;

// Add services to the container.
builder.Services.AddControllersWithViews();

var config = builder.Configuration;
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<EmailSenderOptions>(options =>
{
    options.Host = config["MailSettings:Host"];
    options.Port = int.Parse(config["MailSettings:Port"]);
    options.User = config["MailSettings:User"];
    options.Pass = config["MailSettings:Pass"];
    options.Name = config["MailSettings:Name"];
    options.Sender = config["MailSettings:User"];
});


var app = builder.Build();




using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Customer", "Seller", "Admin" };
    IdentityResult roleResult;
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
    app.MapControllerRoute(
    name: "default",
    pattern: "{controller=UserBooks}/{action=Index}/{id?}");
});


app.MapRazorPages();
app.Run();
