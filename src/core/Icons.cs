namespace LowlandTech.Core;

/// <summary>
/// Static class containing Lucide icon SVG path data.
/// Use with the Icon component to render icons.
/// </summary>
public static class Icons
{
    // Node type icons
    public const string User = """<path d="M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/>""";
    public const string Workspaces = """<path d="M4 20h16a2 2 0 0 0 2-2V8a2 2 0 0 0-2-2h-7.93a2 2 0 0 1-1.66-.9l-.82-1.2A2 2 0 0 0 7.93 3H4a2 2 0 0 0-2 2v13c0 1.1.9 2 2 2Z"/><path d="M8 10v4"/><path d="M12 10v2"/><path d="M16 10v6"/>""";
    public const string Workspace = """<rect width="7" height="7" x="3" y="3" rx="1"/><rect width="7" height="7" x="14" y="3" rx="1"/><rect width="7" height="7" x="14" y="14" rx="1"/><rect width="7" height="7" x="3" y="14" rx="1"/>""";
    public const string Tables = """<path d="M9 3H5a2 2 0 0 0-2 2v4m6-6h10a2 2 0 0 1 2 2v4M9 3v18m0 0h10a2 2 0 0 0 2-2V9M9 21H5a2 2 0 0 1-2-2V9m0 0h18"/>""";
    public const string Table = """<path d="M12 3v18"/><rect width="18" height="18" x="3" y="3" rx="2"/><path d="M3 9h18"/><path d="M3 15h18"/>""";
    public const string Columns = """<rect width="18" height="18" x="3" y="3" rx="2"/><path d="M9 3v18"/><path d="M15 3v18"/>""";
    public const string Column = """<path d="M10 10v11"/><path d="M14 10v11"/><path d="M18 5v1a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h8a2 2 0 0 1 2 2Z"/><path d="M6 21h12"/>""";
    public const string SessionMessage = """<path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"/>""";
    public const string Workitem = """<path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><path d="m9 11 3 3L22 4"/>""";
    public const string Threads = """<path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"/><path d="M8 10h8"/><path d="M8 14h4"/>""";
    public const string Thread = """<path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"/>""";
    public const string RuleTrace = """<path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><path d="M14 2v6h6"/><path d="m9 15 2 2 4-4"/>""";

    // Page icons
    public const string Pages = """<path d="M15 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7Z"/><path d="M14 2v4a2 2 0 0 0 2 2h4"/><path d="M10 9H8"/><path d="M16 13H8"/><path d="M16 17H8"/>""";
    public const string Page = """<path d="M15 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7Z"/><path d="M14 2v4a2 2 0 0 0 2 2h4"/>""";
    public const string PageContainer = """<rect width="18" height="18" x="3" y="3" rx="2"/><path d="M3 9h18"/><path d="M9 21V9"/>""";
    public const string PageBlock = """<rect width="7" height="7" x="3" y="3" rx="1"/><rect width="7" height="7" x="14" y="3" rx="1"/><rect width="7" height="7" x="3" y="14" rx="1"/><rect width="7" height="7" x="14" y="14" rx="1"/>""";
    public const string PageItem = """<circle cx="12" cy="12" r="1"/><path d="M20.2 20.2c2.04-2.03.02-7.36-4.5-11.9-4.54-4.52-9.87-6.54-11.9-4.5-2.04 2.03-.02 7.36 4.5 11.9 4.54 4.52 9.87 6.54 11.9 4.5Z"/>""";
    public const string PageItemAttribute = """<path d="M12 20h9"/><path d="M16.376 3.622a1 1 0 0 1 3.002 3.002L7.368 18.635a2 2 0 0 1-.855.506l-2.872.838a.5.5 0 0 1-.62-.62l.838-2.872a2 2 0 0 1 .506-.854z"/>""";

