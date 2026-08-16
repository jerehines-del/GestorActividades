using Gestor_Actividades.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// PAGINA WEB CON FORMULARIO
app.MapGet("/", async (AppDbContext db) =>
{
    var tareas = await db.Tareas.OrderBy(t => t.Fecha).ToListAsync();
    
    var html = @"
    <html>
    <head><title>Gestor de Tareas</title>
    <style>
        body{font-family:Arial; padding:20px; background:#f5f5f5;}
        .card{background:white; padding:15px; margin:10px 0; border-radius:8px;}
        input,button{padding:8px; margin:5px;}
        button{background:#4CAF50; color:white; border:none; cursor:pointer;}
    </style>
    </head>
    <body>
        <h1>Gestor de Actividades</h1>
        
        <h2>Agregar Nueva Tarea</h2>
        <form method='post' action='/agregar'>
            <input name='Titulo' placeholder='Titulo de la tarea' required>
            <input name='Materia' placeholder='Materia' required>
            <input name='Fecha' type='date' required>
            <button type='submit'>Guardar</button>
        </form>
        
        <h2>Tareas Guardadas</h2>";

    foreach(var t in tareas)
    {
      html += $@"
    <div class='card'>
        <b>{t.Titulo}</b> - {t.Materia} - {t.Fecha:dd/MM/yyyy}
        <form method='post' action='/eliminar/{t.Id}' style='display:inline; float:right;'>
            <button type='submit' style='background:red;color:white;border:none;padding:4px 8px;border-radius:4px;cursor:pointer;'>X</button>
        </form>
    </div>";

    }

    html += "</body></html>";
    return Results.Content(html, "text/html; charset=utf-8");
});

// PARA GUARDAR CUANDO LE DE CLIC
app.MapPost("/agregar", async (HttpRequest request, AppDbContext db) =>
{
    var form = await request.ReadFormAsync();
    
    var nuevaTarea = new Tarea
    {
        Titulo = form["Titulo"],
        Materia = form["Materia"],
        Fecha = DateTime.Parse(form["Fecha"])
    };

    db.Tareas.Add(nuevaTarea);
    await db.SaveChangesAsync();
    
    return Results.Redirect("/");
});
    // PARA ELIMINAR CUANDO LE DE CLIC A LA X
app.MapPost("/eliminar/{id:int}", async (int id, AppDbContext db) =>
{
    var tarea = await db.Tareas.FindAsync(id);
    if (tarea != null)
    {
        db.Tareas.Remove(tarea);
        await db.SaveChangesAsync();
    }
    return Results.Redirect("/");
});
// TU API SIGUE FUNCIONANDO TAMBIEN
app.MapGet("/tareas", async (AppDbContext db) => await db.Tareas.ToListAsync());

app.Run();