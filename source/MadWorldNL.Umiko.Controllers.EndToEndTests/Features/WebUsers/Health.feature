Feature: Web Users Health

Scenario: Health page returns successfully
    Given the Web-Users service is healthy
    When I navigate to the health page on Web-Users
    Then the page should return status code 200

Scenario: Home page loads successfully
    Given the Web-Users service is healthy
    When I navigate to the home page on Web-Users
    Then the page should return status code 200

Scenario: Blazor app initializes without errors
    Given the Web-Users service is healthy
    When I navigate to the home page on Web-Users and wait for it to load
    Then there should be no console errors
    And the heading should be "Hello, world!"