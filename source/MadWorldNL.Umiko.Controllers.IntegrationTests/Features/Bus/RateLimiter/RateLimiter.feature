Feature: Bus Rate Limiter

Scenario: Requests within the rate limit succeed
    Given the Bus service is healthy
    When I send 5 GET requests to "/Status/Ping" on the Bus service
    Then all response status codes should be OK

Scenario: Requests exceeding the rate limit are rejected
    Given the Bus service is healthy
    When I send 6 GET requests to "/Status/Ping" on the Bus service
    Then the first 5 response status codes should be OK
    And the last response status code should be TooManyRequests

Scenario: Health check endpoint is excluded from rate limiting
    Given the Bus service is healthy
    When I send 6 GET requests to "/health" on the Bus service
    Then all response status codes should be OK
