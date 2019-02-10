namespace TypescriptGenerator
{
    public static class Helpers
    {
        public static string Indentation(int indentationLevel)
        {
            string indentation = "";
            for (var i = 0; i < indentationLevel; i++)
            {
                indentation += "\t";
            }
            return indentation;
        }
    }
}
