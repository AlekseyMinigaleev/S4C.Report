namespace C4S.ApiHelpers.Helpers.Swagger
{
    public static class RenameSchemaClassesId
    {
        private static readonly string[] _specialWords = new string[4] { "ViewModel", "Body", "Query", "Command" };

        public static string Selector(Type t)
        {
            if (_specialWords.Contains<string>(t.Name) && (object)t.DeclaringType != null)
            {
                return t.DeclaringType!.Name + t.Name;
            }

            Type declaringType = t.DeclaringType;
            string text = string.Empty;
            while (declaringType != null)
            {
                text = declaringType.Name + "." + text;
                declaringType = declaringType.DeclaringType;
            }

            if (t.IsGenericType)
            {
                Type[] genericArguments = t.GetGenericArguments();
                string text2 = string.Join(string.Empty, genericArguments.Select((Type x) => x.Name));
                char[] trimChars = $"`{genericArguments.Length}".ToCharArray();
                return (text + t.Name).TrimEnd(trimChars) + text2;
            }

            return text + t.Name;
        }
    }
}
