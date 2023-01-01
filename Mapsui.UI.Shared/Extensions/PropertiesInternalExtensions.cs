﻿using Mapsui.UI;

namespace Mapsui.UI.Extensions
{
    internal static class PropertiesInternalExtensions
    {
        public static object GetValue(this IPropertiesInternal propertiesInternal, object property)
        {
            propertiesInternal.Properties.TryGetValue(property, out var result);
            return result;
        }

        public static void SetValue(this IPropertiesInternal propertiesInternal, object property, object value)
        {
            propertiesInternal.Properties[property] = value;
        }
    }
}