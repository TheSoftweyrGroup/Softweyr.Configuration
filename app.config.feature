Feature: Access values from an applications configuration file.

    Scenario: Get a value from the AppSettings section of a configuration file.
        Given you've specified to look in the application configuration file for configuration values
            And you've set up a configuration definition
            And you're configuration definition defines an AppSettings configuration property with the name "MyConfigurationProperty"
            And you've added a key-value pair to the appSettings section of the application configuration file with the name "MyConfigurationProperty" and a value of "Hello World"
	When you've got the configuration
	Then the configuration property "MyConfigurationProperty" will have the value "Hello World".