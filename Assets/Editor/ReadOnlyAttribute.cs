using System;
using System.ComponentModel.Composition;
using UnityEngine;


namespace CustomAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ReadOnlyAttribute : PropertyAttribute { }
}
