# Implementation discovery
Provides easy-accessible, design-time information about implementations throughout your code. Just place an attribute on a specific base class or interface whose implementations you want to discover. A source generator will create an eum at design time that contains instances of all implementations.

> Check out [CodeChops projects](https://www.CodeChops.nl/projects) for more projects.

## Advantages
- All implementations of a specific class or interface are centralized in one place.
- You have a simple and navigable overview over what is implemented.
- Does not use reflection. This improves startup time of your app.
- [Assembly-level trimming](https://devblogs.microsoft.com/dotnet/app-trimming-in-net-5/) can be implemented in your libraries with ease: Implementations will not be trimmed because the enum contains a hard link to each implementation.
- No need to manually implement logic to search for the correct implementation.
- Members can be found in a static context, so there is no need to pass around a collection of implementations.

# Usage
1. Make the `base class` or `interface` whose implementations you want to discover `partial`.
2. Place the attribute `DiscoverImplementations` on the declaration and optionally provide the following parameters:
   - `EnumName`: The name of the enum that is being generated. If not provided, it will create one for you, see [enum name creation](#Enum-name-creation).
   - `GenerateImplementationIds`: If `true` (default), all discovered implementations get an implementation ID property, see [implementation IDs](#Implementation-IDs).
   - `HasSingletonImplementations` If `true`, the ID of all discovered implementations will be their implementation ID. It is set to `false` by default.
   - `GenerateProxies`: If `true`, implementations are discovered in assemblies that reference the base class / interface that has to be discovered. 
This is done by creating a proxy enum in the assembly of the implementation (under the namespace of the base class / interface. 
It is set to `false` by default. For more information, see [cross-assembly implementations](#Cross-assembly-implementations).

> ## Enum name creation
> If no custom enum name has been provided to the attribute, a name will be created for you:
> The name of the base class or interface will be used without the leading 'I' (for interfaces) or trailing 'Base' for base classes.
> `Enum` will be placed at the end of the name.
> Examples:
> - `AnimalBase` will become `AnimalEnum`.
> - `IVehicle`will become `VehicleEnum`.

> The [LightResources library](https://github.com/Code-Chops/LightResources/) makes use of this library to collect all the resources within an assemby.

> The [Geometry library](https://github.com/Code-Chops/Geometry/) makes use of this library to collect every `StrictDirection` implementation under one enum.

> The [Blame game engine library](https://github.com/Code-Chops/Blame/) makes use of this library to discover implemented GameObjects (not released yet).

# Concepts

## Implementations enum
When the 2 steps above are executed, an enum will be source generated in the same `namespace` as the base class / interface. 
The enum is inherited from `ImplementationsEnum`. 
It contains a mapping from implementation class name (the value of the enum member) to a [discovered object](#Discovered-object) (the value of the enum member). 
This enum can be used to iterate or look up members dynamically or statically, see [API](#Api).
> The implementations enum makes use of MagicEnums under the hood: a customizable and extendable way to implement enums. 
> For more information, see the [MagicEnums library](https://github.com/Code-Chops/MagicEnums/).

## Discovered object
The value of each enum member is a `DiscoveredObject`. 
This object contains the type, an instance, and a factory of the discovered implementation. 
It can implicitly be casted to the the concrete type of the implementation or the base class / interface.

# Implementation IDs
When setting `GenerateImplementationIds` is enabled, each discovered implementation gets an implementation ID property when it is set to `partial`.
A `static` property named `ImplementationId` will be generated which holds the corresponding implementation enum value.
It creates an easy way to reach the implementation enum and look up members dynamically.

## API
Implementation discovery makes use of MagicEnums under the hood, so the base API is the same as the [MagicEnum API](https://github.com/Code-Chops/MagicEnums/#api):

| Method                | Description                                                                                                                      |
|-----------------------|----------------------------------------------------------------------------------------------------------------------------------|
| `CreateMember`        | Creates a new discovered implementation member and returns it.                                                                   |
| `GetEnumerator`       | Gets an enumerator over the enum members.                                                                                        |
| `GetMembers`          | Gets an enumerable over:<br/>- All enum members, or<br/>- Members of a specific value: **Throws when no member has been found.** |
| `GetValues`           | Gets an enumerable over the member values.                                                                                       |
| `TryGetMembers`       | Tries to get member(s) by value.                                                                                                 |
| `TryGetSingleMember`  | Tries to get a single member by name / value.<br/>**Throws when multiple members of the same value have been found.**            |
| `GetSingleMember`     | Gets a single member by name / value.<br/>**Throws when not found or multiple members have been found.**                         |
| `GetUniqueValueCount` | Gets the unique member value count.                                                                                              |
| `GetMemberCount`      | Gets the member count.                                                                                                           |
| `GetDefaultValue`     | Gets the default value of the enum.                                                                                              |
| `GetOrCreateMember`   | Creates a member or gets one if a member already exists.                                                                         |

This API can be used to search for, or create implementations at runtime.
This package offers some extra methods and classes on the enum (`ImplementationsEnum`) and on the value (`DiscoveredObject`) of each member:

| Member            | Description                                                                                                                                                                    |
|-------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Instance`        | The instance of the discovered implementation. Don't mutate this instance as it will be used by other processes.                                                               |
| `Type`            | The type of the discovered implementation.                                                                                                                                     |
| `Create()`        | Creates a new instance of the discovered object.                                                                                                                               |
| `IsInitialized`*  | Is `false` when the enum is still in static buildup and `true` if this is finished. This parameter can be used to detect cyclic references during buildup and act accordingly. |
| `GetInstances()`* | Gets an `IEnumerable` over the instances of all discovered implementations.                                                                                                    |

> Members marked with the * are only exposed on the `ImplementationsEnum`, not on the `DiscoveredObject`. 

# Examples

![ImplementationDiscovery usage example](https://raw.githubusercontent.com/Code-Chops/ImplementationDiscovery/master/ImplementationDiscovery.gif)

This generates the following code:

```csharp
// <auto-generated />
#nullable enable
#pragma warning disable CS0109

using CodeChops.ImplementationDiscovery;
using CodeChops.MagicEnums;
using CodeChops.MagicEnums.Core;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

/// <summary>
/// Discovered implementations: <see cref="AnimalEnum"/>.
/// </summary>
public partial record class Animal 
{
	public static IImplementationsEnum<Animal> ImplementationEnum { get; } = new AnimalEnum();
}

/// <summary>
/// Discovered implementations for <see cref="Animal"/>:
/// <list type="table">
/// <item><see cref="global::Cat"/></item>
/// <item><see cref="global::Dog"/></item>
/// </list>
/// </summary>
internal partial record AnimalEnum : ImplementationsEnum<AnimalEnum, Animal>, IInitializable
{
	/// <summary>
	/// <see cref="global::Cat"/>
	/// </summary>
	public static AnimalEnum Cat { get; } = CreateMember(new DiscoveredObject<Animal>(typeof(global::Cat)));

	/// <summary>
	/// <see cref="global::Dog"/>
	/// </summary>
	public static AnimalEnum Dog { get; } = CreateMember(new DiscoveredObject<Animal>(typeof(global::Dog)));

	#region Initialization
	/// <summary>
	/// Is false when the enum is still in static buildup and true if this is finished.
	/// This parameter can be used to detect cyclic references during buildup and act accordingly.
	/// </summary>
	public new static bool IsInitialized { get; }

	static AnimalEnum()
	{
		IsInitialized = true;		
	}
	#endregion
}
```

# Global implementations
By default a global implementations enum will be generated in the root namespace of the assembly with the name `AllDiscoveredImplementations`. 
This enum contains all discovered enums as value and makes it easy to find base enums / interfaces whose implementations should be discovered. 
You can navigate to the concrete implementations using these values.

# Cross-assembly implementations
Implementations can also be discovered across different assemblies, resulting in `proxy eums`. 
This can be done by enabling setting `generateProxies`. If this setting it set to `true` and the base class / interface is implemented in a different assembly, 
the referenced assembly will contain a proxy enum. Imagine the follow situation:
- The package `LightResources` contains an interface `IResource` whose implementations are discoverable. This interface should be implemented by every resource.
- Your website consumes that package and implements `IResource`.

What happens:
- A proxy implementation enum is now created in the project of your website under the `namespace` of `IResource`.
- The name of the `proxy enum` ends with `ProxyEnum`.
- When the resource package is being called you can easily hand over your implementations using dependency injection, so the `LightResources` library can consume it.
