using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Architect.DomainModeling;

namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// Has a factory to create objects of type <typeparamref name="TSelf"/>.
/// </summary>
/// <typeparam name="TSelf">The type to be created.</typeparam>
public interface ICreatable<out TSelf>
    where TSelf : ICreatable<TSelf>, IDomainObject
{
    static abstract TSelf Create();
}

/// <summary>
/// Has a factory to create objects of type <typeparamref name="TSelf"/>
/// by providing parameter <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="TSelf">The type to be created.</typeparam>
public interface ICreatable<out TSelf, in T>
    where TSelf : ICreatable<TSelf, T>, IDomainObject
{
    static abstract TSelf Create(T parameter);
}

/// <summary>
/// Has a factory to create objects of type <typeparamref name="TSelf"/>
/// by providing parameters <typeparamref name="T1"/> and <typeparamref name="T2"/>.
/// </summary>
/// <typeparam name="TSelf">The type to be created.</typeparam>
public interface ICreatable<out TSelf, in T1, in T2>
    where TSelf : ICreatable<TSelf, T1, T2>, IDomainObject
{
    static abstract TSelf Create(T1 parameter1, T2 parameter2);
}

/// <summary>
/// Has a factory to create objects of type <typeparamref name="TSelf"/>
/// by providing parameters <typeparamref name="T1"/>, <typeparamref name="T2"/> and <typeparamref name="T3"/>.
/// </summary>
/// <typeparam name="TSelf">The type to be created.</typeparam>
public interface ICreatable<out TSelf, in T1, in T2, in T3>
    where TSelf : ICreatable<TSelf, T1, T2, T3>, IDomainObject
{
    static abstract TSelf Create(T1 parameter1, T2 parameter2, T3 parameter3);
}

/// <summary>
/// Has a factory to create objects of type <typeparamref name="TSelf"/>
/// by providing parameters <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/> and <typeparamref name="T4"/>.
/// </summary>
/// <typeparam name="TSelf">The type to be created.</typeparam>
public interface ICreatable<out TSelf, in T1, in T2, in T3, in T4>
    where TSelf : ICreatable<TSelf, T1, T2, T3, T4>, IDomainObject
{
    static abstract TSelf Create(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4);
}

/// <summary>
/// Has a factory to create objects of type <typeparamref name="TSelf"/>
/// by providing parameters <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/> and <typeparamref name="T5"/>.
/// </summary>
/// <typeparam name="TSelf">The type to be created.</typeparam>
public interface ICreatable<out TSelf, in T1, in T2, in T3, in T4, in T5>
    where TSelf : ICreatable<TSelf, T1, T2, T3, T4, T5>, IDomainObject
{
    static abstract TSelf Create(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5);
}