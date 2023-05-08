public class FieldNameAttribute : Attribute
{
    public FieldNameAttribute(string name)
    {
        Value = name;
    }

    public readonly string Value;
}