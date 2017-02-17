using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.AspNetCore.Mvc.Formatters.Json;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SimpleTokenProvider;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using NLog.Extensions.Logging;
using NLog.Web;
using Microsoft.AspNetCore.Http;

namespace EscolaDeVoce.API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddIdentity()
            services.AddMvc(
                options =>
                    {
                    options.RespectBrowserAcceptHeader = true; // false by default
                    }
                )
                .AddXmlSerializerFormatters()
                .AddJsonOptions(jsonOptions=>
                {
                    jsonOptions.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });;

            //needed for NLog.Web
            // services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSwaggerGen();

            Services.AutoMapperSetup.Config();

            // Unit Of Work
            services.AddSingleton<Infrastructure.IUnitOfWork, Repository.EntityFramework.UnitOfWork>();
            services.AddSingleton<EscolaDeVoce.Repository.EntityFramework.Interfaces.IUnitOfWork, Repository.EntityFramework.UnitOfWork>();

            // Adding Repositories
            services.AddTransient<EscolaDeVoce.Model.ICategoryRepository, Repository.EntityFramework.CategoryRepository>();
            services.AddTransient<EscolaDeVoce.Model.IProjectRepository, Repository.EntityFramework.Repositories.ProjectRepository>();
            services.AddTransient<EscolaDeVoce.Model.ICourseRepository, Repository.EntityFramework.Repositories.CourseRepository>();
            services.AddTransient<EscolaDeVoce.Model.INewsRepository, Repository.EntityFramework.Repositories.NewsRepository>();
            services.AddTransient<EscolaDeVoce.Model.IVideoRepository, Repository.EntityFramework.Repositories.VideoRepository>();
            services.AddTransient<EscolaDeVoce.Model.IUserRepository, Repository.EntityFramework.Repositories.UserRepository>();
            services.AddTransient<EscolaDeVoce.Model.IClassRoomRepository, Repository.EntityFramework.Repositories.ClassRoomRepository>();
            services.AddTransient<EscolaDeVoce.Model.IPersonRepository, Repository.EntityFramework.Repositories.PersonRepository>();
            services.AddTransient<EscolaDeVoce.Model.IFileRepository, Repository.EntityFramework.Repositories.FileRepository>();
            services.AddTransient<EscolaDeVoce.Model.IThumbRepository, Repository.EntityFramework.Repositories.ThumbRepository>();
            services.AddTransient<EscolaDeVoce.Model.IEspecialistRepository, Repository.EntityFramework.Repositories.EspecialistRepository>();
            services.AddTransient<EscolaDeVoce.Model.IPersonalityQuestionRepository, Repository.EntityFramework.Repositories.PersonalityQuestionRepository>();
            services.AddTransient<EscolaDeVoce.Model.IAnswerRepository, Repository.EntityFramework.Repositories.AnswerRepository>();
            services.AddTransient<EscolaDeVoce.Model.IEscoleteTalkRepository, Repository.EntityFramework.Repositories.EscoleteTalkRepository>();
            services.AddTransient<EscolaDeVoce.Model.IUserAnswerRepository, Repository.EntityFramework.Repositories.UserAnswerRepository>();
            services.AddTransient<EscolaDeVoce.Model.IEscoleteTalkCommentRepository, Repository.EntityFramework.Repositories.EscoleteTalkCommentRepository>();
            services.AddTransient<EscolaDeVoce.Model.IMessageRepository, Repository.EntityFramework.Repositories.MessageRepository>();
            services.AddTransient<EscolaDeVoce.Model.IAdressRepository, Repository.EntityFramework.Repositories.AddressRepository>();
            services.AddTransient<EscolaDeVoce.Model.ISchoolRepository, Repository.EntityFramework.Repositories.SchoolRepository>();
            services.AddTransient<EscolaDeVoce.Model.IDictionaryRepository, Repository.EntityFramework.Repositories.DictionaryRepository>();
            services.AddTransient<EscolaDeVoce.Model.IVideoCategoryRepository, Repository.EntityFramework.Repositories.VideoCategoryRepository>();
            services.AddTransient<EscolaDeVoce.Model.ICourseCategoryRepository, Repository.EntityFramework.CourseCategoryRepository>();
            

            services.AddTransient<EscolaDeVoce.Model.IUserVideoFavoriteRepository, Repository.EntityFramework.Repositories.UserVideoFavoriteRepository>();
            services.AddTransient<EscolaDeVoce.Model.IUserVideoStatusRepository, Repository.EntityFramework.Repositories.UserVideoStatusRepository>();
            services.AddTransient<EscolaDeVoce.Model.IUserCourseNoteRepository, Repository.EntityFramework.Repositories.UserCourseNoteRepository>();
            services.AddTransient<EscolaDeVoce.Model.IUserCourseRepository, Repository.EntityFramework.Repositories.UserCourseRepository>();

            // Adding Services
            services.AddTransient<Services.Interfaces.ICategoryService, Services.CategoryService>();
            services.AddTransient<Services.Interfaces.IProjectService, Services.ProjectService>();
            services.AddTransient<Services.Interfaces.ICourseService, Services.CourseService>();
            services.AddTransient<Services.Interfaces.INewsService, Services.NewsService>();
            services.AddTransient<Services.Interfaces.IVideoService, Services.VideoService>();
            services.AddTransient<Services.Interfaces.IUserService, Services.UserService>();
            services.AddTransient<Services.Interfaces.IClassRoomService, Services.ClassRoomService>();
            services.AddTransient<Services.Interfaces.IPersonService, Services.PersonService>();
            services.AddTransient<Services.Interfaces.IEspecialistService, Services.EspecialistService>();
            services.AddTransient<Services.Interfaces.IPersonalityQuestionService, Services.PersonalityQuestionService>();
            services.AddTransient<Services.Interfaces.IAnswearService, Services.AnswerService>();
            services.AddTransient<Services.Interfaces.IEscoleteTalkService, Services.EscoleteTalkService>();
            services.AddTransient<Services.Interfaces.IMessageService, Services.MessageService>();
            services.AddTransient<Services.Interfaces.ISchoolService, Services.SchoolService>();
            services.AddTransient<Services.Interfaces.IDictionaryService, Services.DictionaryService>();
        }

        // The secret key every token will be signed with.
        // In production, you should store this securely in environment variables
        // or a key management tool. Don't hardcode this into your application!
        private static readonly string secretKey = "mysupersecret_secretkey!123";

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // //add NLog to .NET Core
            // loggerFactory.AddNLog();

            // //Enable ASP.NET Core features (NLog.web) - only needed for ASP.NET Core users
            // app.AddNLogWeb();

            // //needed for non-NETSTANDARD platforms: configure nlog.config in your project root. NB: you need NLog.Web.AspNetCore package for this. 
            // env.ConfigureNLog(env.ContentRootPath + "/src/EscolaDeVoce.API/nlog.config");

            app.UseStaticFiles();
            // Session must be used before MVC routes.
            // app.UseSession();
            // app.UseIdentity();
            // Add JWT generation endpoint:

            // app.UseIdentity();
            //app.UseIdentityServer();

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var options = new TokenProviderOptions
            {
                Audience = "ExampleAudience",
                Issuer = "ExampleIssuer",
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            };
            
            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
            
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "ExampleIssuer",
            
                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "ExampleAudience",
            
                // Validate the token expiry
                ValidateLifetime = true,
            
                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };
            
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUi();
        }
    }
}
