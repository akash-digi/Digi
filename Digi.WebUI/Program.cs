using Digi.WebUI.Constants;
using Digi.WebUI.Filters;
using Digi.WebUI;
using Digi.WebUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(config => config.Filters.Add(typeof(GlobalExceptionFilter)));

// Extract app config section value from appsettings.json file into AppConfig class instance and push it in dependency injection.
builder.Services.Configure<AppConfig>(builder.Configuration.GetSection(ApplicationConstant.APP_CONFIG_SECTION_NAME));

//builder.Services.AddDbContext<AppdbContext>(option =>
//  option.UseSqlServer(builder.Configuration.GetConnectionString("ProductConnection")));

// add all default services from ServiceExtensions
builder.Services.AddDefaultServices();

builder.Services.AddRouting(option =>
{
    option.LowercaseUrls = true; // /Home/SearCh-By-KeyWord/ --> /home/search-by-keyword/
    option.LowercaseQueryStrings = false; // we don't want to convert query params value to be lowercase (if it is true, eg. https://www.example.com/get-by-name?name=Akash --> https://www.example.com/get-by-name?name=akash) 
    option.AppendTrailingSlash = true; // /home/search-by-keyword ---> /home/search-by-keyword/

});

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();