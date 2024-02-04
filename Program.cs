using Microsoft.OpenApi.Models;
using PizzaStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Pizzas") ?? "Data Source=Pizzas.db";



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo{ Title = "PizzaStore API", Description = "Making the Pizzas you love", Version = "v1"});
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtConfig => {
        jwtConfig.Authority = "https://example.com";
        jwtConfig.TokenValidationParameters = new(){
            ValidAudience = "myAudience",
            ValidIssuer = "https://example.com"
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSqlite<PizzaDb>(connectionString);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
});

app.MapGet("/", () => "Hello World!");
app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync());
app.MapPost("/pizzas", [Authorize] async (PizzaDb db, Pizza pizza) => {
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});

app.MapGet("/Pizzas/{id}", async (PizzaDb db, int id) => await db.Pizzas.FindAsync(id));
app.MapPut("/Pizza/{id}", async (PizzaDb db, Pizza update, int id) => {
    var pizza = await db.Pizzas.FindAsync(id);
    if(pizza is null)
    return Results.NotFound();

    pizza.Name = update.Name;
    pizza.Description = update.Description;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/Pizzas/{id}", async (PizzaDb db, int id) => {
    var pizza = await db.Pizzas.FindAsync(id);
    if(pizza is null) return Results.NotFound();

    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();
    return Results.NoContent();
});



app.Run();
