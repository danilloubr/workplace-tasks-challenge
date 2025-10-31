// Em: backend/src/WorkplaceTasks.Api/Program.cs
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WorkplaceTasks.Infrastructure.Data;
using WorkplaceTasks.Api.Middleware; // Importe o middleware
// (Mais tarde, você adicionará os 'usings' dos seus serviços e repositórios aqui)
// ex: using WorkplaceTasks.Application.Interfaces;
// ex: using WorkplaceTasks.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configuração de Serviços (Injeção de Dependência) ---

// Adiciona o CORS para permitir que o Angular (rodando em http://localhost:4200)
// acesse esta API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // URL do seu app Angular
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Registra o DbContext (A "ponte" para o banco de dados)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// (Placeholder) Registra os serviços e repositórios da sua aplicação.
// Você vai descomentar e adicionar suas interfaces/classes aqui.
// builder.Services.AddScoped<IUserRepository, UserRepository>();
// builder.Services.AddScoped<ITaskRepository, TaskRepository>();
// builder.Services.AddScoped<ITaskService, TaskService>();
// builder.Services.AddScoped<IAuthService, AuthService>();


// Adiciona os serviços de Autenticação
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Configura o JWT (as chaves virão do seu appsettings.json)
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "default_super_secret_key_12345"))
    };
});

// Adiciona o serviço de Autorização (onde as Roles "admin", "manager" são usadas)
builder.Services.AddAuthorization();

// Serviços padrão da API
builder.Services.AddControllers();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true; // Força todas as URLs a serem minúsculas
});
builder.Services.AddEndpointsApiExplorer();

// Configura o Swagger/OpenAPI para suportar o envio de Token JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkplaceTasks API", Version = "v1" });

    // Adiciona a definição de segurança (Bearer)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT"
    });

    // Adiciona o requisito de segurança aos endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// --- 2. Construção da Aplicação ---

var app = builder.Build();

// --- 3. Configuração do Pipeline HTTP (Middlewares) ---

// Configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // (Opcional: Middleware de tratamento de exceções para Dev)
    app.UseDeveloperExceptionPage(); 
}

// (Opcional: Middleware global de tratamento de erros - recomendado)
app.UseMiddleware<GlobalErrorHandlingMiddleware>();

// Habilita o HTTPS
app.UseHttpsRedirection();

// Habilita o CORS (DEVE vir antes de UseAuthentication/UseAuthorization)
app.UseCors("AllowAngularDev");

// Habilita a Autenticação (Verifica "quem" você é)
app.UseAuthentication();

// Habilita a Autorização (Verifica "o que" você pode fazer)
app.UseAuthorization();

// Mapeia os Controladores (Endpoints)
app.MapControllers();

// Inicia a aplicação
app.Run();