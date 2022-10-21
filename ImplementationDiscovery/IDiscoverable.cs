namespace CodeChops.ImplementationDiscovery;

public interface IDiscoverable
{
	public static abstract void SetInitialized();
	public static abstract bool IsInitialized { get; }
}