using Xamarin.Forms;

namespace Wizard.Mobile.ViewModels
{
    public static class ColorHelper
    {
        private const double RBG = 255;

        private static Color ColorHelperRGB(int colorHex)
        {
            double b = (byte)colorHex / RBG;
            double g = (byte)(colorHex >> 08) / RBG;
            double r = (byte)(colorHex >> 16) / RBG;

            return new Color(r, g, b);
        }

        private static Color ColorHelperRGBA(int colorHex)
        {
            double a = (byte)colorHex / RBG;
            double b = (byte)(colorHex >> 08) / RBG;
            double g = (byte)(colorHex >> 16) / RBG;
            double r = (byte)(colorHex >> 24) / RBG;

            return new Color(r, g, b, a);
        }

        public static readonly Color DEFAULT_COLOR = ColorHelperRGB(0x2196F3);
    }
}