    // Pipeline icons
    public const string Pipelines = """<path d="M12 3v18"/><rect width="6" height="4" x="9" y="4" rx="1"/><path d="M5 9h4"/><path d="M15 15h4"/><rect width="6" height="4" x="15" y="13" rx="1"/>""";
    public const string Pipeline = """<path d="M12 3v6"/><path d="M12 15v6"/><rect width="16" height="6" x="4" y="9" rx="1"/>""";
    public const string Steps = """<path d="M8 6h10"/><path d="M6 12h9"/><path d="M11 18h7"/>""";
    public const string Step = """<circle cx="12" cy="12" r="10"/><path d="M12 6v6l4 2"/>""";
    public const string Query = """<circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/>""";

    // Processor icons
    public const string TemplateProcessor = """<path d="M14.5 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7.5L14.5 2z"/><polyline points="14 2 14 8 20 8"/><path d="M8 13h2"/><path d="M8 17h2"/>""";
    public const string TemplateTagProcessor = """<path d="m18 16 4-4-4-4"/><path d="m6 8-4 4 4 4"/><path d="m14.5 4-5 16"/>""";
    public const string CommandProcessor = """<rect width="18" height="18" x="3" y="3" rx="2"/><path d="m7 15 4-4-4-4"/><path d="M13 15h4"/>""";
    public const string ServiceProcessor = """<path d="M12 2v4"/><path d="m6.8 15-3.5 2"/><path d="m20.7 17-3.5-2"/><path d="M6.8 9 3.3 7"/><path d="m20.7 7-3.5 2"/><circle cx="12" cy="12" r="4"/>""";

    // Workflow icons
    public const string Workflows = """<rect width="8" height="8" x="3" y="3" rx="2"/><path d="M7 11v4a2 2 0 0 0 2 2h4"/><rect width="8" height="8" x="13" y="13" rx="2"/>""";
    public const string Workflow = """<circle cx="5" cy="6" r="3"/><path d="M5 9v6"/><circle cx="5" cy="18" r="3"/><path d="M12 3h7a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-7"/><path d="m15 9-3 3 3 3"/>""";
    public const string Runs = """<path d="M5 12h14"/><path d="M12 5v14"/>""";
    public const string Run = """<polygon points="6 3 20 12 6 21 6 3"/>""";
    public const string StepExecution = """<path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><path d="m9 11 3 3L22 4"/>""";

    // Git icons
    public const string GitBranch = """<line x1="6" x2="6" y1="3" y2="15"/><circle cx="18" cy="6" r="3"/><circle cx="6" cy="18" r="3"/><path d="M18 9a9 9 0 0 1-9 9"/>""";
    public const string GitCommit = """<circle cx="12" cy="12" r="4"/><line x1="1.05" x2="7" y1="12" y2="12"/><line x1="17.01" x2="22.96" y1="12" y2="12"/>""";
    public const string GitMerge = """<circle cx="18" cy="18" r="3"/><circle cx="6" cy="6" r="3"/><path d="M6 21V9a9 9 0 0 0 9 9"/>""";

    // Graph icons
    public const string Node = """<circle cx="12" cy="12" r="10"/>""";
    public const string Edge = """<path d="M5 12h14m-7-7l7 7-7 7"/>""";
    public const string NodeType = """<circle cx="12" cy="12" r="10"/><path d="M12 2v20"/><path d="M2 12h20"/>""";
    public const string EdgeType = """<path d="M5 12h14m-7-7l7 7-7 7"/>""";
    public const string Group = """<path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M22 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/>""";
    public const string Folder = """<path d="M20 20a2 2 0 0 0 2-2V8a2 2 0 0 0-2-2h-7.9a2 2 0 0 1-1.69-.9L9.6 3.9A2 2 0 0 0 7.93 3H4a2 2 0 0 0-2 2v13a2 2 0 0 0 2 2Z"/>""";
    public const string File = """<path d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>""";
    public const string Settings = """<path d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"/><path d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>""";

