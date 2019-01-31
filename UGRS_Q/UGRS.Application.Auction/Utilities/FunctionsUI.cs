using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UGRS.Core.Application.Forms.Base;

namespace UGRS.Application.Auctions
{
    public class FunctionsUI
    {
        /// <summary>
        /// Muestra dialogo de busqueda y regresa el objeto seleccionado.
        /// </summary>
        public static object ShowWindowDialog(UserControl pUCUserControl, Window pWindow)
        {
            WindowDialog lObjWindow = new WindowDialog();
            lObjWindow.Owner = Window.GetWindow(pWindow);
            //lobjWindow.SizeToContent = System.Windows.SizeToContent.Height;
            //lObjWindow.Width = 600;
            //lObjWindow.SizeToContent = System.Windows.SizeToContent.Width;
            lObjWindow.Width = pUCUserControl.MinWidth;
            lObjWindow.Height = 400;
            lObjWindow.grContent.Children.Add(pUCUserControl);

            object lObjObject = new object();
            if (lObjWindow.ShowDialog() == false)
            {
                lObjObject = lObjWindow.gObject;
            }
            return lObjObject;
        }


        /// <summary>
        /// Coloca los controles a su estado original.
        /// </summary>
        public static Grid ClearControls(Grid pObjGrid)
        {
            UIElementCollection lObjElements = pObjGrid.Children;
            List<FrameworkElement> lLstObjElements = lObjElements.Cast<FrameworkElement>().ToList();

            var lLstObjControls = lLstObjElements.OfType<TextBox>();
            foreach (TextBox lObjControl in lLstObjControls)
            {
                lObjControl.Text = string.Empty;
                lObjControl.BorderBrush = Brushes.Black;
            }
            return pObjGrid;
        }

        /// <summary>
        ///  Valida los controles que se encuentran en blanco.
        /// </summary>
        public static Tuple<bool, Grid> ValidateControls(Grid pObjGrid)
        {
            bool lBolValidate = true;

            UIElementCollection lObjElements = pObjGrid.Children;
            List<FrameworkElement> lLstObjElements = lObjElements.Cast<FrameworkElement>().ToList();

            var lLstObjControls = lLstObjElements.OfType<TextBox>();
            foreach (TextBox lObjControl in lLstObjControls)
            {
                if (string.IsNullOrEmpty(lObjControl.Text))
                {
                    lObjControl.BorderBrush = Brushes.Red;
                    lBolValidate = false;
                }
            }
            return Tuple.Create(lBolValidate, pObjGrid);
        }

    }
}
