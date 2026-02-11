Feature: Api Ping Endpoint

Scenario: Ping endpoint returns OK status code
    Given the Api service is healthy
    When I send a GET request to "/Status/Ping" on the Api service
    Then the response status code should be OK