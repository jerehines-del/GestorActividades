using Microsoft.EntityFrameworkCore;

namespace Gestor_Actividades.Data  // <-- OJO: este nombre
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<Tarea> Tareas { get; set; }
    }

    public class Tarea
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public string Materia { get; set; } = "";
        public DateTime Fecha { get; set; }
    }
}