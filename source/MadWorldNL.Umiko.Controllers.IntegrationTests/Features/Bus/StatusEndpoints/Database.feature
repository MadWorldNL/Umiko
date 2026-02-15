Feature: Bus Database Endpoint

Scenario: Database endpoint returns connected status
    Given the Bus service is healthy
    When I send a GET request to "/Status/Database" on the Bus service
    Then the response should contain "isConnected" with value "true"