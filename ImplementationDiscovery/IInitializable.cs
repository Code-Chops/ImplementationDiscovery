namespace CodeChops.ImplementationDiscovery;

public interface IInitializable
{
	/// <summary>
	/// Is false when the enum is still in static buildup and true if this is finished.
	/// This parameter can be used to detect cyclic references during buildup and act accordingly.
	/// </summary>
	public static abstract bool IsInitialized { get; }
}