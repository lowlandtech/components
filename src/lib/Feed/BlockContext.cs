using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace LowlandTech.Components;

/// <summary>
/// Provides shared context for blocks rendered inside the session feed.
/// </summary>
public sealed class BlockContext
{
	private readonly Dictionary<Guid, BlockMode> _blockModes = new();
	private readonly Dictionary<Guid, List<Action<string>>> _legacySubscribers = new();
	private readonly Dictionary<Guid, List<Action<BlockMode>>> _blockSubscribers = new();
	private readonly Dictionary<Guid, JsonElement> _entityData = new();
	private readonly Dictionary<Guid, bool> _blockValidity = new();
	private readonly Dictionary<Guid, List<Action<bool>>> _validitySubscribers = new();
	private readonly Dictionary<Guid, Func<IReadOnlyList<BlockIntent>>> _intentSurfaces = new();
	private readonly Dictionary<Guid, Func<string, IReadOnlyDictionary<string, object?>?, CancellationToken, Task<BlockIntentDispatchResult>>> _intentDispatchers = new();

	public event Func<Guid, BlockMode, Task>? OnBlockModeChanged;

	public SessionContext Context { get; } = new();

	/// <summary>
	/// Gets the current mode ("normal" or "maximized") in the legacy string format.
	/// </summary>
	public string Mode => ToLegacyMode(GetGlobalMode());

	/// <summary>
	/// Gets whether any block is currently expanded.
	/// </summary>
	public bool IsMaximized => GetGlobalMode() == BlockMode.Expanded;

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
		var mode = isMaximized ? BlockMode.Expanded : BlockMode.Normal;

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

	/// <summary>
	/// Reads the entity data of a block into <typeparamref name="T"/> through the
	/// shape that the caller's generated contract names. This is the trim-safe path:
	/// the shape is declared where the type is known instead of resolved by reflection.
	/// </summary>
	public T? GetEntity<T>(Guid blockId, JsonTypeInfo<T> typeInfo)
	{
		ArgumentNullException.ThrowIfNull(typeInfo);
		if (!_entityData.TryGetValue(blockId, out var data))
			return default;

		try
		{
			return JsonSerializer.Deserialize(data.GetRawText(), typeInfo);
		}
		catch
		{
			return default;
		}
	}

	/// <summary>
	/// Reads the entity data of a block into <typeparamref name="T"/> by reflection.
	/// The caller has to preserve the members of <typeparamref name="T"/>; prefer the
	/// overload that names the shape through a <see cref="JsonTypeInfo{T}"/>.
	/// </summary>
	[RequiresUnreferencedCode("Reads the entity payload through reflection; the caller must preserve the members of T or use the JsonTypeInfo overload.")]
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

	// ── Intent executor channel ──
	// Blocks register a semantic intent surface plus a block-local dispatch
	// function keyed by BlockId. Hosts and agents invoke this boundary without
	// reaching into foreign reducers or constructing foreign actions.

	public void RegisterIntent(
		Guid blockId,
		Func<IReadOnlyList<BlockIntent>> getSurface,
		Func<string, IReadOnlyDictionary<string, object?>?, CancellationToken, Task<BlockIntentDispatchResult>> dispatchAsync)
	{
		_intentSurfaces[blockId] = getSurface;
		_intentDispatchers[blockId] = dispatchAsync;
	}

	public IReadOnlyList<BlockIntent> GetIntentSurface(Guid blockId)
	{
		if (!_intentSurfaces.TryGetValue(blockId, out var getSurface))
			return [];

		try
		{
			return getSurface() ?? [];
		}
		catch
		{
			return [];
		}
	}

	public async Task<BlockIntentDispatchResult> DispatchIntentAsync(
		Guid blockId,
		string key,
		IReadOnlyDictionary<string, object?>? payload = null,
		CancellationToken ct = default)
	{
		if (!_intentDispatchers.TryGetValue(blockId, out var dispatchAsync))
			return new BlockIntentDispatchResult.Unsupported(key);

		try
		{
			return await dispatchAsync(key, payload, ct);
		}
		catch (Exception ex)
		{
			return new BlockIntentDispatchResult.Failure(ex.Message);
		}
	}

	public void UnregisterIntent(Guid blockId)
	{
		_intentSurfaces.Remove(blockId);
		_intentDispatchers.Remove(blockId);
	}

	// ── Validity channel ──
	// A block's BlockBase.IsValid value is published here on render and consumed
	// by session-level observers (or other blocks) that need to react to validity
	// without knowing how the block computes it. Default: true (a block is valid
	// until proven otherwise).

	/// <summary>
	/// Returns the last published validity for <paramref name="blockId"/>, or
	/// <c>true</c> if nothing has been published yet — consumers should treat
	/// "unknown" as "valid" so a block that doesn't validate (e.g. a chat
	/// thread) doesn't trip session-wide gates.
	/// </summary>
	public bool GetValid(Guid blockId)
		=> !_blockValidity.TryGetValue(blockId, out var v) || v;

	/// <summary>
	/// Publishes <paramref name="isValid"/> for <paramref name="blockId"/>.
	/// Notifies subscribers only when the value actually changes — repeated
	/// publishes of the same value are deduped.
	/// </summary>
	public void SetValid(Guid blockId, bool isValid)
	{
		var current = GetValid(blockId);
		_blockValidity[blockId] = isValid;
		if (current == isValid) return;
		NotifyValiditySubscribers(blockId, isValid);
	}

	/// <summary>
	/// Subscribes to validity changes for <paramref name="blockId"/>. The
	/// callback is invoked immediately with the current value (matching the
	/// mode-channel pattern), then once per subsequent change.
	/// </summary>
	public void SubscribeValid(Guid blockId, Action<bool> onValidityChanged)
	{
		if (!_validitySubscribers.ContainsKey(blockId))
			_validitySubscribers[blockId] = new List<Action<bool>>();

		_validitySubscribers[blockId].Add(onValidityChanged);
		onValidityChanged(GetValid(blockId));
	}

	public void UnsubscribeValid(Guid blockId, Action<bool> onValidityChanged)
	{
		if (_validitySubscribers.TryGetValue(blockId, out var callbacks))
		{
			callbacks.Remove(onValidityChanged);
			if (callbacks.Count == 0)
				_validitySubscribers.Remove(blockId);
		}
	}

	/// <summary>Drops cached validity + subscribers for <paramref name="blockId"/>.
	/// Call when a block unmounts to avoid stale state for its id.</summary>
	public void ClearValid(Guid blockId)
	{
		_blockValidity.Remove(blockId);
		_validitySubscribers.Remove(blockId);
	}

	private void NotifyValiditySubscribers(Guid blockId, bool isValid)
	{
		if (!_validitySubscribers.TryGetValue(blockId, out var callbacks))
			return;

		foreach (var callback in callbacks.ToList())
		{
			try
			{
				callback(isValid);
			}
			catch
			{
				// Ignore subscriber errors — match the existing pattern.
			}
		}
	}

	private static string ToLegacyMode(BlockMode mode) => mode switch
	{
		BlockMode.Expanded => "maximized",
		BlockMode.ContentOverlaid => "content-overlaid",
		BlockMode.AppOverlaid => "app-overlaid",
		BlockMode.Collapsed => "collapsed",
		_ => "normal"
	};

	private BlockMode GetGlobalMode()
		=> _blockModes.Values.Any(mode => mode == BlockMode.Expanded)
			? BlockMode.Expanded
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