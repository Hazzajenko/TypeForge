using System.Reflection;
using TypeForge.Core.Attributes;

namespace TypeForge.Core.Services;

public class AttributeService
{
    private readonly Assembly _assembly;
    public AttributeService(Assembly assembly)
    {
        _assembly = assembly;
    }

    public void WriteFilesWithAttribute()
    {
        
        var types = FindClassesWithAttribute.FindClassesWithAttributeOfType<TypeForgeAttribute>(
            _assembly
        );
        if (types.Any())
        {
            Console.WriteLine($"Found types: {types.Count()}");

            foreach (var type in types)
            {
                Console.WriteLine($"Type: {type.Name}");
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    Console.WriteLine($"Property: {property.Name}");
                }

                var attribute = type.GetCustomAttribute<TypeForgeAttribute>();
                if (attribute is null)
                {
                    throw new Exception("Attribute is null");
                }
                Console.WriteLine($"AttributeTypeId: {attribute.TypeId}");
                Console.WriteLine($"AttributePath: {attribute.Path}");

                var location = attribute.GetCurrentLocation();
                Console.WriteLine($"AttributeLocation: {location}");
            }
        }
    }
}