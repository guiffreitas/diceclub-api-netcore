using diceclub_api_netcore.Configures;
using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Interfaces.Repositories;
using diceclub_api_netcore.Domain.Interfaces.Services;
using diceclub_api_netcore.Domain.Services;
using diceclub_api_netcore.Domain.ValueObjects;
using diceclub_api_netcore.Infrastructure.Contexts;
using diceclub_api_netcore.Infrastructure.Repositories;
using diceclub_api_netcore.Infrastructure.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Inject values for secrets
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<ApiUrls>(builder.Configuration.GetSection("ApiUrls"));
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));

//Inject dependecy for services
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();

builder.Services.AddScoped<ICacheRedisRepository, CacheRedisRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();

builder.Services.AddScoped(_ => new MySqlConnection(builder.Configuration.GetConnectionString("dice_club_db")));

//Inject connection for db context

builder.Services.AddDbContext<ApplicationDbContext>(options => 
{
    var conn = builder.Configuration.GetConnectionString("dice_club_db");
    options.UseMySql(conn, ServerVersion.AutoDetect(conn), b => b.MigrationsAssembly("diceclub-api-netcore.Infrastructure"));
});

//Inject conneciton for Redis server
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer
            .Connect(builder.Configuration.GetConnectionString("redis")!));

//Inject Identity services, EF rellations for User Class and Token generation for user confirmation and password reset 
builder.Services.AddDefaultIdentity<User>(options =>
        {
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

//Inject configure for JWT Bearer for authentication
builder.Services.AddAuthenticationConfigure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
