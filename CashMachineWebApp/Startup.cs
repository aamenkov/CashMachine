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
using System.Linq;
using System.Threading.Tasks;

namespace CashMachineWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // После запуска docker compose создается частная сеть для контейнеров.
        // Контейнеры в ней могут обращаться по имени сервиса и порту. Для каждого был выделен внешний порт(5000 и 5001).
        // 1. Пользователь обращается к домашней странице, в нашем случае это localhost и 80й порт по умолчанию.
        // 2. 80й порт прослушивает контейнер, проваливаемся в него и попадаем на веб-сервер.
        // Он видит обращение, которое происходит на стартовый url и перенаправляет на контейнер client:3000.(реакт всегда на 3000м)
        // 3. От клиента получает страницу и отправляет обратно пользователю.
        // 4. На странице когда нажали на кнопку выполняется запрос на указанный url, например GET: "api/users", 
        // обращение снова происходит на localhost, 80й порт, проваливаемся опять в контейнер к веб-серверу.
        // 5. Веб-сервер видит что url начинается с api и перенапраляет на контейнер backend с портом 80,
        // получает от него данные и так же отдавет пользователю
        // 6. Клиент получив данные от сервера, перерисовывает страницу

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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
