using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Interfaces.Services;
using diceclub_api_netcore.Domain.Services;
using diceclub_api_netcore.Domain.ValueObjects;
using diceclub_api_netcore.Infrastructure.DbContext;
using diceclub_api_netcore.Infrastructure.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
    options.SignIn.RequireConfirmedEmail = false)
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient(_ => new MySqlConnection(builder.Configuration.GetConnectionString("dice-club-db")));

builder.Services.AddDbContext<UserDbContext>(options => {
    var conn = builder.Configuration.GetConnectionString("dice-club-db");
    options.UseMySql(conn, ServerVersion.AutoDetect(conn));
});

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<ApiUrls>(builder.Configuration.GetSection("ApiUrls"));

var app = builder.Build();

//app.MapIdentityApi<IdentityUser<int>>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
