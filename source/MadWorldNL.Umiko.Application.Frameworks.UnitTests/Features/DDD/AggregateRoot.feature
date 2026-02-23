Feature: Aggregate Root

Scenario: New aggregate root has no domain events
    Given I have a new aggregate root with ID 1
    Then it should have no domain events

Scenario: Adding a domain event stores it
    Given I have a new aggregate root with ID 1
    When I add a domain event
    Then the domain event count should be 1

Scenario: Adding multiple domain events stores them all
    Given I have a new aggregate root with ID 1
    When I add 3 domain events
    Then the domain event count should be 3

Scenario: Clearing domain events removes all events
    Given I have a new aggregate root with ID 1
    When I add a domain event
    And I clear the domain events
    Then it should have no domain events