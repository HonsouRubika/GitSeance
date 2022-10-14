using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class TrackedFieldAttribute : Attribute
{
	public string _displayName;

	public TrackedFieldAttribute()
	{
		_displayName = null;
	}

	public TrackedFieldAttribute(string displayName)
	{
		_displayName = displayName;
	}
}