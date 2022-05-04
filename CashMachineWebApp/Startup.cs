using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CashMachineWebApp.Context;
using Microsoft.EntityFrameworkCore;

namespace CashMachineWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // ����� ������� docker compose ��������� ������� ���� ��� �����������.
        // ���������� � ��� ����� ���������� �� ����� ������� � �����. ��� ������� ��� ������� ������� ����(5000 � 5001).
        // 1. ������������ ���������� � �������� ��������, � ����� ������ ��� localhost � 80� ���� �� ���������.
        // 2. 80� ���� ������������ ���������, ������������� � ���� � �������� �� ���-������.
        // �� ����� ���������, ������� ���������� �� ��������� url � �������������� �� ��������� client:3000.(����� ������ �� 3000�)
        // 3. �� ������� �������� �������� � ���������� ������� ������������.
        // 4. �� �������� ����� ������ �� ������ ����������� ������ �� ��������� url, �������� GET: "api/users", 
        // ��������� ����� ���������� �� localhost, 80� ����, ������������� ����� � ��������� � ���-�������.
        // 5. ���-������ ����� ��� url ���������� � api � �������������� �� ��������� backend � ������ 80,
        // �������� �� ���� ������ � ��� �� ������� ������������
        // 6. ������ ������� ������ �� �������, �������������� ��������

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<CRUDContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CashMachine_Db")));
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.WithOrigins("http://localhost:5001");
                    builder.WithOrigins("http://localhost:3000");
                    builder.WithOrigins("http://localhost");
                }));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CashMachineWebApp", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(string.Format(@"{0}\CashMachineWebApp.XML", System.AppDomain.CurrentDomain.BaseDirectory));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CashMachineWebApp v1"));
            }

            app.UseRouting();

            app.UseCors("CorsPolicy");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
