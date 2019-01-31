using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.DTO.System;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.Factories;
using WPF.MDI;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Auctions.DTO.Session;
using UGRS.Core.Application.Forms.Base;
using System.Configuration;
using System.Linq;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UGRS.xaml
    /// </summary>
    public partial class MainAuction : Window
    {
        #region Attributes

        SystemServicesFactory mObjServiceFactory = new SystemServicesFactory();
        long mLonId = 0; //bandera si se requiere modificar/eliminar
        bool mBolCanSelected = true;
        bool mBolLoaded = false;

        #endregion

        #region Contructor

        public MainAuction()
        {
            InitializeComponent();

            try
            {
                this.WindowState = WindowState.Maximized;
                //var lObjuri = new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\Images\\logo.png");
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this);
            }
        }

        #endregion

        #region Events

        #region Windows

        private void canvasTop_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                DragMove();
            }
            else
            {
                this.Top = e.GetPosition(this).Y;
                this.WindowState = WindowState.Normal;
                DragMove();
            }
            if (e.ClickCount == 2)
            {
                if (this.WindowState == WindowState.Normal)
                {
                    this.WindowState = WindowState.Maximized;
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                }
            }
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr lObjWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(lObjWindowHandle).AddHook(new HwndSourceHook(WindowsUtility.WindowProc));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowLogin();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                btnMaximizar.Visibility = Visibility.Visible;
                btnRestaurar.Visibility = Visibility.Collapsed;
            }
            if (this.WindowState == WindowState.Maximized)
            {
                btnMaximizar.Visibility = Visibility.Collapsed;
                btnRestaurar.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Buttons

        //private void MaximizeApp()
        //{
        //    MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        //    MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
        //    this.WindowState = WindowState.Maximized;
        //}

        private void btnMaximizar_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            if (CustomMessageBox.ShowOption("Salir","¿Desea salir del sistema de subasta?", "Si", "No", "") == true)
            {
               this.Close();
            }
          
        }

        private void btnMinimizar_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMaximizar_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            btnMaximizar.Visibility = Visibility.Collapsed;
            btnRestaurar.Visibility = Visibility.Visible;
        }

        private void btnRestaurar_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            btnRestaurar.Visibility = Visibility.Collapsed;
            btnMaximizar.Visibility = Visibility.Visible;
        }

        private void btnLateral_Click(object sender, RoutedEventArgs e)
        {
            GridSpliter.Visibility = Visibility.Collapsed;
            SkpMenu.Visibility = Visibility.Collapsed;
            menuLateral.Width = new GridLength(0, GridUnitType.Pixel);
            btnMenu.Visibility = Visibility.Visible;
            btnPin.Visibility = Visibility.Collapsed;
            menu.Width = new GridLength(15, GridUnitType.Pixel);
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            SkpMenu.Visibility = Visibility.Visible;
            GridSpliter.Visibility = Visibility.Visible;
            menuLateral.Width = new GridLength(300, GridUnitType.Pixel);
            menu.Width = new GridLength(0, GridUnitType.Pixel);
            btnPin.Visibility = Visibility.Visible;
         
        }

        #endregion

        #region List

        private void ListItemClick(object sender, RoutedEventArgs e)
        {
            ListBoxItem lObjListBox = sender as ListBoxItem;
            string lStrTittle = lObjListBox.Content.ToString();

            if (!string.IsNullOrEmpty(lObjListBox.Name))
            {
                try
                {
                    object lObjElement = Activator.CreateInstance(Type.GetType("UGRS.Application.Auctions." + lObjListBox.Name));
                    lObjListBox.IsSelected = false;
                    LoadContainerChild(lObjElement, lObjListBox);
                }
                catch (Exception lObjException)
                {
                    CustomMessageBox.Show("Error", lObjException.Message, this);
                }
            }
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            ListBoxItem lObjListBox = sender as ListBoxItem;

            string lStrTittle = lObjListBox.Content.ToString();
            if (!string.IsNullOrEmpty(lObjListBox.Name))
            {
                object lObjElement = Activator.CreateInstance(Type.GetType("UGRS.Application.Auctions." + lObjListBox.Name));
                LoadContainerChild(lObjElement, lObjListBox);
            }
        }

        #endregion

        #endregion

        #region Methods

        private IList<MenuDTO> GetMenu()
        {
            grdMenu.BlockUI(false);
            IList<MenuDTO> lLstObjResult = null;

            try
            {
                SecurityServicesFactory mObjSecurityServiceFactory = new SecurityServicesFactory();
                SessionDTO lObjSession = StaticSessionUtility.GetCurrentSession() as SessionDTO;

                if (IsActiveSecurity() || !IsTestMode())
                {
                    lLstObjResult = mObjSecurityServiceFactory.GetPermissionService().GetSystemMenu(lObjSession.Id);
                }
                else
                {
                    lLstObjResult = mObjSecurityServiceFactory.GetPermissionService().GetTestSystemMenu();
                }
            }
            catch (Exception lObjException)
            {
                this.Dispatcher.Invoke(() =>
                {
                    CustomMessageBox.Show("Error", lObjException.Message);
                });
            }
            finally
            {
                grdMenu.UnblockUI();
            }

            return lLstObjResult;
        }

        public void LoadMenu(IList<MenuDTO> pLstModules)
        {
            //Delegate to main thread
            this.Dispatcher.Invoke((Action)delegate
            {
                while (SkpMenu.Children.Count > 0)
                {
                    SkpMenu.Children.RemoveAt(SkpMenu.Children.Count - 1);
                }

                foreach (MenuDTO lObjModule in pLstModules)
                {
                    Expander lObjExpander = new Expander
                    {
                        FontSize = 14,
                        Header = lObjModule.Name,
                        FontWeight = FontWeights.Bold,
                        
                        Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#EEEEEE"))
                    };

                    ListBox lObjListbox = new ListBox
                    {
                        FontSize = 14,
                        Margin = new Thickness(30, 0, 0, 0),
                       
                        Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#EEEEEE"))
                    };

                    foreach (MenuDTO lObjSection in lObjModule.Children)
                    {
                        ListBoxItem lObjListItem = null;

                        try
                        {
                            lObjListItem = new ListBoxItem
                            {
                                Content = lObjSection.Name,
                                Name = lObjSection.Path,
                                FontWeight = FontWeights.Normal

                            };
                            
                        }
                        catch
                        {
                            lObjListItem = new ListBoxItem
                            {
                                Content = lObjSection.Name,
                                Name = "UCError",
                                FontWeight = FontWeights.Normal

                            };
                        }

                        lObjListItem.PreviewMouseLeftButtonDown += ListItemClick;
                        lObjListbox.Items.Add(lObjListItem);
                    }

                    StackPanel lObjStackPanel = new StackPanel();

                    lObjStackPanel.Children.Add(lObjListbox);
                    lObjExpander.Expanded += ListExpander;
                    lObjExpander.Content = lObjStackPanel;
                 
                    SkpMenu.Children.Add(lObjExpander);
                }

            });
        }

        private void ListExpander(object sender, RoutedEventArgs e)
        {
            try
            {
                UIElementCollection lObjElements = SkpMenu.Children;
                //List<FrameworkElement> lLstObjElements = lObjElements.Cast<FrameworkElement>().ToList();

                foreach (Expander lObjExpander in lObjElements)
                {
                    if (lObjExpander.Header != (sender as Expander).Header)
                    {
                        lObjExpander.IsExpanded = false;
                        StackPanel lObjStackPanel = lObjExpander.Content as StackPanel;
                        UnselectListBox(lObjStackPanel);
                    }
                }
            }
            catch (Exception)
            {

            }
         
        }

        private void UnselectListBox(StackPanel pObjListBox)
        {
            try
            {
                UIElementCollection lObjElements = pObjListBox.Children;
                foreach (ListBox lObjListBox in lObjElements)
                {
                    lObjListBox.UnselectAll();
                }

            }
            catch (Exception)
            {
            }
        }

        private void LoadContainerChild(object pObjElement, ListBoxItem pObjListBox)
        {
            bool Exists = false;
            foreach (MdiChild lobjContainer in Container.Children)
            {
                if (lobjContainer.Title == pObjListBox.Content)
                {
                    lobjContainer.Focus();
                    Exists = true;
                }
            }

            if (!Exists)
            {
                UserControl lObjUserControl = pObjElement as UserControl;

                Container.Children.Add(new MdiChild
                {
                    Title = pObjListBox.Content.ToString(),
                    Height = !double.IsInfinity(lObjUserControl.MaxHeight) ? lObjUserControl.MaxHeight + 50 : double.NaN,
                    Width = !double.IsInfinity(lObjUserControl.MaxWidth) ? lObjUserControl.MaxWidth : double.NaN,
                    Content = pObjElement as UIElement, //new UCSubasta(),
                    Resizable = !double.IsInfinity(lObjUserControl.MaxWidth) && !double.IsInfinity(lObjUserControl.MaxHeight) ? false : true,
                });
                pObjListBox.IsSelected = false;
                mBolCanSelected = false;
            }
        }

        #region TabControl

        //Carga UserControl a un tabcontrol
        private void LoadContent(object pUCContenido, string pStrHeader) //CargarUserControl
        {
            Frame lObjFrametabFrame = new Frame();
            lObjFrametabFrame.Content = pUCContenido;

            tcContenido.Children.Add(lObjFrametabFrame);
        }

        //Carga Page a un tabcontrol
        private void LoadFrame(object pPageContenido, string pStrHeader)
        {
            Frame lObjFrametabFrame = new Frame();
            lObjFrametabFrame.Content = pPageContenido;

            tcContenido.Children.Add(lObjFrametabFrame);

            //ISFirstRender = false;
        }

        private bool SearchTabItem(string pStrTabHeader)
        {
            bool lbooltabItem = false;
            foreach (TabItem lObjTabitem in tcContenido.Children)
            {
                if (lObjTabitem.Header.ToString() == pStrTabHeader)
                {
                    return lObjTabitem.IsSelected = true;
                }
            }
            return lbooltabItem;
        }

        #endregion

        private void ShowLogin()
        {
            LoginDialog lFrmLoginDialog = new LoginDialog();
            //Login lFrmLoginDialog = new Login();
            lFrmLoginDialog.Owner = this;

            if (lFrmLoginDialog.ShowDialog() == true)
            {
                Thread lObjThread = new Thread(() => LoadMenu(GetMenu()));
                lObjThread.Start();
                RemotingConfiguration.Configure(PathUtilities.GetCurrent("Configurations\\WeighingMachine.config"), false);
                RemotingConfiguration.Configure(PathUtilities.GetCurrent("Configurations\\Boards.config"), false);
            }
            else
            {
                this.Close();
            }
        }

        private bool IsTestMode()
        {
            return ConfigurationManager.AppSettings.AllKeys.Contains("TestMode") && (
                   ConfigurationManager.AppSettings["TestMode"].ToString().Equals("true") || 
                   ConfigurationManager.AppSettings["TestMode"].ToString().Equals("True"));
        }

        private bool IsActiveSecurity()
        {
            return ConfigurationManager.AppSettings.AllKeys.Contains("ActiveSecurity") && (
                   ConfigurationManager.AppSettings["ActiveSecurity"].ToString().Equals("true") ||
                   ConfigurationManager.AppSettings["ActiveSecurity"].ToString().Equals("True"));
        }

        #endregion
    }
}
