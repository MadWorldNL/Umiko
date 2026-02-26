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

Scenario: Applying an event mutates state
    Given I have a new aggregate root with ID 1
    When I add a domain event
    Then the last applied date should be set

Scenario: Applying an event adds it to domain events
    Given I have a new aggregate root with ID 1
    When I add a domain event
    Then the domain event count should be 1
    And the last applied date should be set

Scenario: Reconstituting from history mutates state
    Given I have a new aggregate root with ID 1
    When I reconstitute from 1 domain event
    Then the last applied date should be set

Scenario: Reconstituting from history does not add domain events
    Given I have a new aggregate root with ID 1
    When I reconstitute from 1 domain event
    Then it should have no domain events