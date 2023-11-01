using Microsoft.OpenApi.Models;
using Umbraco.Cms.Infrastructure.DependencyInjection;
using WorldDiabetesFoundation.Core.Infrastructure;
using WorldDiabetesFoundation.Core.Infrastructure.AutoRegister;
using WorldDiabetesFoundation.Core.Infrastructure.ScheduledTasks;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.ContentExtractions;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Importers;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Queries;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Searchers;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Searchers.GeneralSearch;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.Searchers.GeneralSearch.Setup;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoTools;
using WorldDiabetesFoundation.Core.Infrastructure.UmbracoTools.DataExtractions;
using WorldDiabetesFoundation.Core.Infrastructure.Utilities;
using WorldDiabetesFoundation.Core.Infrastructure.Utilities.DataExtractions;
using WorldDiabetesFoundation.Core.Services;
using WorldDiabetesFoundation.Core.Services.Interfaces;
using WorldDiabetesFoundation.Web.Infrastructure.Integration;
using ProjectService = WorldDiabetesFoundation.Core.Infrastructure.UmbracoServices.ContentExtractions.ProjectService;

namespace WorldDiabetesFoundation.Web;

    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="webHostEnvironment">The web hosting environment.</param>
        /// <param name="config">The configuration.</param>
        /// <remarks>
        /// Only a few services are possible to be injected here https://github.com/dotnet/aspnetcore/issues/9337.
        /// </remarks>
        public Startup(IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _env = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {   
            services.AddUmbraco(_env, _config)
                .AddBackOffice()
                .AddDeliveryApi()
                .AddExamine()
                .AddWebsite()
                .AddComposers()
                .Build();

            services.AddControllers();

            services.AddEndpointsApiExplorer();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "content-api",
                    new OpenApiInfo
                    {
                        Title = "Content API",
                        // Version can be removed if you don't require different document versions
                        Version = "1.0",
                    });
            });
            
            services.AddHttpServices();

            services.AddTransient<INewsService, NewsService>();

            services.AddTransient<IStoryService, StoryService>();

            services.AddTransient<IProjectService, Core.Services.ProjectService>();
            
            services.AddSingleton<IProjectSettings, ProjectSettings>(sp =>
            {
                var _securityToken = "PD@OA-9xd+4-NPTo.NOr=Oxz6VMBm-hI";
                
                ProjectSettings projectSettings = new ()
                {
                    SecurityToken = _securityToken,
                    ApiUrl = $"https://prod-143.westeurope.logic.azure.com/workflows/0299124fc4a9433ca0c2037beb115baa/triggers/manual/paths/invoke/GetCMSData/{_securityToken}?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=-uSgygZ_g2lEl9KKtae0qQk8YjoUak0YJJq1K6kbzhA",
                    SecretKey = "Dz8A0gHQP",
                    RequestBody = new RequestBody("test", "test", "test", "test")
                };
                
                return projectSettings;
            });

            services.AddTransient<JsonSerializer>();

            services.AddTransient<DataExtractor>();

            services.AddTransient<ContentNodesExtractor>();
            
            services.AddTransient<ProjectDataImporter>();

            //services.AddTransient<ProjectSeeder>();
            
            //services.AddSingleton<IHostedService, TimedProjectFetch>();
            
            services.AddTransient<ExamineSearcher>();

            services.AddTransient<ProjectSearcher>();
            
            services.AddTransient<NewsSearcher>();

            services.AddTransient<StorySearcher>();
            
            services.AddTransient<PatronSearchers>();
            
            services.AddTransient<DonationSearcher>();
            
            services.AddTransient<ContentPageSearcher>();
            
            services.AddTransient<BoardSearcher>();
            
            services.AddTransient<ApplicationSearcher>();
            
            services.AddTransient<GeneralSearcher>();

            services.AddTransient<NewTagsService>();

            services.AddTransient<StatusService>();

            services.AddTransient<RegionService>();

            services.AddTransient<ProjectService>();

            services.AddTransient<FocusAreasService>();
        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web hosting environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseUmbraco()
                .WithMiddleware(u =>
                {
                    u.UseBackOffice();
                    u.UseWebsite();
                })
                .WithEndpoints(u =>
                {
                    u.UseInstallerEndpoints();
                    u.UseBackOfficeEndpoints();
                    u.UseWebsiteEndpoints();
                });
            
            //var seeder = services.GetRequiredService<ProjectSeeder>();

        }
    }

