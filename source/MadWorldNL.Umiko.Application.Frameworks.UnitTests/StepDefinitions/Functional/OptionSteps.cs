namespace MadWorldNL.Umiko.StepDefinitions.Functional;

[Binding]
[Scope(Feature = "Option Pattern")]
public class OptionSteps
{
    private Option<string>? _option;
    private string? _result;

    [Given("I have a Some option with value {string}")]
    public void GivenIHaveASomeOptionWithValue(string value)
    {
        _option = Option<string>.Some(value);
    }

    [Given("I have a None string option")]
    public void GivenIHaveANoneStringOption()
    {
        _option = Option<string>.None();
    }

    [When("I match the option with some returning {string} and none returning {string}")]
    public void WhenIMatchTheOptionWithSomeReturningAndNoneReturning(string someResult, string noneResult)
    {
        _option.ShouldNotBeNull();
        _result = _option!.Match(
            some: _ => someResult,
            none: () => noneResult);
    }

    [When("I match the option mapping the value to the result")]
    public void WhenIMatchTheOptionMappingTheValueToTheResult()
    {
        _option.ShouldNotBeNull();
        _result = _option!.Match(
            some: v => v,
            none: () => string.Empty);
    }

    [Then("the result should be {string}")]
    public void ThenTheResultShouldBe(string expected)
    {
        _result.ShouldBe(expected);
    }

    [Then("the option should be of type Some")]
    public void ThenTheOptionShouldBeOfTypeSome()
    {
        _option.ShouldBeOfType<Some<string>>();
    }

    [Then("the option should be of type None")]
    public void ThenTheOptionShouldBeOfTypeNone()
    {
        _option.ShouldBeOfType<None<string>>();
    }
}