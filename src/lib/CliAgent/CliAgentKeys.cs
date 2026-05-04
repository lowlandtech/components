namespace LowlandTech.Components.CliAgent;

public static class CliAgentKeys
{
    public static string ForBlock(Guid blockId) => $"cli-agent::{blockId}";

    public static string ForAgent(string agentName) => $"cli-agent::{agentName}";
}