    // Requirements & Testing icons
    public const string UserStory = """<path d="M4 19.5v-15A2.5 2.5 0 0 1 6.5 2H20v20H6.5a2.5 2.5 0 0 1 0-5H20"/><path d="M8 8h8"/><path d="M8 12h8"/>""";
    public const string Scenario = """<path d="M3 3v18h18"/><path d="m7 10 4 4 6-6"/>""";
    public const string UAT = """<path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><path d="m9 11 3 3L22 4"/>""";
    public const string Usecase = """<rect width="18" height="18" x="3" y="3" rx="2"/><path d="M7 7h10"/><path d="M7 12h10"/><path d="M7 17h6"/>""";
    public const string Story = """<path d="M4 19.5v-15A2.5 2.5 0 0 1 6.5 2H20v20H6.5a2.5 2.5 0 0 1 0-5H20"/><circle cx="12" cy="8" r="2"/><path d="M15 13a3 3 0 1 0-6 0"/>""";
    public const string TableRelationship = """<path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"/><polyline points="3.27 6.96 12 12.01 20.73 6.96"/><line x1="12" y1="22.08" x2="12" y2="12"/>""";

    // Rule icons
    public const string Rule = """<path d="M12 20h9"/><path d="M16.5 3.5a2.12 2.12 0 0 1 3 3L7 19l-4 1 1-4Z"/><path d="m15 5 3 3"/>""";
    public const string RuleGroup = """<rect width="8" height="4" x="8" y="2" rx="1" ry="1"/><path d="M16 4h2a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h2"/><path d="M9 12h6"/><path d="M9 16h6"/>""";
    public const string RuleCondition = """<path d="M2 20h.01"/><path d="M7 20v-4"/><path d="M12 20v-8"/><path d="M17 20V8"/><path d="M22 4v16"/>""";

    // Terminal icon
    public const string Terminal = """<polyline points="4 17 10 11 4 5"/><line x1="12" x2="20" y1="19" y2="19"/>""";

    // Mailbox icons
    public const string Bell = """<path d="M6 8a6 6 0 0 1 12 0c0 7 3 9 3 9H3s3-2 3-9"/><path d="M10.3 21a1.94 1.94 0 0 0 3.4 0"/>""";

    // Plugin & Navigation icons
    public const string Plugin = """<path d="M12 2v6.5"/><path d="M18 9h-6"/><path d="M8 9H5a2 2 0 0 0-2 2v9a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-9a2 2 0 0 0-2-2h-1"/><circle cx="12" cy="9" r="3"/>""";
    public const string Menu = """<line x1="4" x2="20" y1="12" y2="12"/><line x1="4" x2="20" y1="6" y2="6"/><line x1="4" x2="20" y1="18" y2="18"/>""";
    public const string Link = """<path d="M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71"/><path d="M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71"/>""";

