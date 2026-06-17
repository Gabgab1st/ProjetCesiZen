using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CesiZen.API.Configuration;
using CesiZen.API.Data;
using CesiZen.API.Data.Seed;
using CesiZen.API.Middleware;
using CesiZen.API.Services;
using CesiZen.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ─── Configuration JWT ───────────────────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// ─── Base de données ─────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// ─── Services métier ─────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IExerciceRespirationService, ExerciceRespirationService>();

// ─── Authentication JWT ───────────────────────────────────────────────────────
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
                                       Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

builder.Services.AddAuthorization();

// ─── CORS ────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("CesiZenPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")   // Angular dev
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ─── Controllers + Swagger ───────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CESIZen API",
        Version = "v1",
        Description = "API REST pour l'application CESIZen - Santé mentale"
    });

    // Bouton Authorize dans Swagger pour tester avec JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez : Bearer {votre_token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ─── Middleware global d'exceptions ──────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();

// ─── Swagger (dev uniquement) ─────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CESIZen API v1");
        c.RoutePrefix = string.Empty; // Swagger sur /
    });
}

app.UseHttpsRedirection();
app.UseCors("CesiZenPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ─── Seed de données initiales ────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        db.Database.Migrate();
        await DataSeeder.SeedAsync(db);
        logger.LogInformation("Base de données migrée et seedée avec succès.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erreur lors de la migration / seed.");
    }
}

app.Run();