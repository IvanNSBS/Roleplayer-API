using System;
using UnityEngine;

namespace INUlib.BackendToolkit.Audio
{
    /// <summary>
    /// Attribute that tells which object is needed from the ServiceLocator instance
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class LocateAttribute : Attribute
    {
    }
}