    // UI Icons
    public const string Send = """<path d="m22 2-7 20-4-9-9-4Z"/><path d="M22 2 11 13"/>""";
    public const string Search = """<circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/>""";
    public const string Home = """<path d="m3 9 9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/><polyline points="9 22 9 12 15 12 15 22"/>""";
    public const string Radar = """<path d="M19.07 4.93A10 10 0 0 0 6.99 3.34"/><path d="M4 6h.01"/><path d="M2.29 9.62A10 10 0 1 0 21.31 8.35"/><path d="M16.24 7.76A6 6 0 1 0 8.23 16.67"/><path d="M12 18h.01"/><path d="M17.99 11.66A6 6 0 0 1 15.77 16.67"/><circle cx="12" cy="12" r="2"/>""";
    public const string Store = """<path d="m2 7 4.41-4.41A2 2 0 0 1 7.83 2h8.34a2 2 0 0 1 1.42.59L22 7"/><path d="M4 12v8a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2v-8"/><path d="M15 22v-4a2 2 0 0 0-2-2h-2a2 2 0 0 0-2 2v4"/><path d="M2 7h20"/><path d="M22 7v3a2 2 0 0 1-2 2v0a2.7 2.7 0 0 1-1.59-.63.7.7 0 0 0-.82 0A2.7 2.7 0 0 1 16 12a2.7 2.7 0 0 1-1.59-.63.7.7 0 0 0-.82 0A2.7 2.7 0 0 1 12 12a2.7 2.7 0 0 1-1.59-.63.7.7 0 0 0-.82 0A2.7 2.7 0 0 1 8 12a2.7 2.7 0 0 1-1.59-.63.7.7 0 0 0-.82 0A2.7 2.7 0 0 1 4 12v0a2 2 0 0 1-2-2V7"/>""";
    public const string ChartLine = """<path d="M3 3v18h18"/><path d="m19 9-5 5-4-4-3 3"/>""";
    public const string Briefcase = """<path d="M16 20V4a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16"/><rect width="20" height="14" x="2" y="6" rx="2"/>""";
    public const string RefreshCw = """<path d="M3 12a9 9 0 0 1 9-9 9.75 9.75 0 0 1 6.74 2.74L21 8"/><path d="M21 3v5h-5"/><path d="M21 12a9 9 0 0 1-9 9 9.75 9.75 0 0 1-6.74-2.74L3 16"/><path d="M8 16H3v5"/>""";
    public const string CloudZap = """<path d="M6 16.326A7 7 0 1 1 15.71 8h1.79a4.5 4.5 0 0 1 .5 8.973"/><path d="m13 12-3 5h4l-3 5"/>""";
    public const string XCircle = """<circle cx="12" cy="12" r="10"/><path d="m15 9-6 6"/><path d="m9 9 6 6"/>""";
    public const string Check = """<path d="M20 6 9 17l-5-5"/>""";
    public const string ChevronUp = """<path d="m18 15-6-6-6 6"/>""";
    public const string Ellipsis = """<circle cx="12" cy="12" r="1"/><circle cx="19" cy="12" r="1"/><circle cx="5" cy="12" r="1"/>""";
    public const string EllipsisVertical = """<circle cx="12" cy="4" r="1"/><circle cx="12" cy="12" r="1"/><circle cx="12" cy="20" r="1"/>""";
    public const string Minimize2 = """<path d="M4 14h6v6"/><path d="M20 10h-6V4"/><path d="m14 10 7-7"/><path d="M3 21l7-7"/>""";
    public const string Maximize2 = """<path d="M8 3H5a2 2 0 0 0-2 2v3"/><path d="M21 8V5a2 2 0 0 0-2-2h-3"/><path d="M3 16v3a2 2 0 0 0 2 2h3"/><path d="M16 21h3a2 2 0 0 0 2-2v-3"/>""";
    public const string Layers = """<path d="m12.83 2.18a2 2 0 0 0-1.66 0L2.6 6.08a1 1 0 0 0 0 1.83l8.58 3.91a2 2 0 0 0 1.66 0l8.58-3.9a1 1 0 0 0 0-1.83Z"/><path d="m6.08 9.5-3.5 1.6a1 1 0 0 0 0 1.81l8.6 3.91a2 2 0 0 0 1.65 0l8.58-3.9a1 1 0 0 0 0-1.83l-3.5-1.59"/><path d="m6.08 14.5-3.5 1.6a1 1 0 0 0 0 1.81l8.6 3.91a2 2 0 0 0 1.65 0l8.58-3.9a1 1 0 0 0 0-1.83l-3.5-1.59"/>""";
    public const string Monitor = """<rect width="20" height="14" x="2" y="3" rx="2"/><path d="M8 21h8"/><path d="M12 17v4"/>""";
    public const string Pin = """<path d="M12 17v5"/><path d="M9 10.76a2 2 0 0 1-1.11 1.79l-1.78.9A2 2 0 0 0 5 15.24V16a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-.76a2 2 0 0 0-1.11-1.79l-1.78-.9A2 2 0 0 1 15 10.76V7a1 1 0 0 1 1-1 1 1 0 0 0 1-1V4a1 1 0 0 0-1-1H8a1 1 0 0 0-1 1v1a1 1 0 0 0 1 1 1 1 0 0 1 1 1z"/>""";
    public const string ChevronDown = """<path d="m6 9 6 6 6-6"/>""";
    public const string ChevronRight = """<path d="m9 18 6-6-6-6"/>""";
    public const string ChevronLeft = """<path d="m15 18-6-6 6-6"/>""";
    public const string Minus = """<path d="M5 12h14"/>""";
    public const string Plus = """<path d="M5 12h14"/><path d="M12 5v14"/>""";
    public const string PanelLeft = """<rect width="18" height="18" x="3" y="3" rx="2"/><path d="M9 3v18"/>""";
    public const string PanelLeftOpen = """<rect width="18" height="18" x="3" y="3" rx="2"/><path d="M9 3v18"/><path d="m14 9 3 3-3 3"/>""";
    public const string PanelLeftClose = """<rect width="18" height="18" x="3" y="3" rx="2"/><path d="M9 3v18"/><path d="m16 15-3-3 3-3"/>""";
    public const string Github = """<path d="M15 22v-4a4.8 4.8 0 0 0-1-3.5c3 0 6-2 6-5.5.08-1.25-.27-2.48-1-3.5.28-1.15.28-2.35 0-3.5 0 0-1 0-3 1.5-2.64-.5-5.36-.5-8 0C6 2 5 2 5 2c-.3 1.15-.3 2.35 0 3.5A5.403 5.403 0 0 0 4 9c0 3.5 3 5.5 6 5.5-.39.49-.68 1.05-.85 1.65-.17.6-.22 1.23-.15 1.85v4"/><path d="M9 18c-4.51 2-5-2-7-2"/>""";
    public const string FolderOpen = """<path d="m6 14 1.45-2.9A2 2 0 0 1 9.24 10H20a2 2 0 0 1 1.94 2.5l-1.55 6a2 2 0 0 1-1.94 1.5H4a2 2 0 0 1-2-2V5c0-1.1.9-2 2-2h3.93a2 2 0 0 1 1.66.9l.82 1.2a2 2 0 0 0 1.66.9H18a2 2 0 0 1 2 2v2"/>""";
    public const string AlertCircle = """<circle cx="12" cy="12" r="10"/><line x1="12" x2="12" y1="8" y2="12"/><line x1="12" x2="12.01" y1="16" y2="16"/>""";
    public const string ArrowLeft = """<path d="m12 19-7-7 7-7"/><path d="M19 12H5"/>""";
    public const string ArrowRight = """<path d="M5 12h14"/><path d="m12 5 7 7-7 7"/>""";
    public const string Package = """<path d="m7.5 4.27 9 5.15"/><path d="M21 8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16Z"/><path d="m3.3 7 8.7 5 8.7-5"/><path d="M12 22V12"/>""";
    public const string Braces = """<path d="M8 3H7a2 2 0 0 0-2 2v5a2 2 0 0 1-2 2 2 2 0 0 1 2 2v5c0 1.1.9 2 2 2h1"/><path d="M16 21h1a2 2 0 0 0 2-2v-5c0-1.1.9-2 2-2a2 2 0 0 1-2-2V5a2 2 0 0 0-2-2h-1"/>""";
    public const string Book = """<path d="M4 19.5v-15A2.5 2.5 0 0 1 6.5 2H20v20H6.5a2.5 2.5 0 0 1 0-5H20"/>""";
    public const string Cog = """<path d="M12 20a8 8 0 1 0 0-16 8 8 0 0 0 0 16Z"/><path d="M12 14a2 2 0 1 0 0-4 2 2 0 0 0 0 4Z"/><path d="M12 2v2"/><path d="M12 22v-2"/><path d="m17 20.66-1-1.73"/><path d="M11 10.27 7 3.34"/><path d="m20.66 17-1.73-1"/><path d="m3.34 7 1.73 1"/><path d="M14 12h8"/><path d="M2 12h2"/><path d="m20.66 7-1.73 1"/><path d="m3.34 17 1.73-1"/><path d="m17 3.34-1 1.73"/><path d="m11 13.73-4 6.93"/>""";
    public const string Moon = """<path d="M12 3a6 6 0 0 0 9 9 9 9 0 1 1-9-9Z"/>""";
    public const string Sun = """<circle cx="12" cy="12" r="4"/><path d="M12 2v2"/><path d="M12 20v2"/><path d="m4.93 4.93 1.41 1.41"/><path d="m17.66 17.66 1.41 1.41"/><path d="M2 12h2"/><path d="M20 12h2"/><path d="m6.34 17.66-1.41 1.41"/><path d="m19.07 4.93-1.41 1.41"/>""";
    public const string CircleCheck = """<circle cx="12" cy="12" r="10"/><path d="m9 12 2 2 4-4"/>""";
    public const string CircleX = """<circle cx="12" cy="12" r="10"/><path d="m15 9-6 6"/><path d="m9 9 6 6"/>""";
    public const string CircleAlert = """<circle cx="12" cy="12" r="10"/><line x1="12" x2="12" y1="8" y2="12"/><line x1="12" x2="12.01" y1="16" y2="16"/>""";
    public const string Info = """<circle cx="12" cy="12" r="10"/><path d="M12 16v-4"/><path d="M12 8h.01"/>""";
    public const string X = """<path d="M18 6 6 18"/><path d="m6 6 12 12"/>""";
    public const string Eye = """<path d="M2.062 12.348a1 1 0 0 1 0-.696 10.75 10.75 0 0 1 19.876 0 1 1 0 0 1 0 .696 10.75 10.75 0 0 1-19.876 0"/><circle cx="12" cy="12" r="3"/>""";
    public const string Trash = """<path d="M3 6h18"/><path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"/><path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/><line x1="10" x2="10" y1="11" y2="17"/><line x1="14" x2="14" y1="11" y2="17"/>""";
    public const string Database = """<ellipse cx="12" cy="5" rx="9" ry="3"/><path d="M3 5V19A9 3 0 0 0 21 19V5"/><path d="M3 12A9 3 0 0 0 21 12"/>""";

