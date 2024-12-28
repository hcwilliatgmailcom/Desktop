namespace Desktop.Context
{
    internal class ReferenceAttribute : Attribute
    {
        public Type ReferenceType { get; }
        public string ReferenceProperty { get; }

        public ReferenceAttribute(Type referenceType, string referenceProperty)
        {
            ReferenceType = referenceType;
            ReferenceProperty = referenceProperty;
        }
    }
}