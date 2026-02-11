Feature: Bus Ping Endpoint

Scenario: Ping endpoint returns OK status code
    Given the Bus service is healthy
    When I send a GET request to "/Status/Ping" on the Bus service
    Then the response status code should be OK