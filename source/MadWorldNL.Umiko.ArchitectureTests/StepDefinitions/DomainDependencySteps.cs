using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MadWorldNL.Umiko.StepDefinitions;

[Binding]
[Scope(Feature = "Domain Dependency Rules")]
public sealed class DomainDependencySteps : BaseArchitectureTests
{
    private readonly Dictionary<string, IObjectProvider<IType>> _layers;

    public DomainDependencySteps()
    {
        _layers = new Dictionary<string, IObjectProvider<IType>>
        {
            ["Frameworks"] = FrameworksLayer,
            ["Domain"] = DomainLayer,
            ["Functions"] = FunctionsLayer,
            ["Postgresql"] = PostgresqlLayer,
            ["RabbitMQ"] = RabbitMqLayer,
            ["Api"] = ApiLayer,
            ["Api Contracts"] = ApiContractsLayer,
            ["Bus"] = BusLayer,
            ["Web Administrators"] = WebAdministratorsLayer,
            ["Web Users"] = WebUsersLayer
        };
    }

    [Given("the architecture is loaded")]
    public static void GivenTheArchitectureIsLoaded()
    {
        // Architecture is loaded statically in BaseArchitectureTests
    }

    [Then("the {} layer should not depend on the {} layer")]
    public void ThenTheLayerShouldNotDependOnTheLayer(string sourceLayerName, string targetLayerName)
    {
        var sourceLayer = _layers[sourceLayerName];
        var targetLayer = _layers[targetLayerName];

        IArchRule rule = Types().That().Are(sourceLayer).Should()
            .NotDependOnAny(targetLayer);

        rule.Check(Architecture);
    }
}
