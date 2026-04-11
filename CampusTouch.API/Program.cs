    using CampusTouch.API.Middlewares;
    using CampusTouch.Application.Common.Behaviors;
    using CampusTouch.Application.Features.Authentication.Vaidators;
    using CampusTouch.Infrastructure;
    using CampusTouch.Infrastructure.Persistance.Identity;
    using CampusTouch.Infrastructure.Persistance.Seed;
    using FluentValidation;
    using MediatR;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.Google;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;

    var builder = WebApplication.CreateBuilder(args);

    // ✅ Add services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddInfrastructure(builder.Configuration);

    // ✅ MediatR
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

    // ✅ FluentValidation
    builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommandValidator).Assembly);

    // ✅ Pipeline Behavior
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


    // 🔥🔥🔥 CORRECT AUTH CONFIG (VERY IMPORTANT)
    builder.Services.AddAuthentication(options =>
    {
        // ✅ Use JWT as default for APIs
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    // ✅ Cookie (used internally by Google OAuth)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)

    // ✅ Google OAuth
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.CallbackPath = "/signin-google";

        // Optional but recommended
        options.SaveTokens = true;
    })
    // ✅ JWT
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };

        options.Events = new JwtBearerEvents
        {
            // 🔐 401
            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    success = false,
                    statusCode = 401,
                    message = "Authentication required",
                    errors = new[] { "Access token is missing or invalid" },
                    timestamp = DateTime.UtcNow
                };

                await context.Response.WriteAsJsonAsync(response);
            },

            // 🔐 403
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    success = false,
                    statusCode = 403,
                    message = "Access denied",
                    errors = new[] { "You do not have permission" },
                    timestamp = DateTime.UtcNow
                };

                await context.Response.WriteAsJsonAsync(response);
            },

            // 🔥 Token issues
            OnAuthenticationFailed = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var message = context.Exception switch
                {
                    SecurityTokenExpiredException => "Token expired",
                    _ => "Invalid token"
                };

                var response = new
                {
                    success = false,
                    statusCode = 401,
                    message = message,
                    errors = new[] { context.Exception.Message },
                    timestamp = DateTime.UtcNow
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        };
    });


    // ✅ Swagger with JWT
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter JWT Token like: Bearer {your token}"
        });

        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });

    var app = builder.Build();

    // ✅ Seed Roles + Admin
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        await RoleSeeder.SeedAsync(roleManager);
        await AdminSeeder.SeedAdminAsync(userManager);
    }

    // ✅ Middleware
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<GlobalExceptionMiddleWare>();

    app.UseHttpsRedirection();

    app.UseAuthentication(); // 🔥 MUST before Authorization
    app.UseAuthorization();

    app.MapControllers();

    app.Run();