Feature: Entity

Scenario: Two entities with the same ID are equal
    Given I have an entity with ID 1
    And I have another entity with ID 1
    Then the entities should be equal

Scenario: Two entities with different IDs are not equal
    Given I have an entity with ID 1
    And I have another entity with ID 2
    Then the entities should not be equal

Scenario: Entity is equal to itself
    Given I have an entity with ID 1
    Then the entity should equal itself

Scenario: Entity is not equal to null
    Given I have an entity with ID 1
    Then the entity should not equal null

Scenario: Entities with the same ID have the same hash code
    Given I have an entity with ID 1
    And I have another entity with ID 1
    Then the entities should have the same hash code

Scenario: Equal entities satisfy the == operator
    Given I have an entity with ID 1
    And I have another entity with ID 1
    Then the entity == operator should return true

Scenario: Unequal entities satisfy the != operator
    Given I have an entity with ID 1
    And I have another entity with ID 2
    Then the entity != operator should return true