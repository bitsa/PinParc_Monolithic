using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PinParc_Monolith.Infrastructure.EF;
using PinParc_Monolith.Infrastructure.Location;
using PinParc_Monolith.Infrastructure.Location.Google;
using PinParc_Monolith.Infrastructure.Notifications.Email;
using PinParc_Monolith.Infrastructure.Notifications.OneSignal;
using PinParc_Monolith.Infrastructure.Notifications.Sms;
using PinParc_Monolith.Services.Authentication;
using PinParc_Monolith.Services.Authentication.Managers;
using PinParc_Monolith.Services.Calculator;
using PinParc_Monolith.Services.Couriers;
using PinParc_Monolith.Services.Orders;
using PinParc_Monolith.Services.Products;
using PinParc_Monolith.Services.Review;

namespace PinParc_Monolith
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PinParcDbContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:PinParcConnection"])
            );
            services.AddControllers().AddNewtonsoftJson(x =>
            {
                x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                //x.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                x.SerializerSettings.ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                    {
                        ProcessDictionaryKeys = true,

                    },
                };
                x.SerializerSettings.DateFormatString = @"dd/MM/yyyy";
                x.SerializerSettings.Culture = CultureInfo.InvariantCulture;
                x.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            });

            #region Identity && JWT Authentication

            services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, ApplicationUserClaimsPrincipalFactory>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("PinParcConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddSignInManager<CustomSignInManager>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = false;
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = null,//Configuration["JwtIssuer"],
                        ValidAudience = null,//Configuration["JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                        ValidateIssuerSigningKey = true,

                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    });
            });
            #endregion


            #region Services Injections

            services.AddScoped<ICouriersService, CouriersService>();
            services.AddScoped<IOrdersService, OrdersService>();
            services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ICalculatorService, CalculatorService>();
            services.AddScoped<ILocationService, GoogleLocationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOneSignalService, OneSignalService>();
            services.AddScoped<ISmsService, SmsService>();
            #endregion

            #region AutoMapper Injection

            var automapperAssemblies = new List<Assembly>();
            //automapperAssemblies.Add(typeof(UsersAutoMapperProfile).Assembly);
            automapperAssemblies.Add(typeof(Startup).Assembly);

            services.AddAutoMapper(automapperAssemblies);

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
