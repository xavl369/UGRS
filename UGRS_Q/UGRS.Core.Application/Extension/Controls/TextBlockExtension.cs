using System.Windows.Controls;

namespace UGRS.Core.Application.Extension.Controls
{
    public static class TextBlockExtension
    {
        public static void ClearControl(this TextBlock pObjTextBlock)
        {
            pObjTextBlock.Text = string.Empty;
        }
    }
}
