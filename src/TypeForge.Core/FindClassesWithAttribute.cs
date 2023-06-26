using System.Reflection;

namespace TypeForge.Core;

public static class FindClassesWithAttribute
{
    public static IEnumerable<Type> FindClassesWithAttributeOfType<TAttribute>(Assembly assembly)
        where TAttribute : Attribute
        => assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(TAttribute), true).Any());

    public static IEnumerable<Type> FindClassesWithAttributeOfType<TAttribute>(IEnumerable<Assembly> assemblies)
        where TAttribute : Attribute
        => assemblies.SelectMany(FindClassesWithAttributeOfType<TAttribute>);
}