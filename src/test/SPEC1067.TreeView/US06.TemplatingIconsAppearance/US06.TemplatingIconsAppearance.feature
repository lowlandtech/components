@SPEC:1067
@US:06
Feature: SPEC1067.US06 - TreeView Templating Icons and Appearance
  As an application author
  I want TreeView to support item templates, body content, icons, end content, density, hover, sizing, and class customization
  So that it can match MudBlazor template/icon examples and Radzen item render customization

  Background:
    Given the component library LowlandTech.Components.TreeView
    And every rendered node has stable slots for toggle, checkbox, icon, label, body, end content, and children

  # STATE
  # interface TreeViewAppearanceState {
  #   variant: 'default' | 'dense' | 'hover' | 'readonly'
  #   width: string | null
  #   height: string | null
  #   maxHeight: string | null
  #   iconByKey: Record<string, string | null>
  #   expandedIconByKey: Record<string, string | null>
  #   classBySlot: Record<string, string>
  #   renderedTemplateByKey: Record<string, string>
  # }

  # FORM
  # interface TreeViewAppearanceForm {
  #   templateName: string
  #   icon: string | null
  #   expandedIcon: string | null
  #   endText: string | null
  #   cssClass: string | null
  # }

  # REPRESENTATION
  # interface TreeAppearanceNodeRepresentation {
  #   key: string
  #   text: string
  #   icon: string | null
  #   expandedIcon: string | null
  #   endText: string | null
  #   templateName: string | null
  #   contentAttributes: Record<string, string>
  # }

  # ACTIONS
  # type TreeViewAppearanceAction =
  #   | { type: 'RenderDefaultItem'; key: string }
  #   | { type: 'RenderItemTemplate'; key: string; templateName: string }
  #   | { type: 'RenderBodyContent'; key: string; templateName: string }
  #   | { type: 'SetIcon'; key: string; icon: string | null; expandedIcon: string | null }
  #   | { type: 'ApplyAppearanceOptions'; dense: boolean; hover: boolean; width: string | null; height: string | null }
  #   | { type: 'ApplyItemRenderAttributes'; key: string; attributes: Record<string, string> }

  # REDUCER TRANSITIONS
  # RenderDefaultItem -> label and optional end content rendered from node data
  # RenderItemTemplate -> full item template controls item content
  # RenderBodyContent -> body slot replaces default text/end/icon layout only
  # SetIcon -> iconByKey and expandedIconByKey updated
  # ApplyAppearanceOptions -> root style and variant state updated
  # ApplyItemRenderAttributes -> custom attributes merged onto node content

  # EFFECTS
  # on ApplyItemRenderAttributes -> call item render callback before attributes are emitted
  # on RenderItemTemplate -> no selection or expansion side effect

  # EVENTS
  # type TreeViewAppearanceEvent = 'ItemRendered' | 'TemplateRendered' | 'AppearanceChanged'

  @state:TreeViewAppearanceState
  @SC:01
  Scenario: SC01 - Default item layout exposes stable slots
    @precondition:DefaultNodeAvailable
    Given node "All Mail" has text and icon metadata
    @action:RenderDefaultItem
    @param:key="All Mail"
    When TreeView renders the node
    @postcondition:DefaultSlotsRendered
    Then the node should render a content slot @UAC:01
    And the node should render a label slot containing "All Mail" @UAC:02
    And the node should render an icon slot when an icon is configured @UAC:03
    And leaf nodes should reserve alignment for the missing toggle when alignment is enabled @UAC:04
    @postevent:ItemRendered
    And ItemRendered should include key "All Mail" @UAC:05

  @state:TreeViewAppearanceState
  @SC:02
  Scenario: SC02 - Expanded icon replaces collapsed icon for open branches
    @precondition:IconPairConfigured
    Given node "Categories" has icon "folder" and expanded icon "folder-open"
    And node "Categories" is expanded
    @action:SetIcon
    @param:key="Categories"
    @param:icon="folder"
    @param:expandedIcon="folder-open"
    When TreeView renders the expanded node
    @postcondition:ExpandedIconRendered
    Then the icon slot should render "folder-open" @UAC:06
    And collapsing the node should render "folder" @UAC:07

  @state:TreeViewAppearanceState
  @SC:03
  Scenario: SC03 - Item template can own the entire node surface
    @precondition:ItemTemplateConfigured
    Given node "Product" uses item template "ProductTreeItem"
    @action:RenderItemTemplate
    @param:key="Product"
    @param:templateName="ProductTreeItem"
    When TreeView renders the node
    @postcondition:TemplateOwnsSurface
    Then renderedTemplateByKey for "Product" should be "ProductTreeItem" @UAC:08
    And default text and end text should not be duplicated outside the template @UAC:09
    And selection and expansion metadata should still be available to the template @UAC:10
    @postevent:TemplateRendered
    And TemplateRendered should include template "ProductTreeItem" @UAC:11

  @state:TreeViewAppearanceState
  @SC:04
  Scenario: SC04 - Body content customizes the center of the row while preserving tree chrome
    @precondition:BodyContentConfigured
    Given node "src" uses body content template "ExplorerBody"
    @action:RenderBodyContent
    @param:key="src"
    @param:templateName="ExplorerBody"
    When TreeView renders the node body
    @postcondition:TreeChromePreserved
    Then the toggle slot should remain outside the body content @UAC:12
    And the checkbox slot should remain outside the body content when checkboxes are enabled @UAC:13
    And the body content should receive the node context @UAC:14

  @state:TreeViewAppearanceState
  @SC:05
  Scenario: SC05 - Dense hover and sizing options apply to the root and items
    @precondition:AppearanceOptionsProvided
    Given dense and hover options are enabled
    And width is "100%" and maxHeight is "420px"
    @action:ApplyAppearanceOptions
    @param:dense=true
    @param:hover=true
    @param:width="100%"
    @param:height=null
    When TreeView renders with appearance options
    @postcondition:AppearanceApplied
    Then the root should include dense styling @UAC:15
    And hover styling should only apply to interactive items @UAC:16
    And the root style should include width "100%" @UAC:17
    And maxHeight should make the tree body vertically scrollable @UAC:18
    @postevent:AppearanceChanged
    And AppearanceChanged should include variant "dense" @UAC:19

  @state:TreeViewAppearanceState
  @SC:06
  Scenario: SC06 - ItemRender can add per-node attributes and classes
    @precondition:ItemRenderConfigured
    Given ItemRender adds data-kind "folder" to folder nodes
    @action:ApplyItemRenderAttributes
    @param:key="Categories"
    @param:attributes={"data-kind":"folder"}
    When TreeView renders node "Categories"
    @postcondition:CustomAttributesRendered
    Then the node content should include data-kind "folder" @UAC:20
    And custom content, icon, and label classes should merge with component classes @UAC:21
    And custom attributes should not replace required aria attributes @UAC:22
    @postevent:ItemRendered
    And ItemRendered should include custom attribute count 1 @UAC:23

