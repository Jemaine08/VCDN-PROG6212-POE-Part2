using ClaimFlow.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.Configure<JsonClaimRepository.StorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.Configure<AesFileEncryptor.StorageCryptoOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.Configure<FileValidationService.FileRules>(builder.Configuration.GetSection("Storage"));

builder.Services.AddSingleton<IClaimRepository, JsonClaimRepository>();
builder.Services.AddSingleton<IFileEncryptor, AesFileEncryptor>();
builder.Services.AddSingleton<IFileValidationService, FileValidationService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Claims}/{action=Create}/{id?}");

app.Run();