    // Additional UI Icons
    public const string Download = """<path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><polyline points="7 10 12 15 17 10"/><line x1="12" x2="12" y1="15" y2="3"/>""";
    public const string CheckCircle = """<circle cx="12" cy="12" r="10"/><path d="m9 12 2 2 4-4"/>""";
    public const string AlertTriangle = """<path d="m21.73 18-8-14a2 2 0 0 0-3.48 0l-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.73-3Z"/><path d="M12 9v4"/><path d="M12 17h.01"/>""";
    public const string Trophy = """<path d="M6 9H4.5a2.5 2.5 0 0 1 0-5H6"/><path d="M18 9h1.5a2.5 2.5 0 0 0 0-5H18"/><path d="M4 22h16"/><path d="M10 14.66V17c0 .55-.47.98-.97 1.21C7.85 18.75 7 20.24 7 22"/><path d="M14 14.66V17c0 .55.47.98.97 1.21C16.15 18.75 17 20.24 17 22"/><path d="M18 2H6v7a6 6 0 0 0 12 0V2Z"/>""";
    public const string ChartNoAxesGantt = """<path d="M8 6h10"/><path d="M6 12h9"/><path d="M11 18h7"/>""";
    public const string Clock = """<circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/>""";
    public const string Inbox = """<polyline points="22 12 16 12 14 15 10 15 8 12 2 12"/><path d="M5.45 5.11 2 12v6a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-6l-3.45-6.89A2 2 0 0 0 16.76 4H7.24a2 2 0 0 0-1.79 1.11z"/>""";
    public const string Cylinder = """<ellipse cx="12" cy="5" rx="9" ry="3"/><path d="M3 5v14a9 3 0 0 0 18 0V5"/>""";
    public const string Wand = """<path d="M15 4V2"/><path d="M15 16v-2"/><path d="M8 9h2"/><path d="M20 9h2"/><path d="M17.8 11.8 19 13"/><path d="M15 9h.01"/><path d="M17.8 6.2 19 5"/><path d="m3 21 9-9"/><path d="M12.2 6.2 11 5"/>""";
    public const string ExternalLink = """<path d="M15 3h6v6"/><path d="M10 14 21 3"/><path d="M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6"/>""";
    public const string LogOut = """<path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/><polyline points="16 17 21 12 16 7"/><line x1="21" x2="9" y1="12" y2="12"/>""";
    public const string EyeOff = """<path d="M10.733 5.076a10.744 10.744 0 0 1 11.205 6.575 1 1 0 0 1 0 .696 10.747 10.747 0 0 1-1.444 2.49"/><path d="M14.084 14.158a3 3 0 0 1-4.242-4.242"/><path d="M17.479 17.499a10.75 10.75 0 0 1-15.417-5.151 1 1 0 0 1 0-.696 10.75 10.75 0 0 1 4.446-5.143"/><path d="m2 2 20 20"/>""";
    public const string Hash = """<line x1="4" x2="20" y1="9" y2="9"/><line x1="4" x2="20" y1="15" y2="15"/><line x1="10" x2="8" y1="3" y2="21"/><line x1="16" x2="14" y1="3" y2="21"/>""";
    public const string Cloud = """<path d="M17.5 19H9a7 7 0 1 1 6.71-9h1.79a4.5 4.5 0 1 1 0 9Z"/>""";
    public const string Lock = """<rect width="18" height="11" x="3" y="11" rx="2" ry="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/>""";
    public const string LogIn = """<path d="M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4"/><polyline points="10 17 15 12 10 7"/><line x1="15" x2="3" y1="12" y2="12"/>""";
    public const string UserPlus = """<path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><line x1="19" x2="19" y1="8" y2="14"/><line x1="22" x2="16" y1="11" y2="11"/>""";

