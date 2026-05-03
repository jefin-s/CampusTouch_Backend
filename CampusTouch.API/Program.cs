//using Amazon.SecretsManager;
//using Amazon.SecretsManager.Model;
//using CampusTouch.API.Middlewares;
//using CampusTouch.Application.Common.Behaviors;
//using CampusTouch.Application.Features.Authentication.Vaidators;
//using CampusTouch.Infrastructure;
//using CampusTouch.Infrastructure.Persistance.Identity;
//using CampusTouch.Infrastructure.Persistance.Seed;
//using FluentValidation;
//using MediatR;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.Google;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Serilog;
//using System.Text;
//using System.Text.Json;

//var builder = WebApplication.CreateBuilder(args);

//// 🔥 Load AWS Secrets
//var client = new AmazonSecretsManagerClient(Amazon.RegionEndpoint.APSouth1);
//var secretName = "campustouch-prod-secrets";

//try
//{
//    var response = await client.GetSecretValueAsync(new GetSecretValueRequest
//    {
//        SecretId = secretName
//    });

//    var jsonDoc = JsonDocument.Parse(response.SecretString);
//    var dict = new Dictionary<string, string>();

//    void AddToDict(JsonElement element, string prefix = "")
//    {
//        foreach (var prop in element.EnumerateObject())
//        {
//            var key = string.IsNullOrEmpty(prefix)
//                ? prop.Name
//                : $"{prefix}:{prop.Name}";

//            if (prop.Value.ValueKind == JsonValueKind.Object)
//                AddToDict(prop.Value, key);
//            else
//                dict[key] = prop.Value.ToString();
//        }
//    }

//    AddToDict(jsonDoc.RootElement);
//    builder.Configuration.AddInMemoryCollection(dict);

//    Console.WriteLine("✅ Secrets loaded");
//    Console.WriteLine("ClientId: " + builder.Configuration["Authentication:Google:ClientId"]);
//}
//catch (Exception ex)
//{
//    Console.WriteLine("❌ Secret load failed: " + ex.Message);
//}

//// ✅ Serilog
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .Enrich.FromLogContext()
//    .CreateLogger();

//builder.Host.UseSerilog();

//try
//{
//    Log.Information("🚀 Application Starting...");

//    // Services
//    builder.Services.AddControllers();
//    builder.Services.AddEndpointsApiExplorer();
//    builder.Services.AddSwaggerGen();
//    builder.Services.AddInfrastructure(builder.Configuration);

//    builder.Services.AddMediatR(cfg =>
//        cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

//    builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommandValidator).Assembly);

//    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
//    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

//    // 🔐 Authentication (FIXED ORDER)
//    var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
//    var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

//    Console.WriteLine($"🔥 FINAL ClientId: {googleClientId}");

//    var authBuilder = builder.Services.AddAuthentication(options =>
//    {
//        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    })
//    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
//    {
//        options.TokenValidationParameters = new()
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//        };

//        options.Events = new JwtBearerEvents
//        {
//            OnChallenge = async context =>
//            {
//                context.HandleResponse();
//                context.Response.StatusCode = 401;

//                await context.Response.WriteAsJsonAsync(new
//                {
//                    success = false,
//                    message = "Authentication required"
//                });
//            }
//        };
//    });

//    // ✅ Google (same logic, correct placement)
//    if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
//    {
//        authBuilder.AddGoogle(options =>
//        {
//            options.ClientId = googleClientId;
//            options.ClientSecret = googleClientSecret;
//            options.CallbackPath = "/signin-google";
//            options.SaveTokens = true;
//        });

//        Console.WriteLine("✅ Google Auth Enabled");
//    }
//    else
//    {
//        Console.WriteLine("❌ Google Auth DISABLED");
//    }

//    // Redis
//    builder.Services.AddStackExchangeRedisCache(options =>
//    {
//        options.Configuration = "campustouch-redis-yco7ym.serverless.aps1.cache.amazonaws.com:6379,ssl=True,abortConnect=False";
//        options.InstanceName = "CampusTouch_";
//    });

//    // CORS
//    builder.Services.AddCors(options =>
//    {
//        options.AddPolicy("AllowFrontend", policy =>
//        {
//            policy.WithOrigins("http://localhost:5174")
//                  .AllowAnyHeader()
//                  .AllowAnyMethod()
//                  .AllowCredentials();
//        });
//    });

//    var app = builder.Build();

//    using (var scope = app.Services.CreateScope())
//    {
//        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//        db.Database.Migrate();
//    }

//    using (var scope = app.Services.CreateScope())
//    {
//        var services = scope.ServiceProvider;
//        await RoleSeeder.SeedAsync(services.GetRequiredService<RoleManager<IdentityRole>>());
//        await AdminSeeder.SeedAdminAsync(services.GetRequiredService<UserManager<ApplicationUser>>());
//    }

