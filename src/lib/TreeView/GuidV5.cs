namespace LowlandTech.Components;

/// <summary>
/// Generates deterministic (name-based) UUIDs per RFC 4122 §4.3 (version 5, SHA-1).
/// </summary>
internal static class GuidV5
{
    public static Guid Create(Guid namespaceId, string name)
    {
        // Convert namespace UUID to network byte order (big-endian)
        var nsBytes = namespaceId.ToByteArray();
        SwapToNetworkOrder(nsBytes);

        var nameBytes = Encoding.UTF8.GetBytes(name);

        var combined = new byte[nsBytes.Length + nameBytes.Length];
        nsBytes.CopyTo(combined, 0);
        nameBytes.CopyTo(combined, nsBytes.Length);

        var hash = SHA1.HashData(combined);

        // Stamp version 5 (0101xxxx) into octet 6
        hash[6] = (byte)((hash[6] & 0x0F) | 0x50);
        // Stamp RFC 4122 variant (10xxxxxx) into octet 8
        hash[8] = (byte)((hash[8] & 0x3F) | 0x80);

        // Take the first 16 bytes and convert back to host byte order
        var guidBytes = hash[..16];
        SwapToNetworkOrder(guidBytes);
        return new Guid(guidBytes);
    }

    // The Guid ↔ byte[] layout on Windows stores the first three fields in
    // little-endian, while RFC 4122 mandates big-endian.  This method swaps
    // those three fields in place; calling it twice is its own inverse.
    private static void SwapToNetworkOrder(byte[] b)
    {
        (b[0], b[3]) = (b[3], b[0]);
        (b[1], b[2]) = (b[2], b[1]);
        (b[4], b[5]) = (b[5], b[4]);
        (b[6], b[7]) = (b[7], b[6]);
    }
}
