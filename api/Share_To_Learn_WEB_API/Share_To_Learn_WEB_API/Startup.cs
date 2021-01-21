using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neo4jClient;
using Share_To_Learn_WEB_API.RedisConnection;
using Share_To_Learn_WEB_API.Services;
using Share_To_Learn_WEB_API.HubConfig;
using Microsoft.AspNetCore.SignalR;
using Share_To_Learn_WEB_API.Services.RepositoryContracts;
using Share_To_Learn_WEB_API.Services.RepositoryServices;

namespace Share_To_Learn_WEB_API
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
            services.AddControllers();
            var neo4jClient = new GraphClient(new Uri("http://localhost:7474/"), "neo4j", "sharetolearn");
            neo4jClient.ConnectAsync();
            services.AddSingleton<IGraphClient>(neo4jClient);
            services.AddSingleton<IRedisConnectionBuilder, RedisConnectionBuilder>();
            services.AddScoped<ISTLRepository, STLRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            }).AddMvcOptions(options =>
            {
                options.EnableEndpointRouting = false;
            });
            services.AddCors(options =>
            {
                options.AddPolicy("CORS", builder =>
                {
                    builder.AllowAnyHeader()
                   .AllowAnyMethod()
                   .SetIsOriginAllowed((host) => true)
                   .AllowCredentials();
                });
            });
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

         
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            

            app.UseRouting();


            app.UseCors("CORS");

            app.UseAuthorization();

   
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MessageHub>("chat");
            });


        }
    }
}
