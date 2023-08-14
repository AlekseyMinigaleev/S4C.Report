namespace C4S.ApiHelpers.Helpers.Swagger
{
    /*TODO: разобраться + рефакторинг*/
    public static class RenameSchemaClassesId
    {
        private static readonly string[] _specialWords = new string[4] { "ViewModel", "Body", "Query", "Command" };

        public static string Selector(Type t)
        {
            if (_specialWords.Contains(t.Name) && (object)t.DeclaringType != null)
                return t.DeclaringType!.Name +"." + t.Name;

            var declaringType = t.DeclaringType;
            var text = string.Empty;
            while (declaringType != null)
            {
                text = declaringType.Name + "." + text;
                declaringType = declaringType.DeclaringType;
            }

            if (t.IsGenericType)
            {
                var genericArguments = t.GetGenericArguments();
                var text2 = string.Join(string.Empty, genericArguments.Select((Type x) => x.Name));
                var trimChars = $"`{genericArguments.Length}".ToCharArray();
                return (text + t.Name).TrimEnd(trimChars) + text2;
            }

            return text + t.Name;
        }
    }
}
