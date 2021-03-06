﻿using System;

namespace TabletDriverPlugin.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        public PropertyAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        public string DisplayName { set; get; }
    }
}