    // Editor/Block icons
    public const string AlignLeft = """<line x1="17" x2="3" y1="10" y2="10"/><line x1="21" x2="3" y1="6" y2="6"/><line x1="21" x2="3" y1="14" y2="14"/><line x1="17" x2="3" y1="18" y2="18"/>""";
    public const string Image = """<rect width="18" height="18" x="3" y="3" rx="2" ry="2"/><circle cx="9" cy="9" r="2"/><path d="m21 15-3.086-3.086a2 2 0 0 0-2.828 0L6 21"/>""";
    public const string GripVertical = """<circle cx="9" cy="12" r="1"/><circle cx="9" cy="5" r="1"/><circle cx="9" cy="19" r="1"/><circle cx="15" cy="12" r="1"/><circle cx="15" cy="5" r="1"/><circle cx="15" cy="19" r="1"/>""";
    public const string Quote = """<path d="M3 21c3 0 7-1 7-8V5c0-1.25-.756-2.017-2-2H4c-1.25 0-2 .75-2 1.972V11c0 1.25.75 2 2 2 1 0 1 0 1 1v1c0 1-1 2-2 2s-1 .008-1 1.031V21c0 1 0 1 1 1z"/><path d="M15 21c3 0 7-1 7-8V5c0-1.25-.757-2.017-2-2h-4c-1.25 0-2 .75-2 1.972V11c0 1.25.75 2 2 2h.75c0 2.25.25 4-2.75 4v3c0 1 0 1 1 1z"/>""";
    public const string Code = """<polyline points="16 18 22 12 16 6"/><polyline points="8 6 2 12 8 18"/>""";
    public const string Save = """<path d="M15.2 3a2 2 0 0 1 1.4.6l3.8 3.8a2 2 0 0 1 .6 1.4V19a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2z"/><path d="M17 21v-7a1 1 0 0 0-1-1H8a1 1 0 0 0-1 1v7"/><path d="M7 3v4a1 1 0 0 0 1 1h7"/>""";

