using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("1.0.5", new OpenApiInfo
    {
        Version = "1.0.5",
        Title = "Trading card recognizer",
        Description = "Trading card recognizer for Hackathon 2024"
    });

    c.IncludeXmlComments($"{AppContext.BaseDirectory}{builder.Environment.ApplicationName}.xml");
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("Dev", policyBuilder =>
    {
        policyBuilder
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .WithOrigins("http://localhost:5235")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(origin => true)
            .Build();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/1.0.5/swagger.json", "Trading card recognizer Documentation"));
}

app.UseHttpsRedirection();


app.UseCors("Dev");

app.UseAuthorization();

app.MapControllers();

app.Run();
