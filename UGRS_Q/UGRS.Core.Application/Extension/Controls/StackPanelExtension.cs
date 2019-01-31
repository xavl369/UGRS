using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace UGRS.Core.Application.Extension.Controls
{
    public static class StackPanelExtension
    {
        public static void InitializeEvents(this StackPanel pObjStackPanel)
        {
            foreach (FrameworkElement lObjElement in pObjStackPanel.GetChildren().Where(x => x.GetType() == typeof(TextBox) ||
                x.GetType() == typeof(CheckBox) ||
                x.GetType() == typeof(ToggleButton) ||
                x.GetType() == typeof(PasswordBox) ||
                x.GetType() == typeof(Button)))
            {
                lObjElement.KeyDown += new KeyEventHandler(Element_KeyDown);
            }

            //foreach (TextBox lObjTextBox in pObjGrid.GetChildren().OfType<TextBox>())
            //{
            //    lObjTextBox.KeyDown += new KeyEventHandler(Element_KeyDown);
            //}

            //foreach (CheckBox lObjCheckBox in pObjGrid.GetChildren().OfType<CheckBox>())
            //{
            //    lObjCheckBox.KeyDown += new KeyEventHandler(Element_KeyDown);
            //}

            //foreach (ToggleButton lObjToggleButton in pObjGrid.GetChildren().OfType<ToggleButton>())
            //{
            //    lObjToggleButton.KeyDown += new KeyEventHandler(Element_KeyDown);
            //}

            //foreach (PasswordBox lObjPasswordBox in pObjGrid.GetChildren().OfType<PasswordBox>())
            //{
            //    lObjPasswordBox.KeyDown += new KeyEventHandler(Element_KeyDown);
            //}

            foreach (Grid lObjGrid in pObjStackPanel.GetChildren().OfType<Grid>())
            {
                lObjGrid.InitializeEvents();
            }

            foreach (StackPanel lObjStackPanel in pObjStackPanel.GetChildren().OfType<StackPanel>())
            {
                lObjStackPanel.InitializeEvents();
            }
        }

        public static void ClearControl(this StackPanel pObjStackPanel)
        {
            foreach (TextBox lObjTextBox in pObjStackPanel.GetChildren().OfType<TextBox>())
            {
                lObjTextBox.ClearControl();
            }

            foreach (PasswordBox lObjPasswordBox in pObjStackPanel.GetChildren().OfType<PasswordBox>())
            {
                lObjPasswordBox.ClearControl();
            }

            foreach (Grid lObjGrid in pObjStackPanel.GetChildren().OfType<Grid>())
            {
                lObjGrid.ClearControl();
            }

            foreach (StackPanel lObjStackPanel in pObjStackPanel.GetChildren().OfType<StackPanel>())
            {
                lObjStackPanel.ClearControl();
            }
        }

        public static void DisableControl(this StackPanel pObjStackPanel)
        {
            foreach (TextBox lObjTextBox in pObjStackPanel.GetChildren().OfType<TextBox>())
            {
                lObjTextBox.IsEnabled = false;
            }

            foreach (PasswordBox lObjPasswordBox in pObjStackPanel.GetChildren().OfType<PasswordBox>())
            {
                lObjPasswordBox.IsEnabled = false;
            }

            foreach (CheckBox lObjCheckBox in pObjStackPanel.GetChildren().OfType<CheckBox>())
            {
                lObjCheckBox.IsEnabled = false;
            }

            foreach (Button lObjButton in pObjStackPanel.GetChildren().OfType<Button>())
            {
                lObjButton.IsEnabled = false;
            }

            foreach (Grid lObjGrid in pObjStackPanel.GetChildren().OfType<Grid>())
            {
                lObjGrid.DisableControl();
            }

            foreach (StackPanel lObjStackPanel in pObjStackPanel.GetChildren().OfType<StackPanel>())
            {
                lObjStackPanel.DisableControl();
            }
        }

        public static void EnableControl(this StackPanel pObjStackPanel)
        {
            foreach (TextBox lObjTextBox in pObjStackPanel.GetChildren().OfType<TextBox>())
            {
                lObjTextBox.IsEnabled = true;
            }

            foreach (PasswordBox lObjPasswordBox in pObjStackPanel.GetChildren().OfType<PasswordBox>())
            {
                lObjPasswordBox.IsEnabled = true;
            }

            foreach (CheckBox lObjCheckBox in pObjStackPanel.GetChildren().OfType<CheckBox>())
            {
                lObjCheckBox.IsEnabled = true;
            }

            foreach (Button lObjButton in pObjStackPanel.GetChildren().OfType<Button>())
            {
                lObjButton.IsEnabled = true;
            }

            foreach (Grid lObjGrid in pObjStackPanel.GetChildren().OfType<Grid>())
            {
                lObjGrid.EnableControl();
            }

            foreach (StackPanel lObjStackPanel in pObjStackPanel.GetChildren().OfType<StackPanel>())
            {
                lObjStackPanel.EnableControl();
            }
        }

        public static bool Valid(this StackPanel pObjStackPanel)
        {
            bool lBolResult = true;

            foreach (TextBox lObjTextBox in pObjStackPanel.GetChildren().OfType<TextBox>())
            {
                if (!lObjTextBox.ValidRequired())
                {
                    lBolResult = false;
                }
            }

            foreach (ComboBox lObjComboBox in pObjStackPanel.GetChildren().OfType<ComboBox>())
            {
                if (!lObjComboBox.ValidRequired())
                {
                    lBolResult = false;
                }
            }

            foreach (PasswordBox lObjPasswordBox in pObjStackPanel.GetChildren().OfType<PasswordBox>())
            {
                if (!lObjPasswordBox.ValidRequired())
                {
                    lBolResult = false;
                }
            }

            foreach (Grid lObjGrid in pObjStackPanel.GetChildren().OfType<Grid>())
            {
                lObjGrid.Valid();
            }

            foreach (StackPanel lObjStackPanel in pObjStackPanel.GetChildren().OfType<StackPanel>())
            {
                lObjStackPanel.Valid();
            }

            return lBolResult;
        }

        public static List<FrameworkElement> GetChildren(this StackPanel pObjStackPanel)
        {
            return pObjStackPanel.Children.Count > 0 ? pObjStackPanel.Children.Cast<FrameworkElement>().ToList() : new List<FrameworkElement>();
        }

        private static void Element_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            if (pObjArgs.Key == Key.Enter)
            {
                FocusNavigationDirection lObjFocusDirection = FocusNavigationDirection.Next;
                TraversalRequest lObjRequest = new TraversalRequest(lObjFocusDirection);
                UIElement lObjElementWithFocus = Keyboard.FocusedElement as UIElement;

                if (lObjElementWithFocus != null)
                {
                    if (lObjElementWithFocus.MoveFocus(lObjRequest)) pObjArgs.Handled = true;
                }
            }
        }
    }
}
