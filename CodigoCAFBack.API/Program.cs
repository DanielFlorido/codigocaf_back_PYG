using CodigoCAFBack.API.Middleware;
using CodigoCAFBack.Aplicacion.Interfaces.BalanceGeneral;
using CodigoCAFBack.Aplicacion.Interfaces.Comun;
using CodigoCAFBack.Aplicacion.Interfaces.FlujoCaja;
using CodigoCAFBack.Aplicacion.Interfaces.Log;
using CodigoCAFBack.Aplicacion.Interfaces.ProgramadorPagos;
using CodigoCAFBack.Aplicacion.Interfaces.Mail;
using CodigoCAFBack.Aplicacion.Services.BalanceGeneral;
using CodigoCAFBack.Aplicacion.Services.Comun;
using CodigoCAFBack.Aplicacion.Services.FlujoCaja;
using CodigoCAFBack.Aplicacion.Services.ProgramadorPago;
using CodigoCAFBack.Aplicacion.Services.Mail;
using CodigoCAFBack.Dominio.Configuracion;
using CodigoCAFBack.Infraestructura;
using CodigoCAFBack.Infraestructura.Repository;
using Microsoft.EntityFrameworkCore;

// Existing code...

using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CodigoCAFBack.Aplicacion.Services.PYG;
using CodigoCAFBack.Aplicacion.Interfaces.PYG;
using CodigoCAFBack.Aplicacion.Interfaces.Utils;
using CodigoCAFBack.Aplicacion.Utilidades;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//use this for real database on your sql server
builder.Services.AddDbContext<CodigoCAFContext>(options =>
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DbContext"), providerOptions => providerOptions.EnableRetryOnFailure()
        ).EnableSensitiveDataLogging().EnableDetailedErrors();
    }
);

// Configuración de JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Use JwtBearerDefaults.AuthenticationScheme
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("❌ Error de autenticación: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("⚠️ Challenge lanzado: " + context.ErrorDescription);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("✅ Token validado correctamente");
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddTransient<ExceptionMiddleware>();

builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IUsuarioMulticlienteService, UsuarioMulticlienteService>();
builder.Services.AddScoped<IParametrosBalanceGeneralService, ParametrosBalanceGeneralService>();
builder.Services.AddScoped<IBalanceGeneralService, BalanceGeneralService>();
builder.Services.AddScoped<IMesesService, MesesService>();
builder.Services.AddScoped<IFlujoCajaService, FlujoCajaService>();
builder.Services.AddScoped<IParametroFlujoCajaService, ParametroFlujoCajaService>();
builder.Services.AddScoped<IClienteLogoService, ClienteLogoService>();
builder.Services.AddScoped<IBancoService, BancoService>();
builder.Services.AddScoped<ITipoCuentaService, TipoCuentaService>();
builder.Services.AddScoped<IParametroTerceroService, ParametroTerceroService>();
builder.Services.AddScoped<IProgramadorPagoService, ProgramadorPagoService>();
builder.Services.AddScoped<IItemProgramadorPagoService, ItemProgramadorPagoService>();
builder.Services.AddScoped<IPYGService, PYGService>();
builder.Services.AddScoped<IExcelService, ExcelService>();

builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<IUsuarioMulticlienteRepository, UsuarioMulticlienteRepository>();
builder.Services.AddScoped<IParametrosBalanceGeneralRepository, ParametrosBalanceGeneralRepository>();
builder.Services.AddScoped<IBalanceGeneralRepository, BalanceGeneralRepository>();
builder.Services.AddScoped<IFlujoCajaRepository, FlujoCajaRepository>();
builder.Services.AddScoped<IParametroFlujoCajaRepository, ParametroFlujoCajaRepository>();
builder.Services.AddScoped<IClienteLogoRepository, ClienteLogoRepository>();
builder.Services.AddScoped<IBancoRepository, BancoRepository>();
builder.Services.AddScoped<ITipoCuentaRepository, TipoCuentaRepository>();
builder.Services.AddScoped<IParametroTerceroRepository, ParametroTerceroRepository>();
builder.Services.AddScoped<IProgramadorPagoRepository, ProgramadorPagoRepository>();
builder.Services.AddScoped<IItemProgramadorPagoRepository, ItemProgramadorPagoRepository>();
builder.Services.AddScoped<IPYGRepository, PYGRepository>();


builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IMailService, MailService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.CongigureExceptionMiddleware();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
