using System;

public class CommandArgumentParser
{
    public static T Parse<T>(string argumentName, string stringValue)
    {
        if (typeof(T) == typeof(long))
        {
            try
            {
                return (T)(object)long.Parse(stringValue);
            } catch (FormatException ex)
            {
                throw new Exception(String.Format("Failed to parse {0} as a long", stringValue));
            }
        }
        throw new InvalidOperationException(String.Format("Doesn't support parsing to {0}", typeof(T).Name));
    }
}
