# Implementation discovery
Provides easy-accessible, design-time information about implementations throughout your code. Just place an attribute on a specific base class or interface whose implementations you want to discover. A source generator will create an eum at design time that contains instances of all implementations.

## Advantages
- All implementations of a specific class or interface are centralized in one place.
- You have a simple and navigable overview over what is implemented.
- No need to use slow reflection to collect implementations. This improves startup time of your app.
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
> - `AnimalBase` ->  `AnimalEnum`
> - `IVehicle` -> `VehicleEnum`

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
This object contains the type, a globally shared instance, and a factory of the discovered implementation. 
It can be used to retrieve a shared or a new instance. It can implicitly be casted to the the concrete type of the implementation or the base class / interface.

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

This API also offers some extra methods and classes on the enum (`ImplementationsEnum`) and on the value (`DiscoveredObject`) of each member:

| Member            | Description                                                                                                                                                                    |
|-------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Instance`        | The instance of the discovered implementation. Don't mutate this instance as it will be used by other processes.                                                               |
| `Type`            | The type of the discovered implementation.                                                                                                                                     |
| `Create()`        | Creates a new instance of the discovered object.                                                                                                                               |
| `IsInitialized`*  | Is `false` when the enum is still in static buildup and `true` if this is finished. This parameter can be used to detect cyclic references during buildup and act accordingly. |
| `GetInstances()`* | Gets an `IEnumerable` over the instances of all discovered implementations.                                                                                                    |

> Members marked with the * are only exposed on the `ImplementationsEnum`, not on the `DiscoveredObject`. 

# Examples

> The [LightResources library](https://github.com/Code-Chops/LightResources/) makes use of this library to collect all the resources (and their localizations).

> The [Geometry library](https://github.com/Code-Chops/Geometry/) makes use of this library to collect every `StrictDirection` implementation under one enum.

> The [Blame game engine library](https://github.com/Code-Chops/Blame/) makes use of this library to discover implemented GameObjects.

# Global implementations
By default a global implementations enum will be generated in the root namespace of the assembly. 
This enum contains all discovered enums as value. 
This way you can easily find base enums / interfaces whose implementations should be discovered. 
From there, you can discover the concrete implementations.

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