Feature: Bus MessageBus Endpoint

Scenario: MessageBus endpoint returns connected status
    Given the Bus service is healthy
    When I send a GET request to "/Status/MessageBus" on the Bus service
    Then the response should contain "isConnected" with value "true"