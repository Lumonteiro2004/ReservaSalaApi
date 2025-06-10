using Microsoft.EntityFrameworkCore;
using ReservaSalasApi.Data;
using ReservaSalasApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o DbContext com SQLite
builder.Services.AddDbContext<ReservaSalaDbContext>(options =>
    options.UseSqlite("Data Source=reservas.db"));

// Adiciona serviços para controllers
builder.Services.AddControllers();

// --- Configura CORS apenas para o front ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFront", policy =>
    {
        policy
            .WithOrigins("http://127.0.0.1:5501") // Ou "http://localhost:5501" se for esse o caso
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// --- Usa a política de CORS antes de qualquer coisa relacionada à requisição ---
app.UseCors("PermitirFront");

// Redirecionamento HTTPS (pode manter ou remover dependendo se está usando só HTTP)
app.UseHttpsRedirection();

// Roteamento de controllers
app.MapControllers();

// Popula dados de exemplo no banco, se estiver vazio
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ReservaSalaDbContext>();
    dbContext.Database.EnsureCreated(); // Cria banco e tabelas se não existirem

    if (!dbContext.ReservasSalas.Any())
    {
        dbContext.ReservasSalas.AddRange(new[]
        {
            new ReservaSala { NomeSala = "Sala A", DataReserva = DateTime.Today, HoraEntrada = TimeSpan.Parse("08:00"), HoraEntrega = TimeSpan.Parse("10:00"), Responsavel = "Maria" },
            new ReservaSala { NomeSala = "Sala B", DataReserva = new DateTime(2024, 8, 15), HoraEntrada = TimeSpan.Parse("10:00"), HoraEntrega = TimeSpan.Parse("12:00"), Responsavel = "João" },
            new ReservaSala { NomeSala = "Sala C", DataReserva = DateTime.Today.AddDays(1), HoraEntrada = TimeSpan.Parse("09:00"), HoraEntrega = TimeSpan.Parse("11:00"), Responsavel = "Carlos" },
            new ReservaSala { NomeSala = "Sala D", DataReserva = new DateTime(2024, 9, 1), HoraEntrada = TimeSpan.Parse("13:00"), HoraEntrega = TimeSpan.Parse("15:00"), Responsavel = "Fernanda" },
            new ReservaSala { NomeSala = "Sala E", DataReserva = DateTime.Today.AddDays(2), HoraEntrada = TimeSpan.Parse("14:00"), HoraEntrega = TimeSpan.Parse("16:00"), Responsavel = "Ana" },
            new ReservaSala { NomeSala = "Sala F", DataReserva = new DateTime(2024, 10, 10), HoraEntrada = TimeSpan.Parse("08:30"), HoraEntrega = TimeSpan.Parse("10:30"), Responsavel = "Roberto" },
            new ReservaSala { NomeSala = "Sala G", DataReserva = DateTime.Today.AddDays(3), HoraEntrada = TimeSpan.Parse("11:00"), HoraEntrega = TimeSpan.Parse("13:00"), Responsavel = "Juliana" },
            new ReservaSala { NomeSala = "Sala H", DataReserva = new DateTime(2024, 12, 5), HoraEntrada = TimeSpan.Parse("15:00"), HoraEntrega = TimeSpan.Parse("17:00"), Responsavel = "Marcos" },
            new ReservaSala { NomeSala = "Sala I", DataReserva = DateTime.Today.AddDays(4), HoraEntrada = TimeSpan.Parse("09:30"), HoraEntrega = TimeSpan.Parse("11:30"), Responsavel = "Beatriz" },
            new ReservaSala { NomeSala = "Sala J", DataReserva = new DateTime(2024, 7, 20), HoraEntrada = TimeSpan.Parse("13:30"), HoraEntrega = TimeSpan.Parse("15:30"), Responsavel = "Paulo" }
        });

        dbContext.SaveChanges();
    }
}

app.Run();