//    app.UseMiddleware<GlobalExceptionMiddleWare>();
//    app.UseSerilogRequestLogging();

//    app.UseCors("AllowFrontend");

//    app.UseAuthentication();
//    app.UseAuthorization();

//    app.MapControllers();
//    app.MapGet("/", () => "CampusTouch API is running 🚀");

//    app.Run();
//}
//catch (Exception ex)
//{
//    Log.Fatal(ex, "Application failed to start");
//}
//finally
//{
//    Log.CloseAndFlush();
//}


//using Amazon.SecretsManager;
//using Amazon.SecretsManager.Model;
//using CampusTouch.API.Middlewares;
//using CampusTouch.Application.Common.Behaviors;
//using CampusTouch.Application.Features.Authentication.Vaidators;
//using CampusTouch.Infrastructure;
//using CampusTouch.Infrastructure.Persistance.Identity;
//using CampusTouch.Infrastructure.Persistance.Seed;
//using FluentValidation;
//using MediatR;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.Google;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Serilog;
//using System.Text;
//using System.Text.Json;

//var builder = WebApplication.CreateBuilder(args);

//#region 🔧 CONFIGURATION LOAD (CORRECT ORDER)

//// ✅ Load base configs
//builder.Configuration
//    .AddJsonFile("appsettings.json", optional: false)
//    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
//    .AddEnvironmentVariables();

//// 🔐 Load AWS Secrets ONLY in production
//if (builder.Environment.IsProduction())
//{
//    try
//    {
//        var client = new AmazonSecretsManagerClient(Amazon.RegionEndpoint.APSouth1);
//        var secretName = "campustouch-prod-secrets";

//        var response = await client.GetSecretValueAsync(new GetSecretValueRequest
//        {
//            SecretId = secretName
//        });

//        var jsonDoc = JsonDocument.Parse(response.SecretString);
//        var dict = new Dictionary<string, string>();

//        void AddToDict(JsonElement element, string prefix = "")
//        {
//            foreach (var prop in element.EnumerateObject())
//            {
//                var key = string.IsNullOrEmpty(prefix)
//                    ? prop.Name
//                    : $"{prefix}:{prop.Name}";

//                if (prop.Value.ValueKind == JsonValueKind.Object)
//                    AddToDict(prop.Value, key);
//                else
//                    dict[key] = prop.Value.ToString();
//            }
//        }

//        AddToDict(jsonDoc.RootElement);
//        builder.Configuration.AddInMemoryCollection(dict);

//        Log.Information("✅ AWS Secrets loaded successfully");
//    }
//    catch (Exception ex)
//    {
//        Log.Error($"❌ Failed to load AWS secrets: {ex.Message}");
//    }
//}

//#endregion

//#region 📊 SERILOG

//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .Enrich.FromLogContext()
//    .CreateLogger();

//builder.Host.UseSerilog();

//#endregion

//try
//{
//    Log.Information("🚀 Application Starting...");

//    #region 🧩 SERVICES

//    builder.Services.AddControllers();
//    builder.Services.AddEndpointsApiExplorer();
//    builder.Services.AddSwaggerGen();

//    builder.Services.AddInfrastructure(builder.Configuration);

//    builder.Services.AddMediatR(cfg =>
//        cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

//    builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommandValidator).Assembly);

//    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
//    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

//    #endregion

//    #region 🔐 AUTHENTICATION

//    var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
//    var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

//    var authBuilder = builder.Services.AddAuthentication(options =>
//    {
//        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    })
//    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
//    {
//        options.TokenValidationParameters = new()
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//        };

//        options.Events = new JwtBearerEvents
//        {
//            OnChallenge = async context =>
//            {
//                context.HandleResponse();
//                context.Response.StatusCode = 401;

//                await context.Response.WriteAsJsonAsync(new
//                {
//                    success = false,
//                    message = "Authentication required"
//                });
//            }
//        };
//    });

//    if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
//    {
//        authBuilder.AddGoogle(options =>
//        {
//            options.ClientId = googleClientId;
//            options.ClientSecret = googleClientSecret;
//            options.CallbackPath = "/signin-google";
//            options.SaveTokens = true;
//        });

//        Log.Information("✅ Google Auth Enabled");
//    }
//    else
//    {
//        Log.Warning("❌ Google Auth Disabled");
//    }

//    #endregion

//    #region ⚡ REDIS

//    builder.Services.AddStackExchangeRedisCache(options =>
//    {
//        options.Configuration = builder.Configuration["Redis:ConnectionString"];
//        options.InstanceName = "CampusTouch_";
//    });

//    #endregion

//    #region 🌐 CORS

//    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

//    builder.Services.AddCors(options =>
//    {
//        options.AddPolicy("AllowFrontend", policy =>
//        {
//            policy.WithOrigins(allowedOrigins ?? Array.Empty<string>())
//                  .AllowAnyHeader()
//                  .AllowAnyMethod()
//                  .AllowCredentials();
//        });
//    });

