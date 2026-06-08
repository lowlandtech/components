// Global using directives

global using System.ComponentModel;
global using System.Reflection;
global using System.Text.Json;
global using Bunit;
global using LowlandTech.Components;
global using LowlandTech.Components.Stories.Services.Themes;
global using LowlandTech.Components.Stories.Showcode;
global using LowlandTech.Components.Stories.Showcode.Stories.Misc;
global using LowlandTech.Components.Tests.Infrastructure;
global using LowlandTech.Core;
global using LowlandTech.Core.Abstractions;
global using LowlandTech.Core.Graph.Entities;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Rendering;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.Extensions.DependencyInjection;
// Alias for ThemeProvider component — the class name matches its namespace,
// so a direct reference competes with the namespace. Use TP everywhere in tests.
global using TP = LowlandTech.Components.ThemeProvider;
// Component aliases to disambiguate type from namespace (class name == namespace name).
global using BadgeSut = LowlandTech.Components.Badge;
global using ButtonSut = LowlandTech.Components.Button;
global using CardSut = LowlandTech.Components.Card;
global using SessionAppSut = LowlandTech.Components.SessionApp;
