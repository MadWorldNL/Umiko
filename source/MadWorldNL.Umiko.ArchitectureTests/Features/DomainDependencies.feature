Feature: Domain Dependency Rules

Scenario: Domain layer should only depend on Frameworks
    Given the architecture is loaded
    Then the Domain layer should not depend on the Functions layer
    And the Domain layer should not depend on the Postgresql layer
    And the Domain layer should not depend on the RabbitMQ layer
    And the Domain layer should not depend on the Api layer
    And the Domain layer should not depend on the Api Contracts layer
    And the Domain layer should not depend on the Bus layer
    And the Domain layer should not depend on the Web Administrators layer
    And the Domain layer should not depend on the Web Users layer
