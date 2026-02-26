Feature: Api GetCurriculumVitae Endpoint

Scenario: Returns NotFound for unknown id
    Given the Api service is healthy
    When I get a curriculum vitae with an unknown id on the Api service
    Then the response status code should be NotFound