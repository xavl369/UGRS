using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace UGRS.Core.Application.Extension.Controls
{
    public static class GridExtension
    {
        public static void InitializeEvents(this Grid pObjGrid)
        {
            foreach (FrameworkElement lObjElement in pObjGrid.GetChildren().Where(x=> x.GetType() == typeof(TextBox) || 
                x.GetType() == typeof(CheckBox) ||
                x.GetType()== typeof(ToggleButton) ||
                x.GetType() == typeof(PasswordBox) || 
                x.GetType() == typeof(Button)))
            {
                lObjElement.KeyDown += new KeyEventHandler(Element_KeyDown);
            }

            foreach (Grid lObjGrid in pObjGrid.GetChildren().OfType<Grid>())
            {
                lObjGrid.InitializeEvents();
            }

            foreach (StackPanel lObjStackPanel in pObjGrid.GetChildren().OfType<StackPanel>())
            {
                lObjStackPanel.InitializeEvents();
            }
        }

        public static bool IsBlocked(this Grid pObjGrid)
        {
            return (bool)pObjGrid.Dispatcher.Invoke(new Func<bool>(() =>
            {
                Grid lObjGridLoading = GetLoadingGrid(pObjGrid);
                return lObjGridLoading != null && lObjGridLoading.Visibility == Visibility.Visible;

            }));
        }

        public static void BlockUI(this Grid pObjGrid, string pStrMessage = "Por favor espere...")
        {
            try
            {
                pObjGrid.Dispatcher.Invoke((Action)delegate
                {
                    Grid lObjGridLoading = GetLoadingGrid(pObjGrid);
                    if (lObjGridLoading != null)
                    {
                        lObjGridLoading.Visibility = Visibility.Visible;
                    }

                    pObjGrid.SetWaitMessage(pStrMessage);

                });
            }
            catch (Exception)
            {
                //Ignore
            }
        }

        public static void UnblockUI(this Grid pObjGrid)
        {
            try
            {
                pObjGrid.Dispatcher.Invoke((Action)delegate
                {
                    Grid lObjGridLoading = GetLoadingGrid(pObjGrid);

                    if (lObjGridLoading != null)
                    {
                        lObjGridLoading.Visibility = Visibility.Collapsed;
                    }
                });
            }
            catch
            {
                //Ignore
            }
        }

        public static void BlockUI(this Grid pObjGrid, bool pBolBackground)
        {
            try
            {
                pObjGrid.Dispatcher.Invoke((Action)delegate
                {
                    Grid lObjGridLoading = GetLoadingGrid(pObjGrid);

                    if (lObjGridLoading != null)
                    {
                        lObjGridLoading.Visibility = Visibility.Visible;

                        if (!pBolBackground)
                        {
                            lObjGridLoading.Opacity = 0;

                            foreach (FrameworkElement lObjElement in lObjGridLoading.GetChildren().OfType<Grid>())
                            {
                                lObjElement.Opacity = 1;
                            }
                        }
                    }

                    pObjGrid.SetWaitMessage("Por favor espere...");

                });
            }
            catch (Exception)
            {
                //Ignore
            }
        }

        public static void SetWaitMessage(this Grid pObjGrid, string pStrMessage)
        {
            try
            {
                pObjGrid.Dispatcher.Invoke((Action)delegate
                {
                    Grid lObjGridLoading = GetLoadingGrid(pObjGrid);

                    if (lObjGridLoading != null)
                    {
                        foreach (Grid lObjElement in lObjGridLoading.GetChildren().OfType<Grid>())
                        {
                            foreach (TextBlock lObjTextBlock in lObjElement.GetChildren().OfType<TextBlock>())
                            {
                                lObjTextBlock.Text = pStrMessage;
                            }
                        }
                    }

                });
            }
            catch (Exception)
            {
                //Ignore
            }
        }

        public static void ClearControl(this Grid pObjGrid)
        {
            foreach (TextBox lObjTextBox in pObjGrid.GetChildren().OfType<TextBox>())
            {
                lObjTextBox.ClearControl();
            }

            foreach (PasswordBox lObjPasswordBox in pObjGrid.GetChildren().OfType<PasswordBox>())
            {
                lObjPasswordBox.ClearControl();
            }

            foreach (Grid lObjGrid in pObjGrid.GetChildren().OfType<Grid>())
            {
                lObjGrid.ClearControl();
            }

            foreach (StackPanel lObjStackPanel in pObjGrid.GetChildren().OfType<StackPanel>())
            {
                lObjStackPanel.ClearControl();
            }
        }

        public static void DisableControl(this Grid pObjGrid)
        {
            foreach (TextBox lObjTextBox in pObjGrid.GetChildren().OfType<TextBox>())
            {
                lObjTextBox.IsEnabled = false;
            }

            foreach (PasswordBox lObjPasswordBox in pObjGrid.GetChildren().OfType<PasswordBox>())
            {
                lObjPasswordBox.IsEnabled = false;
            }

            foreach (CheckBox lObjCheckBox in pObjGrid.GetChildren().OfType<CheckBox>())
            {
                lObjCheckBox.IsEnabled = false;
            }

            foreach (Button lObjButton in pObjGrid.GetChildren().OfType<Button>())
            {
                lObjButton.IsEnabled = false;
            }

            foreach (Grid lObjGrid in pObjGrid.GetChildren().OfType<Grid>())
            {
                lObjGrid.DisableControl();
            }

            foreach (StackPanel lObjStackPanel in pObjGrid.GetChildren().OfType<StackPanel>())
            {
                lObjStackPanel.DisableControl();
            }
        }

        public static void EnableControl(this Grid pObjGrid)
        {
            foreach (TextBox lObjTextBox in pObjGrid.GetChildren().OfType<TextBox>())
            {
                lObjTextBox.IsEnabled = true;
            }

            foreach (PasswordBox lObjPasswordBox in pObjGrid.GetChildren().OfType<PasswordBox>())
            {
                lObjPasswordBox.IsEnabled = true;
            }

            foreach (CheckBox lObjCheckBox in pObjGrid.GetChildren().OfType<CheckBox>())
            {
                lObjCheckBox.IsEnabled = true;
            }

            foreach (Button lObjButton in pObjGrid.GetChildren().OfType<Button>())
            {
                lObjButton.IsEnabled = true;
            }

            foreach (Grid lObjGrid in pObjGrid.GetChildren().OfType<Grid>())
            {
                lObjGrid.EnableControl();
            }

            foreach (StackPanel lObjStackPanel in pObjGrid.GetChildren().OfType<StackPanel>())
            {
                lObjStackPanel.EnableControl();
            }
        }

        public static bool Valid(this Grid pObjGrid)
        {
            bool lBolResult = true;

            foreach (TextBox lObjTextBox in pObjGrid.GetChildren().OfType<TextBox>())
            {
                if (!lObjTextBox.ValidRequired())
                {
                    lBolResult = false;
                }
            }

            foreach (ComboBox lObjComboBox in pObjGrid.GetChildren().OfType<ComboBox>())
            {
                if (!lObjComboBox.ValidRequired())
                {
                    lBolResult = false;
                }
            }

            foreach (PasswordBox lObjPasswordBox in pObjGrid.GetChildren().OfType<PasswordBox>())
            {
                if (!lObjPasswordBox.ValidRequired())
                {
                    lBolResult = false;
                }
            }

            foreach (Grid lObjGrid in pObjGrid.GetChildren().OfType<Grid>())
            {
                lObjGrid.Valid();
            }

            foreach (StackPanel lObjStackPanel in pObjGrid.GetChildren().OfType<StackPanel>())
            {
                lObjStackPanel.Valid();
            }

            return lBolResult;
        }

        public static IEnumerable<FrameworkElement> GetChildren(this Grid pObjGrid)
        {
            return pObjGrid.Children.Cast<FrameworkElement>();
        }

        #region Internal Methods

        private static Grid GetLoadingGrid(this Grid pObjGrid)
        {
            Grid lObjLoadingGrid = null;

            if (!ExistsLoadingGrid(pObjGrid))
            {
                lObjLoadingGrid = pObjGrid.GetInternalLoadingGrid();

                if (pObjGrid.ColumnDefinitions.Count > 0)
                {
                    lObjLoadingGrid.SetValue(Grid.ColumnProperty, 0);
                    lObjLoadingGrid.SetValue(Grid.ColumnSpanProperty, pObjGrid.ColumnDefinitions.Count);
                }

                if (pObjGrid.RowDefinitions.Count > 0)
                {
                    lObjLoadingGrid.SetValue(Grid.RowProperty, 0);
                    lObjLoadingGrid.SetValue(Grid.RowSpanProperty, pObjGrid.RowDefinitions.Count);
                }
                 
                pObjGrid.Children.Add(lObjLoadingGrid);
            }
            else
            {
                lObjLoadingGrid = pObjGrid.GetChildren().OfType<Grid>().FirstOrDefault(x => x.Name == "grdLoading");
            }

            return lObjLoadingGrid;
        }

        private static bool ExistsLoadingGrid(this Grid pObjGrid)
        {
            return pObjGrid.GetChildren().OfType<Grid>().Where(x => x.Name == "grdLoading").Count() > 0;
        }

        private static Grid GetInternalLoadingGrid(this Grid pObjGrid)
        {
            Grid lObjGridContainer = new Grid();
            lObjGridContainer.Name = "grdLoading";
            lObjGridContainer.Visibility = Visibility.Hidden;
            
            Grid lObjGridBackground = new Grid();
            lObjGridBackground.Name = "grdBackground";
            lObjGridBackground.Background = Brushes.White;
            lObjGridBackground.HorizontalAlignment = HorizontalAlignment.Stretch;
            lObjGridBackground.VerticalAlignment = VerticalAlignment.Stretch;
            lObjGridBackground.Opacity = 0.9;

            lObjGridBackground.Children.Add(pObjGrid.GetInternalLoadingProgressBar());
            lObjGridBackground.Children.Add(pObjGrid.GetInternalLoadingTextBlock("Por favor espere..."));

            lObjGridContainer.Children.Add(lObjGridBackground);
            return lObjGridContainer;
        }

        private static ProgressBar GetInternalLoadingProgressBar(this Grid pObjGrid)
        {
            ProgressBar lObjProgressBar = new ProgressBar();
            lObjProgressBar.Name = "pbLoading";
            lObjProgressBar.Style = System.Windows.Application.Current.FindResource("MaterialDesignCircularProgressBar") as Style;
            lObjProgressBar.Visibility = Visibility.Visible;
            lObjProgressBar.Foreground = ((Brush)new BrushConverter().ConvertFrom("#FFFCB913"));
            lObjProgressBar.Opacity = 1;
            lObjProgressBar.Value = 0;
            lObjProgressBar.IsIndeterminate = true;
            lObjProgressBar.Width = pObjGrid.GetInternalLoadingSize();
            lObjProgressBar.Height = pObjGrid.GetInternalLoadingSize();
            lObjProgressBar.Margin = new Thickness(0, (pObjGrid.GetInternalLoadingSize() / 2 * -1), 0, 0);
            lObjProgressBar.VerticalAlignment = VerticalAlignment.Center;
            lObjProgressBar.HorizontalAlignment = HorizontalAlignment.Center;

            return lObjProgressBar;
        }

        private static TextBlock GetInternalLoadingTextBlock(this Grid pObjGrid, string pStrText)
        {
            TextBlock lObjTextBlock = new TextBlock();
            lObjTextBlock.Name = "tbLoading";
            lObjTextBlock.Text = pStrText;
            lObjTextBlock.Margin = new Thickness(0, pObjGrid.GetInternalLoadingSize(), 0, 0);
            lObjTextBlock.VerticalAlignment = VerticalAlignment.Center;
            lObjTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            lObjTextBlock.Foreground = ((Brush)new BrushConverter().ConvertFrom("#333"));

            return lObjTextBlock;
        }

        private static int GetInternalLoadingSize(this Grid pObjGrid)
        {
            //Valid if has it enough size
            if (pObjGrid.ActualHeight > 100 && pObjGrid.ActualWidth > 100)
            {
                //Return maximum size
                return 60;
            }
            else
            {
                //Get the small size
                int lIntSmallSize = (int)(pObjGrid.ActualHeight > pObjGrid.ActualWidth ? pObjGrid.ActualWidth : pObjGrid.ActualHeight);
                return lIntSmallSize - 20;
            }
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

        #endregion
    }
}
