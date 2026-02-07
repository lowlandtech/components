    @SC04
    Scenario: Implement Label component
        Given I have a shadcn/ui Label component
        When I port it to a Blazor component
        #UAC13
        Then it should render a <label> element with proper classes
        #UAC14
        And it should merge a Class parameter with generated classes
        #UAC15
        And it should forward unmatched attributes to the underlying element
        #UAC16
        And it should render child content
    @SC04
    Scenario: Implement Label component
        Given I have a shadcn/ui Label component
        When I port it to a Blazor component
        #UAC13
        Then it should render a <label> element with proper classes
        #UAC14
        And it should merge a Class parameter with generated classes
        #UAC15
        And it should forward unmatched attributes to the underlying element
        #UAC16
        And it should render child content
