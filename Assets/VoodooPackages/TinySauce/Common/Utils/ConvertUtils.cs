namespace Voodoo.Sauce.Common.Utils
{
    public static class ConvertUtils
    {
        private const string TAG = "ConvertUtils";
        public static int ByteToMegaByte(float size)
        {
            return (int)(size / 1048576f);
        }
    }
}