//    #endregion

//    var app = builder.Build();

//    #region 🔥 MIDDLEWARE

//    if (!app.Environment.IsDevelopment())
//    {
//        app.UseExceptionHandler("/error");
//        app.UseHsts();
//    }

//    app.UseHttpsRedirection();

//    if (app.Environment.IsDevelopment())
//    {
//        app.UseSwagger();
//        app.UseSwaggerUI();
//    }

//    app.UseMiddleware<GlobalExceptionMiddleWare>();

//    app.UseSerilogRequestLogging();

//    app.UseCors("AllowFrontend");

//    app.UseAuthentication();
//    app.UseAuthorization();

//    #endregion

//    #region 🗄 DATABASE & SEEDING

//    using (var scope = app.Services.CreateScope())
//    {
//        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//        db.Database.Migrate();
//    }

//    using (var scope = app.Services.CreateScope())
//    {
//        var services = scope.ServiceProvider;
//        await RoleSeeder.SeedAsync(services.GetRequiredService<RoleManager<IdentityRole>>());
//        await AdminSeeder.SeedAdminAsync(services.GetRequiredService<UserManager<ApplicationUser>>());
//    }

//    #endregion

//    #region 📡 ENDPOINTS

//    app.MapControllers();

//    app.MapGet("/", () => "CampusTouch API is running 🚀");
//    app.MapGet("/health", () => "Healthy"); 

//    #endregion

//    app.Run();
//}
//catch (Exception ex)
//{
//    Log.Fatal(ex, "Application failed to start");
//}
//finally
//{
//    Log.CloseAndFlush();
//}

using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
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
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

#region 🔧 CONFIGURATION LOAD

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(); // 🔥 FIX
}

// 🔐 AWS Secrets
if (builder.Environment.IsProduction())
{
    try
    {
        var client = new AmazonSecretsManagerClient(Amazon.RegionEndpoint.APSouth1);
        var response = await client.GetSecretValueAsync(new GetSecretValueRequest
        {
            SecretId = "campustouch-prod-secrets"
        });

        var jsonDoc = JsonDocument.Parse(response.SecretString);
        var dict = new Dictionary<string, string>();

        void AddToDict(JsonElement element, string prefix = "")
        {
            foreach (var prop in element.EnumerateObject())
            {
                var key = string.IsNullOrEmpty(prefix)
                    ? prop.Name
                    : $"{prefix}:{prop.Name}";

                if (prop.Value.ValueKind == JsonValueKind.Object)
                    AddToDict(prop.Value, key);
                else
                    dict[key] = prop.Value.ToString();
            }
        }

        AddToDict(jsonDoc.RootElement);
        builder.Configuration.AddInMemoryCollection(dict);

        Log.Information("AWS Secrets loaded successfully");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Failed to load AWS secrets");
        throw; // 🔥 stop app if secrets fail
    }
}

#endregion

#region 📊 SERILOG

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

#endregion

try
{
    Log.Information("🚀 Application Starting...");

    #region 🧩 SERVICES

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddSwaggerGen();
    }

    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

    builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommandValidator).Assembly);

    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

    #endregion

    #region 🔐 AUTHENTICATION

    var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
    var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    var authBuilder = builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

    if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
    {
        authBuilder.AddGoogle(options =>
        {
            options.ClientId = googleClientId;
            options.ClientSecret = googleClientSecret;
            options.CallbackPath = "/signin-google";
            options.SaveTokens = true;
        });

        Log.Information("Google Auth Enabled");
    }
    else
    {
        Log.Warning("Google Auth Disabled");
    }

    #endregion

    #region ⚡ REDIS

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration["Redis:ConnectionString"];
        options.InstanceName = "CampusTouch_";
    });

    #endregion

    #region 🌐 CORS

    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            if (allowedOrigins != null && allowedOrigins.Length > 0)
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            }
            else
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            }
        });
    });

    #endregion

    var app = builder.Build();

    #region 🔥 MIDDLEWARE

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
    });

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<GlobalExceptionMiddleWare>();

    app.UseSerilogRequestLogging();

    app.UseCors("AllowFrontend");

    app.UseAuthentication();
    app.UseAuthorization();

    #endregion

    #region 🗄 DATABASE & SEEDING

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        await RoleSeeder.SeedAsync(services.GetRequiredService<RoleManager<IdentityRole>>());
        await AdminSeeder.SeedAdminAsync(services.GetRequiredService<UserManager<ApplicationUser>>());
    }

    #endregion

    #region 📡 ENDPOINTS

    app.MapControllers();

    app.MapGet("/", () => "CampusTouch API is running 🚀");
    app.MapGet("/health", () => "Healthy");

    #endregion

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}