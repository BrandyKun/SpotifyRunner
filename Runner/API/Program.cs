using Application.Interface;
using Domain.Entities;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions();
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Configuration.GetConnectionString("DefaultSql");
builder.Services.AddCors( options => {
    options.AddPolicy(name: "NextPolicy", 
    policy => {
        policy.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyOrigin();
    });
});
builder.Services.AddControllers();
builder.Services.AddHttpClient<ISpotifyLogin, SpotifyLogin>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("NextPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
