using JasperFx;
using JasperFx.CodeGeneration;
using MadWorldNL.Umiko.CurriculaVitae;
using MadWorldNL.Umiko.Events;
using Marten;

namespace MadWorldNL.Umiko.Configurations;

public static class WebApplicationBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public void AddApplication()
        {
            builder.Services.AddScoped<CreateCurriculumVitaeUseCase>();
        }

        public void AddDatabase()
        {
            builder.Services.AddScoped<IEventsContext, EventsContext>();

            builder.ConfigureDatabase();
        }

        private void ConfigureDatabase()
        {
            var connectionString = builder.Configuration.GetConnectionString("umikodb");
        
            var options = new StoreOptions();
            options.Connection(connectionString!);
            options.Events.DatabaseSchemaName = "events";
            options.Events.AddEventType<NewCurriculumVitaeEvent>();

            builder.Services
                .AddMarten(options)
                .ApplyAllDatabaseChangesOnStartup();
    
            builder.Services.CritterStackDefaults(x =>
            {
                x.Production.GeneratedCodeMode = TypeLoadMode.Static;
                x.Production.ResourceAutoCreate = AutoCreate.None;
    
                x.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;
                x.Development.ResourceAutoCreate = AutoCreate.CreateOrUpdate;
            });
        }
    }
}