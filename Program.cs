using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchoolPortal;
using SchoolPortal.Model;
using SchoolPortal.Repository;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = builder.Configuration;
string? conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SchoolDbContext>(opt =>
    opt.UseNpgsql(conn));


string issuer = builder.Configuration.GetSection("Jwt:Issuer").Value.ToString();
string key = builder.Configuration.GetSection("Jwt:Key").Value.ToString();

builder.Services.AddScoped(typeof(IAllRepository<>), typeof(AllRepository<>));
builder.Services.AddScoped<IRegisterRepository, RegisterRepository>();
// Add services to the container.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidateAudience = false,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               ValidIssuer = issuer,
               ValidAudience = issuer,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
           };
           options.Events = new JwtBearerEvents
           {
               OnAuthenticationFailed = context =>
               {
                   context.Response.OnStarting(() =>
                   {
                       context.Response.StatusCode = 401;
                       return Task.CompletedTask;
                   });

                   if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                   {
                       context.Response.Headers.Add("Token-Expired", "true");
                   }
                   return Task.CompletedTask;
               }
           };

       });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "School API", Version = "v1" });
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        //In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    // Configure Swagger to use the Bearer token


    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
            jwtSecurityScheme,
                    Array.Empty<string>()
                }
            }
    );
});
AllHelper.RegisterMappings();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowAll");
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "School API V1");
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
