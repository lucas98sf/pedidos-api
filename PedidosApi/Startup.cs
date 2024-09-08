using MongoDB.Driver;
using PedidosApi.Services;

namespace PedidosApi
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMongoClient>(sp =>
            {
                var connectionString = Configuration.GetConnectionString("MongoDb");
                return new MongoClient(connectionString);
            });

            services.AddSingleton<PedidoService>();
            services.AddSingleton<RabbitMQConsumer>();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var rabbitMqConsumer = app.ApplicationServices.GetRequiredService<RabbitMQConsumer>();
            rabbitMqConsumer.Start();
        }
    }
}