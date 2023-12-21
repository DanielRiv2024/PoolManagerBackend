using PoolManagerBackend.Interface;
using PoolManagerBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IPiscinaService, PiscinaService>();
builder.Services.AddScoped<IClasesPrivadasService, ClasePrivadaService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IComentario, ComentarioService>();
builder.Services.AddScoped<ITarjetaUsuario, TarjetaUsuarioService>();
builder.Services.AddScoped<ITransacion, TransaccionService > ();
builder.Services.AddScoped<IInstructor, InstructorService>();
builder.Services.AddScoped<IEvalucionInstructor, EvaluacionService>();
builder.Services.AddScoped<IRegistroCliente, RegistroService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("NuevaPolitica", app =>
    {
        app.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("NuevaPolitica");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
