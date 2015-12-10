namespace System.IO
{
    internal static class PathEx
    {
        public static bool IsDirectorySeparator(this char separator)
        {
            return (Path.DirectorySeparatorChar == separator || Path.AltDirectorySeparatorChar == separator);
        }
    }
}
