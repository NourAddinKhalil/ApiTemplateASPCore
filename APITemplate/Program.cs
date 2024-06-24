using APITemplate.Helpers;
using APITemplate.InitializeDependancies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

// App custom services
builder.Services.SetupCustomServices();

// EF Core
builder.Services.SetupEFCore(builder.Configuration);

//JWT
builder.Services.SetupJWTAuthentication(builder.Configuration);


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.SetupSwagger();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.SetupSwaggerForDevolopment();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
