using Inredningsbutik.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers + auto-400 -> ValidationProblemDetails
builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            // Detta säkerställer att 400 blir ValidationProblemDetails
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred.",
                Type = "https://httpstatuses.com/400",
                Instance = context.HttpContext.Request.Path
            };

            return new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    });

// ProblemDetails (för 404/500 och generellt)
builder.Services.AddProblemDetails();

// OpenAPI
builder.Services.AddOpenApi();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// 500 -> ProblemDetails
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionFeature?.Error;

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Type = "https://httpstatuses.com/500",
            Instance = context.Request.Path
        };

        if (app.Environment.IsDevelopment() && exception is not null)
        {
            problem.Detail = exception.Message;
        }

        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    });
});

// 404 (och andra statuskoder som inte redan har body) -> ProblemDetails
app.UseStatusCodePages(async statusCodeContext =>
{
    var context = statusCodeContext.HttpContext;

    // Om någon redan skrivit en body, låt den vara
    if (context.Response.HasStarted || context.Response.ContentLength > 0)
        return;

    var statusCode = context.Response.StatusCode;

    // Sätt content-type och skriv ProblemDetails
    context.Response.ContentType = "application/problem+json";

    var problem = new ProblemDetails
    {
        Status = statusCode,
        Title = statusCode switch
        {
            StatusCodes.Status404NotFound => "Resource not found.",
            StatusCodes.Status401Unauthorized => "Unauthorized.",
            StatusCodes.Status403Forbidden => "Forbidden.",
            _ => "Request failed."
        },
        Type = $"https://httpstatuses.com/{statusCode}",
        Instance = context.Request.Path
    };

    await context.Response.WriteAsJsonAsync(problem);
});

app.MapControllers();

app.Run();
