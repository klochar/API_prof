using ApiProf_A.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ExcelService>();
/*builder.Services.AddScoped<IConfiguration>();
*/
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
}
else
{
    // Configuration for a production environment
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// Configure routes
app.UseEndpoints(endpoints =>
{
    // Configure default route to point to the API controller
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=professeur}/{action=Index}/{id?}");

    // Map other controllers if needed
    endpoints.MapControllers();
});


/*app.MapControllerRoute(
    name: "default",
    pattern: "{controller=professeur}/{action=Index}/{id?}");*/

app.Run();
