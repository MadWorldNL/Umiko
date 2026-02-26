Feature: Api CurriculaVitae Endpoints

Scenario: Returns Accepted with an id when creating a valid curriculum vitae
    Given the Api service is healthy
    When I create a curriculum vitae with first name "John" and last name "Doe" on the Api service
    Then the response status code should be Accepted
    And the curriculum vitae should eventually be retrievable on the Api service

Scenario: Returns NotFound for unknown id
    Given the Api service is healthy
    When I get a curriculum vitae with an unknown id on the Api service
    Then the response status code should be NotFound