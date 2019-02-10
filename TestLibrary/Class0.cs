using TypescriptGeneratorCommons;

namespace TestLibrary
{
    /// <summary>
    /// idid
    /// </summary>
    [TypescriptOptional]
    public class Class0
    {
        [TypescriptMoreType(typeof(string), typeof(bool))]
        public int PropX { get; set; }

        public Class0 PropY { get; set; }
    }
}
