Feature: Option Pattern

Scenario: Some option resolves to the some branch
    Given I have a Some option with value "hello"
    When I match the option with some returning "found" and none returning "missing"
    Then the result should be "found"

Scenario: None option resolves to the none branch
    Given I have a None string option
    When I match the option with some returning "found" and none returning "missing"
    Then the result should be "missing"

Scenario: Some option passes the value to the some branch
    Given I have a Some option with value "hello"
    When I match the option mapping the value to the result
    Then the result should be "hello"

Scenario: Some option is of type Some
    Given I have a Some option with value "hello"
    Then the option should be of type Some

Scenario: None option is of type None
    Given I have a None string option
    Then the option should be of type None