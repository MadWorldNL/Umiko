namespace MadWorldNL.Umiko.StepDefinitions.Functional;

[Binding]
[Scope(Feature = "Result Pattern")]
public class ResultSteps
{
    private Result<string>? _result;
    private string? _matchResult;

    [Given("I have a Success result with value {string}")]
    public void GivenIHaveASuccessResultWithValue(string value)
    {
        _result = Result<string>.Success(value);
    }

    [Given("I have a Failure result with message {string}")]
    public void GivenIHaveAFailureResultWithMessage(string message)
    {
        _result = Result<string>.Failure(new Exception(message));
    }

    [When("I match the result with success returning {string} and failure returning {string}")]
    public void WhenIMatchTheResultWithSuccessReturningAndFailureReturning(string successResult, string failureResult)
    {
        _result.ShouldNotBeNull();
        _matchResult = _result!.Match(
            success: _ => successResult,
            failure: _ => failureResult);
    }

    [When("I match the result mapping the value to the result")]
    public void WhenIMatchTheResultMappingTheValueToTheResult()
    {
        _result.ShouldNotBeNull();
        _matchResult = _result!.Match(
            success: v => v,
            failure: _ => string.Empty);
    }

    [When("I match the result mapping the exception message to the result")]
    public void WhenIMatchTheResultMappingTheExceptionMessageToTheResult()
    {
        _result.ShouldNotBeNull();
        _matchResult = _result!.Match(
            success: _ => string.Empty,
            failure: ex => ex.Message);
    }

    [Then("the result should be {string}")]
    public void ThenTheResultShouldBe(string expected)
    {
        _matchResult.ShouldBe(expected);
    }

    [Then("the result should be of type Success")]
    public void ThenTheResultShouldBeOfTypeSuccess()
    {
        _result.ShouldBeOfType<Success<string>>();
    }

    [Then("the result should be of type Failure")]
    public void ThenTheResultShouldBeOfTypeFailure()
    {
        _result.ShouldBeOfType<Failure<string>>();
    }
}