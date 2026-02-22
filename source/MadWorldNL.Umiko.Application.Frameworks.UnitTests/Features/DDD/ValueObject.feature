Feature: Value Object

Scenario: Two value objects with the same components are equal
    Given I have a value object with value "hello"
    And I have another value object with value "hello"
    Then the value objects should be equal

Scenario: Two value objects with different components are not equal
    Given I have a value object with value "hello"
    And I have another value object with value "world"
    Then the value objects should not be equal

Scenario: Value object is equal to itself
    Given I have a value object with value "hello"
    Then the value object should equal itself

Scenario: Value object is not equal to null
    Given I have a value object with value "hello"
    Then the value object should not equal null

Scenario: Value objects with the same components have the same hash code
    Given I have a value object with value "hello"
    And I have another value object with value "hello"
    Then the value objects should have the same hash code

Scenario: Value objects of different types are not equal even with the same components
    Given I have a value object with value "hello"
    And I have a different type value object with value "hello"
    Then the value objects should not be equal

Scenario: Equal value objects satisfy the == operator
    Given I have a value object with value "hello"
    And I have another value object with value "hello"
    Then the value object == operator should return true

Scenario: Unequal value objects satisfy the != operator
    Given I have a value object with value "hello"
    And I have another value object with value "world"
    Then the value object != operator should return true