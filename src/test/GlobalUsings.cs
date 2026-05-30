// Global using directives

global using System.ComponentModel;
global using System.Reflection;
global using Bunit;
global using LowlandTech.Components;
global using LowlandTech.Components.Feed;
global using LowlandTech.Components.Popover;
global using LowlandTech.Components.Resizable;
global using LowlandTech.Components.Stories.Services.Themes;
global using LowlandTech.Components.Stories.Showcode;
global using LowlandTech.Components.Stories.Showcode.Stories.Misc;
global using LowlandTech.Components.Select;
global using LowlandTech.Components.Session;
global using LowlandTech.Components.Session.Agents;
global using LowlandTech.Components.Tabs;
global using LowlandTech.Components.Tests.Infrastructure;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Rendering;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.Extensions.DependencyInjection;
// Alias for ThemeProvider component — the class name matches its namespace,
// so a direct reference competes with the namespace. Use TP everywhere in tests.
global using TP = LowlandTech.Components.ThemeProvider.ThemeProvider;
