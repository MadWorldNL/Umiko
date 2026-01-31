using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using MadWorldNL.Umiko.Web;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MadWorldNL.Umiko;

public abstract class BaseArchitectureTests
{
    protected static readonly Architecture Architecture =
        new ArchLoader()
            .LoadAssembly(typeof(IDomainMarker).Assembly)
            .LoadAssembly(typeof(IFunctionsMarker).Assembly)
            .LoadAssembly(typeof(IPostgresqlMarker).Assembly)
            .LoadAssembly(typeof(IRabbitMqMarker).Assembly)
            .LoadAssembly(typeof(IApiMarker).Assembly)
            .LoadAssembly(typeof(IApiContractsMarker).Assembly)
            .LoadAssembly(typeof(IBusMarker).Assembly)
            .LoadAssembly(typeof(IWebAdministratorsMarker).Assembly)
            .LoadAssembly(typeof(IWebUsersMarker).Assembly)
            .Build();

    protected readonly IObjectProvider<IType> DomainLayer =
        Types().That().ResideInAssembly(typeof(IDomainMarker).Assembly).As("Domain Layer");

    protected readonly IObjectProvider<IType> FunctionsLayer =
        Types().That().ResideInAssembly(typeof(IFunctionsMarker).Assembly).As("Functions Layer");

    protected readonly IObjectProvider<IType> PostgresqlLayer =
        Types().That().ResideInAssembly(typeof(IPostgresqlMarker).Assembly).As("Postgresql Layer");

    protected readonly IObjectProvider<IType> RabbitMqLayer =
        Types().That().ResideInAssembly(typeof(IRabbitMqMarker).Assembly).As("RabbitMQ Layer");

    protected readonly IObjectProvider<IType> ApiLayer =
        Types().That().ResideInAssembly(typeof(IApiMarker).Assembly).As("Api Layer");

    protected readonly IObjectProvider<IType> ApiContractsLayer =
        Types().That().ResideInAssembly(typeof(IApiContractsMarker).Assembly).As("Api Contracts Layer");

    protected readonly IObjectProvider<IType> BusLayer =
        Types().That().ResideInAssembly(typeof(IBusMarker).Assembly).As("Bus Layer");

    protected readonly IObjectProvider<IType> WebAdministratorsLayer =
        Types().That().ResideInAssembly(typeof(IWebAdministratorsMarker).Assembly).As("Web Administrators Layer");

    protected readonly IObjectProvider<IType> WebUsersLayer =
        Types().That().ResideInAssembly(typeof(IWebUsersMarker).Assembly).As("Web Users Layer");
}