    public const string Key = """<path d="m15.5 7.5 2.3 2.3a1 1 0 0 0 1.4 0l2.1-2.1a1 1 0 0 0 0-1.4L19 4"/><path d="m21 2-9.6 9.6"/><circle cx="7.5" cy="15.5" r="5.5"/>""";
    public const string Copy = """<rect width="14" height="14" x="8" y="8" rx="2" ry="2"/><path d="M4 16c-1.1 0-2-.9-2-2V4c0-1.1.9-2 2-2h10c1.1 0 2 .9 2 2"/>""";
    public const string Pencil = """<path d="M21.174 6.812a1 1 0 0 0-3.986-3.987L3.842 16.174a2 2 0 0 0-.5.83l-1.321 4.352a.5.5 0 0 0 .623.622l4.353-1.32a2 2 0 0 0 .83-.497z"/><path d="m15 5 4 4"/>""";

    // Spacing icons
    public const string Rows = """<rect width="18" height="4" x="3" y="3" rx="1"/><rect width="18" height="4" x="3" y="17" rx="1"/>""";
    public const string RowsCompressed = """<rect width="18" height="4" x="3" y="5" rx="1"/><rect width="18" height="4" x="3" y="15" rx="1"/>""";

    // Blog/Feed icons
    public const string Heart = """<path d="M19 14c1.49-1.46 3-3.21 3-5.5A5.5 5.5 0 0 0 16.5 3c-1.76 0-3 .5-4.5 2-1.5-1.5-2.74-2-4.5-2A5.5 5.5 0 0 0 2 8.5c0 2.3 1.5 4.05 3 5.5l7 7Z"/>""";
    public const string MessageCircle = """<path d="M7.9 20A9 9 0 1 0 4 16.1L2 22Z"/>""";
    public const string Bookmark = """<path d="m19 21-7-4-7 4V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2v16z"/>""";
    public const string Play = """<polygon points="6 3 20 12 6 21 6 3"/>""";
    public const string Square = """<rect width="14" height="14" x="5" y="5" rx="2"/>""";

