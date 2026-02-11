Feature: Infrastructure Dependency Rules

Scenario: Postgresql should not depend on Functions or Controllers
    Given the architecture is loaded
    Then the Postgresql layer should not depend on the Functions layer
    And the Postgresql layer should not depend on the Api layer
    And the Postgresql layer should not depend on the Api Contracts layer
    And the Postgresql layer should not depend on the Bus layer
    And the Postgresql layer should not depend on the Web Administrators layer
    And the Postgresql layer should not depend on the Web Users layer

Scenario: RabbitMQ should not depend on Functions or Controllers
    Given the architecture is loaded
    Then the RabbitMQ layer should not depend on the Functions layer
    And the RabbitMQ layer should not depend on the Api layer
    And the RabbitMQ layer should not depend on the Api Contracts layer
    And the RabbitMQ layer should not depend on the Bus layer
    And the RabbitMQ layer should not depend on the Web Administrators layer
    And the RabbitMQ layer should not depend on the Web Users layer
