using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MadWorldNL.Umiko;

public class InfrastructureDependencyTests : BaseArchitectureTests
{
    [Fact]
    public void Postgresql_ShouldNotDependOnFunctionsOrControllers()
    {
        IArchRule rule = Types().That().Are(PostgresqlLayer).Should()
            .NotDependOnAny(FunctionsLayer)
            .AndShould().NotDependOnAny(ApiLayer)
            .AndShould().NotDependOnAny(ApiContractsLayer)
            .AndShould().NotDependOnAny(BusLayer)
            .AndShould().NotDependOnAny(WebAdministratorsLayer)
            .AndShould().NotDependOnAny(WebUsersLayer);

        rule.Check(Architecture);
    }

    [Fact]
    public void RabbitMQ_ShouldNotDependOnFunctionsOrControllers()
    {
        IArchRule rule = Types().That().Are(RabbitMqLayer).Should()
            .NotDependOnAny(FunctionsLayer)
            .AndShould().NotDependOnAny(ApiLayer)
            .AndShould().NotDependOnAny(ApiContractsLayer)
            .AndShould().NotDependOnAny(BusLayer)
            .AndShould().NotDependOnAny(WebAdministratorsLayer)
            .AndShould().NotDependOnAny(WebUsersLayer);

        rule.Check(Architecture);
    }
}
