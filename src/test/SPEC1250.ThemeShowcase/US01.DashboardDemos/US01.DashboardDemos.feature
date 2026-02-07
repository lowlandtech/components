@US01
Feature: US01 - Dashboard Demo Port
    As a Blazor developer
    I want the Vite dashboard demos ported to Blazor
    So that the theme showcases core layout and data presentation components

    Background:
        Given I have a Blazor theme project at "src/theme"
        And the Vite sample exists at "samples/vite"
        And component classes are defined in "src/lib/wwwroot/css/input.css"

    @SC01
    Scenario: Port demo1 light sidebar dashboard - Main layout structure
        Given the Vite demo1 light sidebar page exists at "samples/vite/src/pages/dashboards/demo1/light-sidebar"
        When I build the Blazor demo1 light sidebar page
        #UAC01
        Then it should render a responsive grid with gap-5 lg:gap-7.5 spacing
        #UAC02
        And it should have three main row sections in the grid layout
        #UAC03
        And the first row should use lg:grid-cols-3 with col-span-1 for stats and col-span-2 for callout
        #UAC04
        And the second row should use lg:grid-cols-3 with col-span-1 for highlights and col-span-2 for chart
        #UAC05
        And the third row should use lg:grid-cols-3 with col-span-1 for team meeting and col-span-2 for teams table

    @SC01.1
    Scenario: Port demo1 light sidebar dashboard - ChannelStats component
        Given the Vite demo1 ChannelStats component exists
        When I render the ChannelStats section
        #UAC06
        Then it should render multiple stat cards in a grid-cols-2 layout
        #UAC07
        And each stat card should display a numeric value with trend indicator
        #UAC08
        And each stat card should use the card component class
        #UAC09
        And stat cards should show channel names like "LinkedIn", "Twitter", "Facebook", "Instagram"

    @SC01.2
    Scenario: Port demo1 light sidebar dashboard - EntryCallout component
        Given the Vite demo1 EntryCallout component exists
        When I render the EntryCallout section
        #UAC10
        Then it should render as a card with h-full class for full height
        #UAC11
        And it should display a headline message or call-to-action
        #UAC12
        And it should include an illustrative image or icon
        #UAC13
        And it should have an action button using btn component class

    @SC01.3
    Scenario: Port demo1 light sidebar dashboard - Highlights component
        Given the Vite demo1 Highlights component exists
        When I render the Highlights section with limit=3
        #UAC14
        Then it should render a card container with highlights
        #UAC15
        And it should display exactly 3 highlight items when limit is set to 3
        #UAC16
        And each highlight should show an icon, title, and description
        #UAC17
        And highlights should use badge classes for status indicators

    @SC01.4
    Scenario: Port demo1 light sidebar dashboard - EarningsChart component
        Given the Vite demo1 EarningsChart component exists
        When I render the EarningsChart section
        #UAC18
        Then it should render a card container for the chart
        #UAC19
        And it should display a chart title and time period selector
        #UAC20
        And it should render a chart visualization or placeholder with aspect ratio preserved
        #UAC21
        And the chart area should adapt to card dimensions responsively

    @SC01.5
    Scenario: Port demo1 light sidebar dashboard - TeamMeeting component
        Given the Vite demo1 TeamMeeting component exists
        When I render the TeamMeeting section
        #UAC22
        Then it should render a card with meeting information
        #UAC23
        And it should display meeting time and duration
        #UAC24
        And it should show participant avatars or count
        #UAC25
        And it should include a join or view button

    @SC01.6
    Scenario: Port demo1 light sidebar dashboard - Teams component
        Given the Vite demo1 Teams component exists
        When I render the Teams section
        #UAC26
        Then it should render a card container with teams table
        #UAC27
        And it should display table headers with sortable columns
        #UAC28
        And each team row should show team name, members count, and status
        #UAC29
        And table should use responsive table component classes
        #UAC30
        And it should include action buttons per row using btn classes

    @SC02
    Scenario: Port demo1 dark sidebar dashboard
        Given the Vite demo1 dark sidebar page exists at "samples/vite/src/pages/dashboards/demo1/dark-sidebar"
        When I build the Blazor demo1 dark sidebar page
        #UAC31
        Then it should reuse the Demo1LightSidebarContent component
        #UAC32
        And it should wrap content in a dark sidebar layout shell
        #UAC33
        And the sidebar should use dark background semantic colors
        #UAC34
        And it should render a toolbar with breadcrumb navigation
        #UAC35
        And it should include a date range picker control in the toolbar
        #UAC36
        And the toolbar should use toolbar component classes

    @SC03
    Scenario: Port demo2 dashboard - Layout structure
        Given the Vite demo2 dashboard page exists at "samples/vite/src/pages/dashboards/demo2"
        When I build the Blazor demo2 dashboard page
        #UAC37
        Then it should render a responsive grid with gap-5 lg:gap-7.5 spacing
        #UAC38
        And it should have four main row sections in the grid layout
        #UAC39
        And row 1 should use lg:grid-cols-3 with col-span-2 for options grid and col-span-1 for balance
        #UAC40
        And row 2 should use lg:grid-cols-3 with col-span-2 for callout and col-span-1 for report settings
        #UAC41
        And row 3 should use lg:grid-cols-3 with col-span-2 for integrations and col-span-1 for block list
        #UAC42
        And row 4 should use lg:grid-cols-3 with col-span-2 for teams and col-span-1 for manage data

    @SC03.1
    Scenario: Port demo2 dashboard - Options component
        Given the Vite demo2 Options component exists
        When I render the Options section
        #UAC43
        Then it should render multiple option cards in an md:grid-cols-2 layout
        #UAC44
        And each option card should display an icon and label
        #UAC45
        And each option card should be clickable or toggleable
        #UAC46
        And option cards should use card and btn component classes

    @SC03.2
    Scenario: Port demo2 dashboard - MyBalance component
        Given the Vite demo2 MyBalance component exists
        When I render the MyBalance section
        #UAC47
        Then it should render a card with h-full class
        #UAC48
        And it should display current balance amount prominently
        #UAC49
        And it should show balance trend or change indicator
        #UAC50
        And it should include action buttons for transactions

    @SC03.3
    Scenario: Port demo2 dashboard - Reused and new components
        Given the Vite demo2 page reuses EntryCallout and Teams from demo1
        When I render the demo2 dashboard
        #UAC51
        Then it should render EntryCallout with h-full class
        #UAC52
        And it should render ReportSettings component from account/security
        #UAC53
        And it should render Integrations component specific to demo2
        #UAC54
        And it should render BlockList component with custom text prop
        #UAC55
        And it should render ManageData component with h-full class

    @SC04
    Scenario: Port demo3 dashboard - Layout structure
        Given the Vite demo3 dashboard page exists at "samples/vite/src/pages/dashboards/demo3"
        When I build the Blazor demo3 dashboard page
        #UAC56
        Then it should render a responsive grid with gap-5 lg:gap-7.5 spacing
        #UAC57
        And it should have three main row sections in the grid layout
        #UAC58
        And row 1 should use lg:grid-cols-3 with col-span-2 for create team and col-span-1 for highlights
        #UAC59
        And row 2 should use lg:grid-cols-3 with col-span-2 for teams and col-span-1 for block list
        #UAC60
        And row 3 should use lg:grid-cols-3 with col-span-2 for integrations and col-span-1 for manage data

    @SC04.1
    Scenario: Port demo3 dashboard - CreateTeam component
        Given the Vite demo3 CreateTeam component exists from partials/common
        When I render the CreateTeam section
        #UAC61
        Then it should render a card with h-full class
        #UAC62
        And it should display illustration image with dark variant (32-dark.svg)
        #UAC63
        And it should show title "Swift Setup for New Teams"
        #UAC64
        And it should show subtitle with multi-line description
        #UAC65
        And it should include engage button with label "Create Team" and btnColor "mono"
        #UAC66
        And the engage button should link to "/public-profile/teams"

    @SC04.2
    Scenario: Port demo3 dashboard - Reused components
        Given the Vite demo3 page reuses Highlights, Teams, and BlockList
        When I render the demo3 dashboard
        #UAC67
        Then it should render Highlights component without limit prop
        #UAC68
        And it should render Teams component from demo1
        #UAC69
        And it should render BlockList with limit=3 and custom text
        #UAC70
        And it should render Integrations component specific to demo3
        #UAC71
        And it should render ManageData component from demo2

    @SC05
    Scenario: Port demo4 dashboard - Layout structure
        Given the Vite demo4 dashboard page exists at "samples/vite/src/pages/dashboards/demo4"
        When I build the Blazor demo4 dashboard page
        #UAC72
        Then it should render a responsive grid with gap-5 lg:gap-7.5 spacing
        #UAC73
        And it should have two main row sections in the grid layout
        #UAC74
        And row 1 should use lg:grid-cols-3 with col-span-2 for create team and col-span-1 for highlights
        #UAC75
        And row 2 should use lg:grid-cols-3 with col-span-2 for teams and col-span-1 for block list

    @SC05.1
    Scenario: Port demo4 dashboard - CreateTeam variant
        Given the Vite demo4 CreateTeam uses btnColor "primary"
        When I render the demo4 CreateTeam section
        #UAC76
        Then it should render CreateTeam with same structure as demo3
        #UAC77
        And the engage button should use btnColor "primary" instead of "mono"
        #UAC78
        And it should use Fragment instead of JSX fragment syntax

    @SC05.2
    Scenario: Port demo4 dashboard - Reused components
        Given the Vite demo4 page reuses Highlights, Teams, and BlockList
        When I render the demo4 dashboard
        #UAC79
        Then it should render Highlights component without limit prop
        #UAC80
        And it should render Teams component from demo1
        #UAC81
        And it should render BlockList without limit prop and with custom text

    @SC06
    Scenario: Port demo5 dashboard - Layout structure
        Given the Vite demo5 dashboard page exists at "samples/vite/src/pages/dashboards/demo5"
        When I build the Blazor demo5 dashboard page
        #UAC82
        Then it should render a responsive grid with gap-5 lg:gap-7.5 spacing
        #UAC83
        And it should have two main row sections in the grid layout
        #UAC84
        And row 1 should use lg:grid-cols-3 with col-span-2 for team info and col-span-1 for options
        #UAC85
        And row 2 should use lg:grid-cols-1 for teams and FAQ in vertical stack

    @SC06.1
    Scenario: Port demo5 dashboard - TeamInfo component
        Given the Vite demo5 TeamInfo component exists from account/members/team-info
        When I render the TeamInfo section
        #UAC86
        Then it should render team profile information in a card
        #UAC87
        And it should display team name and description
        #UAC88
        And it should show team statistics or member count
        #UAC89
        And it should include team action buttons

    @SC06.2
    Scenario: Port demo5 dashboard - Options and FAQ components
        Given the Vite demo5 page uses Options and Faq components
        When I render the demo5 dashboard
        #UAC90
        Then it should render Options component specific to demo5 in grid-cols-1 layout
        #UAC91
        And it should render Teams component from demo1
        #UAC92
        And it should render Faq component from partials/common
        #UAC93
        And FAQ should display collapsible question/answer pairs
        #UAC94
        And FAQ should use accordion or expansion panel component classes
