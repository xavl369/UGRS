using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UGRS.Core.Application.Extension.Controls
{
    public static class UserControlExtension
    {
        public static T FindParent<T>(this DependencyObject lUnkDependencyObject) where T : DependencyObject
        {
            var lUnkParent = VisualTreeHelper.GetParent(lUnkDependencyObject);

            if (lUnkParent == null) return null;

            var lUnkGrandparent = lUnkParent as T;

            return lUnkGrandparent ?? FindParent<T>(lUnkParent);
        }

        public static T FindChild<T>(this DependencyObject lUnkDependencyObject) where T : DependencyObject
        {
            var lUnkChild = VisualTreeHelper.GetChild(lUnkDependencyObject, 0);

            if (lUnkChild == null) return null;

            var lUnkGrandchild = lUnkChild as T;

            return lUnkGrandchild ?? FindChild<T>(lUnkChild);
        }

        public static Window GetParent(this UserControl pObjUsercontrol)
        {
            return Window.GetWindow(pObjUsercontrol);
        }
    }
}
