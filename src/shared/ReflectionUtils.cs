using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Scalider
{

    internal static class ReflectionUtils
    {

        public static IEnumerable<Type> GetExportedTypes(Assembly assembly)
        {
            if (assembly == null)
                return Array.Empty<Type>();

            // Try to retrieve all the assembly exported types
            Type[] exportedTypes;
            try
            {
                exportedTypes = assembly.GetExportedTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                exportedTypes = e.Types;
            }

            // Done
            return exportedTypes.Where(t => t != null).ToArray();
        }

        public static bool IsAssignableFromGenericType(Type genericType, Type type)
        {
            if (genericType.IsAssignableFrom(type))
            {
                // The type to test is assignable to the expected type
                return true;
            }

            if (!genericType.IsGenericTypeDefinition)
            {
                // The expected type isn't a type with a generic definition
                return false;
            }

            // Retrieve a collection containing all the types we need to validate and determine if any
            // is assignable to the expected type
            var typesToExplore = genericType.IsInterface 
                ? type.GetInterfaces().Where(t => t != null)
                : GetAllInheritedTypes(type, false);

            return typesToExplore.Any(t => t.IsGenericType &&
                                           genericType.IsAssignableFrom(t.GetGenericTypeDefinition()));
        }

        public static bool IsInstanceOfGenericType(Type genericType, object obj)
        {
            if (genericType.IsInstanceOfType(obj))
            {
                // The object is an instance of the expected type
                return true;
            }

            if (!genericType.IsGenericTypeDefinition)
            {
                // The expected type isn't a type with a generic definition
                return false;
            }

            // Retrieve a collection containing all the types we need to validate and determine if any
            // is assignable to the expected type
            var objType = obj.GetType();
            var typesToExplore = genericType.IsInterface 
                ? objType.GetInterfaces().Where(t => t != null)
                : GetAllInheritedTypes(objType, false);
            
            return typesToExplore.Any(t => t.IsGenericType &&
                                           // genericType.IsAssignableFrom(t) &&
                                           genericType.IsAssignableFrom(t.GetGenericTypeDefinition()));
        }
        
        private static IEnumerable<Type> GetAllInheritedTypes(Type type, bool includingSelf)
        {
            var result = new Stack<Type>();
            if (includingSelf)
                result.Push(type);

            // Navigate through all the base types until we reach the root
            var baseType = type.BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                result.Push(baseType);
                baseType = baseType.BaseType;
            }
            
            // Done
            return result.ToArray();
        }
        
    }
    
}