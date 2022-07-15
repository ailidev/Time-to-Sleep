#if UNITY_EDITOR
using UnityEngine;
using System;

namespace Aili.ReadOnlyField
{
    /// <summary>
    /// Read Only attribute
    /// Attribute to mark read only properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute { }
}
#endif
