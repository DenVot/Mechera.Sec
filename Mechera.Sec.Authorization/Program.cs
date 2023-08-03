using Mechera.Sec.Authorization;
using Mechera.Sec.Authorization.Tools;
using Mechera.Sec.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#if DEBUG
builder.Services.AddMecheraSecData(
    builder.Configuration.GetConnectionString("MecheraSecDB")!, 
    builder.Configuration.GetConnectionString("Redis")!);
#else
builder.Services.AddMecheraSecData(
    EnvConfig.DbConnectionString!,
    EnvConfig.RedisConnectionString!);
#endif

builder.Services.AddScoped<IUserAuthenticator, UserAuthenticator>()
    .AddScoped<IUserManager, UserManager>()
    .AddSingleton<IJwtGenerator, JwtGenerator>()
    .AddHostedService<DataSetupService>();

builder.Services.AddAuthorization()    
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
#if DEBUG
        var config = builder.Configuration;
#else
        var jwtKey = EnvConfig.JwtKey;
        var jwtIssuer = EnvConfig.JwtIssuer;

        if (jwtKey == null || jwtIssuer == null)
        {
            throw new NullReferenceException("JWT settings are null");
        }
#endif
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
#if RELEASE
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
#else
            ValidIssuer = config["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
#endif
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
