using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MadWorldNL.Umiko;

public class DomainDependencyTests : BaseArchitectureTests
{
    [Fact]
    public void Domain_ShouldNotDependOnOtherProjects()
    {
        IArchRule rule = Types().That().Are(DomainLayer).Should()
            .NotDependOnAny(FunctionsLayer)
            .AndShould().NotDependOnAny(PostgresqlLayer)
            .AndShould().NotDependOnAny(RabbitMqLayer)
            .AndShould().NotDependOnAny(ApiLayer)
            .AndShould().NotDependOnAny(ApiContractsLayer)
            .AndShould().NotDependOnAny(BusLayer)
            .AndShould().NotDependOnAny(WebAdministratorsLayer)
            .AndShould().NotDependOnAny(WebUsersLayer);
        
        rule.Check(Architecture);
    }
}