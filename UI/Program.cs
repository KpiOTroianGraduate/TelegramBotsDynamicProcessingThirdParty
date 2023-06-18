using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using UI;
using User = UI.Entities.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseInMemoryDatabase("IbMemoryDb"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();

app.MapGet("/", () => Results.Redirect("swagger"));
app.UseCookiePolicy();
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(_ => true)
    .AllowCredentials());

app.MapPost("/", async ([FromBody] Message body, ApplicationContext db, ILogger<object> logger) =>
{
    try
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.TelegramId == body.Chat.Id).ConfigureAwait(false);
        if (user == null)
        {
            var nweUser = new User
            {
                TelegramId = body.Chat.Id,
                Count = 0
            };

            await db.Users.AddAsync(nweUser).ConfigureAwait(false);
            await db.SaveChangesAsync().ConfigureAwait(false);

            return Results.Ok("Welcome, new User");
        }

        user.Count++;
        await db.SaveChangesAsync().ConfigureAwait(false);

        return Results.Ok($"Your counter = {user.Count}");
    }
    catch(Exception ex)
    {
        logger.LogError(ex, "Error");
        return Results.BadRequest(ex.Message);
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();