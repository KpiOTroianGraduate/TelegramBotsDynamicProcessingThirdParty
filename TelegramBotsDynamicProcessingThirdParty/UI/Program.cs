using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseInMemoryDatabase("IbMemoryDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/", async ([FromBody] Message body, ApplicationContext db) =>
{
    try
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.TelegramId == body.Chat.Id).ConfigureAwait(false);
        if (user == null)
        {
            var nweUser = new UI.Entities.User
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
    catch
    {
        return Results.BadRequest();
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
