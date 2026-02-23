Feature: Api MessageBus Endpoint

Scenario: MessageBus endpoint returns connected status
    Given the Api service is healthy
    When I send a GET request to "/Status/MessageBus" on the Api service
    Then the response should contain "isConnected" with value "true"