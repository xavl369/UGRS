using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Forms.Base;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.DTO.Session;
using UGRS.Core.Auctions.DTO.Users;
using UGRS.Core.Auctions.Enums.System;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.Factories;


namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        #region Attributes

        private UsersServicesFactory mObjUserFactory;
        private SystemServicesFactory mObjSystemFactory;
        private Thread mObjWorker;
        string mStrVersion = string.Empty;
        string mStrSerVersion = string.Empty;
        #endregion

        #region Constructor

        public LoginDialog()
        {
            InitializeComponent();
            mObjUserFactory = new UsersServicesFactory();
            string DirectoryDestination = System.IO.Directory.GetCurrentDirectory(); ;
            DirectoryDestination = DirectoryDestination + @"\Configurations";

            if (!System.IO.Directory.Exists(DirectoryDestination))
            {
                System.IO.Directory.CreateDirectory(DirectoryDestination);
                string[] files = System.IO.Directory.GetFiles(@"\\192.168.16.2\ClickOnceSubastaSur\Configurations");


                foreach (string s in files)
                {
                    string fileName = System.IO.Path.GetFileName(s);
                    string Destination = System.IO.Path.Combine(DirectoryDestination, fileName);
                    System.IO.File.Copy(s, Destination, true);
                }

            }
            mObjSystemFactory = new SystemServicesFactory();

        }

        #endregion

        #region Events

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr lObjWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(lObjWindowHandle).AddHook(new HwndSourceHook(WindowsUtility.WindowProc));

            mStrVersion = lblVersion.Content.ToString();
            //mStrSerVersion = 

            txtUser.Text = Properties.Settings.Default.gStrUser;

            if (!string.IsNullOrEmpty(txtUser.Text))
            {
                txtPassword.Focus();
                tbnRemmember.IsChecked = true;
            }
            else
            {
                txtUser.Focus();
                tbnRemmember.IsChecked = false;
            }
        }

        private void Rectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                DragMove();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (CustomMessageBox.ShowOption("Cerrar salir", "¿Desea salir del sistema de subasta?", "Si", "No", "") == true)
            {
                this.Close();
            }
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            if (!grdLogin.IsBlocked())
            {
                UCConfiguration lObjUCConfiguration = new UCConfiguration();
                BaseForm lObjBaseForm = new BaseForm(Window.GetWindow(this));

                lObjBaseForm.tblTitle.Text = "Configuración";
                lObjBaseForm.Width = 560;
                lObjBaseForm.MaxWidth = 560;
                lObjBaseForm.grdContainer.Children.Add(lObjUCConfiguration);
                lObjBaseForm.SizeToContent = System.Windows.SizeToContent.Height;
                lObjBaseForm.ResizeMode = System.Windows.ResizeMode.NoResize;

                lObjBaseForm.ShowDialog();
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (grdLogin.Valid())
            {
                SetUserCookie();
                DoLogin(txtUser.Text, txtPassword.Password);
            }
            else
            {
                CustomMessageBox.Show("Error", "Favor de completar los campos.", this);
            }
        }

        private void txtUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtPassword.Focus();
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        #endregion

        #region Methods

        private void DoLogin(string pStrUser, string pStrPassword)
        {
            if (!grdLogin.IsBlocked())
            {
                mObjWorker = new Thread(() => Login(pStrUser, pStrPassword));
                mObjWorker.Start();
            }
        }

        private void Login(string pStrUser, string pStrPassword)
        {
            FormLoading();
            try
            {
                if (CheckVersion())
                {
                    if (ActiveSecurity() && !TestMode())
                    {
                        SetUserSession(mObjUserFactory.GetUserService().Login(pStrUser, pStrPassword));
                        AuthorizeAccess();
                    }
                    else
                    {
                        StaticSessionUtility.mObjSeccion = new SessionDTO()
                        {
                            Id = 0,
                            UserName = "TestSystem"
                        };

                        AuthorizeAccess();
                    }

                }
            }
            catch (Exception lObjException)
            {
                FormDefult();
                ShowMessage(lObjException.Message);
            }
            finally
            {
                FormDefult();
            }
        }

        private bool CheckVersion()
        {
            if (mStrVersion.Contains(GetConfiguration(ConfigurationKeyEnum.APP_VERSION)))
            {
                return true;
            }
            else
            {
                ShowMessage("Favor de actualizar la aplicación");
                return false;
            }
        }


        private void SetUserSession(UserDTO pObjUser)
        {
            StaticSessionUtility.mObjSeccion = new SessionDTO()
            {
                Id = pObjUser.Id,
                UserName = pObjUser.UserName,
                Objects = new Dictionary<string, object>()
                {
                    {
                        "UserDTO", 
                        pObjUser
                    }
                }
            };
        }

        #region Configurations

        private bool TestMode()
        {
            return ConfigurationManager.AppSettings["TestMode"] != null ? ConfigurationUtility.GetValue<bool>("TestMode") : false;
        }

        private bool ActiveSecurity()
        {
            return ConfigurationManager.AppSettings["ActiveSecurity"] != null ? ConfigurationUtility.GetValue<bool>("ActiveSecurity") : true;
        }

        #endregion

        #region Main Thread

        private void AuthorizeAccess()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                this.DialogResult = true;
                this.Close();
            });
        }

        private void ShowMessage(string pStrMessage)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                CustomMessageBox.Show("Error", pStrMessage, this);
            });
        }

        private void FormLoading()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdLogin.BlockUI();
            });
        }

        private void FormDefult()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdLogin.UnblockUI();
            });
        }

        #endregion

        private void SetUserCookie()
        {
            if (tbnRemmember.IsChecked == true)
            {
                Properties.Settings.Default.gStrUser = txtUser.Text;
            }
            else
            {
                Properties.Settings.Default.gStrUser = string.Empty;
            }

            Properties.Settings.Default.Save();
        }

        private string GetConfiguration(ConfigurationKeyEnum pEnmKey)
        {
            return mObjSystemFactory.GetConfigurationService().GetByKey(pEnmKey);
        }



        #endregion
    }
}