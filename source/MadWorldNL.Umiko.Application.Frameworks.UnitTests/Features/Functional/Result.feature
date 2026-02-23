Feature: Result Pattern

Scenario: Success result resolves to the success branch
    Given I have a Success result with value "hello"
    When I match the result with success returning "ok" and failure returning "error"
    Then the result should be "ok"

Scenario: Failure result resolves to the failure branch
    Given I have a Failure result with message "something went wrong"
    When I match the result with success returning "ok" and failure returning "error"
    Then the result should be "error"

Scenario: Success result passes the value to the success branch
    Given I have a Success result with value "hello"
    When I match the result mapping the value to the result
    Then the result should be "hello"

Scenario: Failure result passes the exception message to the failure branch
    Given I have a Failure result with message "something went wrong"
    When I match the result mapping the exception message to the result
    Then the result should be "something went wrong"

Scenario: Success result is of type Success
    Given I have a Success result with value "hello"
    Then the result should be of type Success

Scenario: Failure result is of type Failure
    Given I have a Failure result with message "something went wrong"
    Then the result should be of type Failure