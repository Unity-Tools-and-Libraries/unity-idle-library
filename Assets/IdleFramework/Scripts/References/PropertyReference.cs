using BreakInfinity;

namespace IdleFramework
{
    public interface PropertyReference : ValueContainer
    {
        
    }

    public static class PropertyReferenceExtension
    {
        public static string[] TokenizePropertyString(this PropertyReference propRef, string propertyString)
        {
            return propertyString != null ? propertyString.Split('.') : new string[] { };
        }
    }


}