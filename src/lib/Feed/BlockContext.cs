using System.Text.Json;

namespace LowlandTech.Components.Feed;

/// <summary>
/// Provides shared context for blocks rendered inside the session feed.
/// </summary>
public sealed class BlockContext
{
	private readonly Dictionary<Guid, BlockMode> _blockModes = new();
	private readonly Dictionary<Guid, List<Action<string>>> _legacySubscribers = new();
	private readonly Dictionary<Guid, List<Action<BlockMode>>> _blockSubscribers = new();
	private readonly Dictionary<Guid, JsonElement> _entityData = new();

	public event Func<Guid, BlockMode, Task>? OnBlockModeChanged;

	public SessionContext Context { get; } = new();

	/// <summary>
	/// Gets the current mode ("normal" or "maximized") in the legacy string format.
	/// </summary>
	public string Mode => ToLegacyMode(GetGlobalMode());

	/// <summary>
	/// Gets whether any block is currently maximized.
	/// </summary>
	public bool IsMaximized => GetGlobalMode() == BlockMode.Maximized;

	public BlockMode GetMode(Guid blockId)
		=> _blockModes.TryGetValue(blockId, out var mode) ? mode : GetGlobalMode();

	public Task SetModeAsync(Guid blockId, BlockMode mode)
	{
		SetMode(blockId, mode);
		return Task.CompletedTask;
	}

	public void SetMode(Guid blockId, BlockMode mode)
	{
		if (GetMode(blockId) == mode)
			return;

		_blockModes[blockId] = mode;
		NotifyBlockSubscribers(blockId, mode);
		NotifyLegacySubscribers(blockId, mode);

		if (OnBlockModeChanged is { } handler)
			_ = handler.Invoke(blockId, mode);
	}

	/// <summary>
	/// Sets a global mode for all subscribers.
	/// </summary>
	public void SetMode(bool isMaximized)
	{
		var mode = isMaximized ? BlockMode.Maximized : BlockMode.Normal;

		foreach (var subscribers in _legacySubscribers.Values)
		{
			foreach (var callback in subscribers.ToList())
			{
				try
				{
					callback(ToLegacyMode(mode));
				}
				catch
				{
					// Ignore subscriber errors.
				}
			}
		}

		foreach (var subscribers in _blockSubscribers.Values)
		{
			foreach (var callback in subscribers.ToList())
			{
				try
				{
					callback(mode);
				}
				catch
				{
					// Ignore subscriber errors.
				}
			}
		}
	}

	/// <summary>
	/// Legacy subscribe API that receives mode as a string.
	/// </summary>
	public void Subscribe(Guid blockId, Action<string> onModeChange)
	{
		if (!_legacySubscribers.ContainsKey(blockId))
			_legacySubscribers[blockId] = new List<Action<string>>();

		_legacySubscribers[blockId].Add(onModeChange);
		onModeChange(ToLegacyMode(GetMode(blockId)));
	}

	public void Subscribe(Guid blockId, Action<BlockMode> onModeChange)
	{
		if (!_blockSubscribers.ContainsKey(blockId))
			_blockSubscribers[blockId] = new List<Action<BlockMode>>();

		_blockSubscribers[blockId].Add(onModeChange);
		onModeChange(GetMode(blockId));
	}

	/// <summary>
	/// Legacy unsubscribe API that receives mode as a string.
	/// </summary>
	public void Unsubscribe(Guid blockId, Action<string> onModeChange)
	{
		if (_legacySubscribers.TryGetValue(blockId, out var callbacks))
		{
			callbacks.Remove(onModeChange);
			if (callbacks.Count == 0)
				_legacySubscribers.Remove(blockId);
		}
	}

	public void Unsubscribe(Guid blockId, Action<BlockMode> onModeChange)
	{
		if (_blockSubscribers.TryGetValue(blockId, out var callbacks))
		{
			callbacks.Remove(onModeChange);
			if (callbacks.Count == 0)
				_blockSubscribers.Remove(blockId);
		}
	}

	/// <summary>
	/// Legacy entity-data setter API.
	/// </summary>
	public void SetEntityData(Guid blockId, JsonElement data)
	{
		_entityData[blockId] = data;
		Context.Publish(blockId, "Unknown", data);
	}

	public void SetEntityData(Guid blockId, string blockType, JsonElement data)
	{
		_entityData[blockId] = data;
		Context.Publish(blockId, blockType, data);
	}

	/// <summary>
	/// Legacy entity-data getter API.
	/// </summary>
	public JsonElement? GetEntityData(Guid blockId)
	{
		return _entityData.TryGetValue(blockId, out var data) ? data : null;
	}

	public T? GetEntity<T>(Guid blockId) where T : class
	{
		if (!_entityData.TryGetValue(blockId, out var data))
			return null;

		try
		{
			return JsonSerializer.Deserialize<T>(data.GetRawText());
		}
		catch
		{
			return null;
		}
	}

	public void ClearEntityData(Guid blockId)
	{
		_entityData.Remove(blockId);
		Context.Remove(blockId);
	}

	private static string ToLegacyMode(BlockMode mode) => mode switch
	{
		BlockMode.Maximized => "maximized",
		BlockMode.Collapsed => "collapsed",
		_ => "normal"
	};

	private BlockMode GetGlobalMode()
		=> _blockModes.Values.Any(mode => mode == BlockMode.Maximized)
			? BlockMode.Maximized
			: BlockMode.Normal;

	private void NotifyBlockSubscribers(Guid blockId, BlockMode mode)
	{
		if (!_blockSubscribers.TryGetValue(blockId, out var callbacks))
			return;

		foreach (var callback in callbacks.ToList())
		{
			try
			{
				callback(mode);
			}
			catch
			{
				// Ignore subscriber errors.
			}
		}
	}

	private void NotifyLegacySubscribers(Guid blockId, BlockMode mode)
	{
		if (!_legacySubscribers.TryGetValue(blockId, out var callbacks))
			return;

		var legacyMode = ToLegacyMode(mode);
		foreach (var callback in callbacks.ToList())
		{
			try
			{
				callback(legacyMode);
			}
			catch
			{
				// Ignore subscriber errors.
			}
		}
	}
}