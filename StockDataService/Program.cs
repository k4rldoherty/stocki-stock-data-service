using StockDataService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<ApiService>();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.WebHost.UseKestrel(options =>
{
    // Http
    options.ListenAnyIP(8080);

    // Https - Not really needed for an internal microservice
    // options.ListenAnyIP(
    //     8081,
    //     listenOptions =>
    //     {
    //         listenOptions.UseHttps(
    //             "/https/https-cert.pfx",
    //             Environment.GetEnvironmentVariable("HTTPSCERTPASSWORD")
    //         );
    //     }
    // );
});
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(
        "OpenPolicy",
        policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowAnyOrigin(); // Change this when other services created. .WithOrigins("x", "y")
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("OpenPolicy");

app.MapControllers();

app.Run();
