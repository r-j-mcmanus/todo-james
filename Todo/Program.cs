using Microsoft.EntityFrameworkCore;
using DotNetEnv;

Env.Load();

// the name is arbitrary
var MyAllowedSpecificOrigins = "_MyAllowedSpecificOrigins";


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("ToDoList"));
builder.Services.AddDbContext<UserDb>(opt => opt.UseInMemoryDatabase("UserList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer(); 
// Enables the API Explorer, which is a service that provides metadata about the HTTP API. 
// The API Explorer is used by Swagger to generate the Swagger document.
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "TodoAPI";
    config.Title = "TodoAPI v1";
    config.Version = "v1";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowedSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("http://127.0.0.1:5501")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                      });
});

var app = builder.Build();
app.UseCors(MyAllowedSpecificOrigins); 


if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "TodoAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.MapUserEndpoints();
//app.MapTodoEndpoints();

app.MapGet("/", () => "Hello World!").RequireCors(MyAllowedSpecificOrigins);

app.MapGet("/todoitems", async (TodoDb db) => await db.Todos.ToListAsync());

app.MapPatch("/todoitems/{id}", async (int id, bool isComplete, TodoDb db)=> {
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        todo.IsComplete = isComplete;
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
}
);

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) => 
{
    if (await db.Todos.FindAsync(id) is Todo todo){
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
}
);

app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        todo.IsComplete = inputTodo.IsComplete;
        todo.Name = inputTodo.Name;
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
}
);

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) => 
{
    if (await db.Todos.FindAsync(id) is Todo todo){
        return Results.Ok(new TodoDTO(todo));
    }

    return Results.NotFound();
}
);

app.Run();
