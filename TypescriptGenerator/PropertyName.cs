namespace TypescriptGenerator
{
    public class PropertyName
    {
        public PropertyName(string name, Documentor documentor)
        {
            SourceName = name;
            if (SourceName.Contains(":") || SourceName.Contains("."))
            {
                documentor.AddDocumentationLine($"Property name has been quoted because of special characters in JsonProperty attribute.");
                Name = $"\"{SourceName}\"";
            }
            else
            {
                Name = SourceName;
            }
        }
        public string Name { get; set; }
        public string SourceName { get; set; }
    }
}
