using Mechera.Sec.Authorization.Tools;
using Mechera.Sec.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMecheraSecData(builder.Configuration)
    .AddScoped<IUserAuthenticator, UserAuthenticator>()
    .AddSingleton<IJwtGenerator, JwtGenerator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