    // ── Aliases for common NodeType keys that don't have their own glyph ──
    public const string Session = Thread;
    public const string Sessions = Threads;
    public const string Flow = Workflow;
    public const string Flows = Workflows;
    public const string Setting = Settings;
    public const string Header = Folder;

    // Lucide rotate-ccw / rotate-cw — undo / redo arrows. Used by the canvas
    // designer toolbar (SPEC2100.US24).
    public const string Undo = """<path d="M3 7v6h6"/><path d="M21 17a9 9 0 0 0-9-9 9 9 0 0 0-6 2.3L3 13"/>""";
    public const string Redo = """<path d="M21 7v6h-6"/><path d="M3 17a9 9 0 0 1 9-9 9 9 0 0 1 6 2.3l3 2.7"/>""";

    // ── Lookup by name ──

    private static readonly Dictionary<string, string> _byName = typeof(Icons)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.IsLiteral && f.FieldType == typeof(string))
        .ToDictionary(f => f.Name, f => (string)f.GetRawConstantValue()!, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Alphabetised catalog of every Lucide icon constant declared on
    /// <see cref="Icons"/>, materialised once at type load. Drives icon
    /// pickers (e.g. SPEC2100.US19 Type Definition Icon dropdown) so new
    /// constants land in the UI without touching consumers.
    /// </summary>
    public static readonly IReadOnlyList<IconEntry> All = _byName
        .Select(kv => new IconEntry(kv.Key, kv.Value))
        .OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
        .ToList();

    /// <summary>One row in <see cref="All"/>: the constant's identifier
    /// + its SVG paths string. Bundled together so icon pickers can render
    /// the glyph next to the label without a second reflection pass.</summary>
    public sealed record IconEntry(string Name, string Paths);

    /// <summary>
    /// Resolve an icon SVG paths string by field name (e.g. "Workspace", "Settings").
    /// Accepts PascalCase ("CodeGen"), kebab-case ("code-gen"), snake_case ("code_gen"),
    /// or space-separated ("code gen"). Falls back to singular/plural variants when the
    /// exact name is not found. Returns null if no match.
    /// </summary>
    public static string? GetByName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        if (_byName.TryGetValue(name, out var paths)) return paths;

        // Strip separators (-, _, space) so kebab-case keys resolve to PascalCase fields.
        var normalized = name.Replace("-", "").Replace("_", "").Replace(" ", "");
        if (_byName.TryGetValue(normalized, out paths)) return paths;

        // Try singular ("packages" → "package") and naive plural ("package" → "packages").
        if (normalized.Length > 1 && normalized.EndsWith('s')
            && _byName.TryGetValue(normalized[..^1], out paths)) return paths;
        if (_byName.TryGetValue(normalized + "s", out paths)) return paths;

        // For dotted names like "flow.header" fall back to the last segment ("header")
        // so sub-typed node keys inherit the base type's icon.
        if (name.Contains('.'))
        {
            var lastSegment = name[(name.LastIndexOf('.') + 1)..];
            if (_byName.TryGetValue(lastSegment, out paths)) return paths;
            var normalizedLast = lastSegment.Replace("-", "").Replace("_", "").Replace(" ", "");
            if (_byName.TryGetValue(normalizedLast, out paths)) return paths;
        }

        return null;
    }
}
