using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Security;
using UGRS.Core.Auctions.Enums.Security;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    public partial class UCAuthorization : UserControl
    {
        #region Attributes

        private long mLonBatchId;
        private SpecialFunctionsEnum mEnmCurrentFunction;
        private SecurityServicesFactory mObjSecurityFactory;
        private Thread mObjWorker;

        #endregion

        #region Contructor

        public UCAuthorization(long pLonBatchId, SpecialFunctionsEnum pEnmFunction)
        {
            mLonBatchId = pLonBatchId;
            mEnmCurrentFunction = pEnmFunction;
            
            mObjSecurityFactory = new SecurityServicesFactory();
            InitializeComponent();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            grdAuthorization.ClearControl();
            txtUserName.Focus();
        }

        private void txtComment_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !grdAuthorization.IsBlocked())
            {
                Authorize(txtUserName.Text, txtPassword.Password, txtComment.Text);
            }
        }

        private void btnAuthorize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!grdAuthorization.IsBlocked())
            {
                Authorize(txtUserName.Text, txtPassword.Password, txtComment.Text);
            }
        }
        
        #endregion

        #region Methods

        private void Authorize(string pStrUserName, string pStrPassword, string pStrComment)
        {
            if (grdAuthorization.Valid())
            {
                mObjWorker = new Thread(() => AuthorizeThread(pStrUserName, pStrPassword, pStrComment));
                mObjWorker.Start();
            }
            else
            {
                CustomMessageBox.Show("Error", "Favor de completar los campos.", this.GetParent());
            }
        }

        private void AuthorizeThread(string pStrUserName, string pStrPassword, string pStrComment)
        {
            FormLoading();
            try
            {
                if(mObjSecurityFactory.GetAuthorizationService().Authorize(pStrUserName, pStrPassword, mEnmCurrentFunction))
                {
                    mObjSecurityFactory.GetAuthorizationService().Save(new Authorization()
                    {
                        BatchId = mLonBatchId,
                        UserId = mObjSecurityFactory.GetAuthorizationService().GetUserId(pStrUserName),
                        Function = mEnmCurrentFunction,
                        Comment = pStrComment
                    });

                    this.Dispatcher.Invoke(() =>
                    {
                        this.GetParent().DialogResult = true;
                        this.GetParent().Close();
                    });
                }
                else
                {
                    ShowMessage("Autorización denegada");
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

        private void ShowMessage(string pStrMessage)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                CustomMessageBox.Show("Error", pStrMessage, this.GetParent());
            });
        }

        private void FormLoading()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdAuthorization.BlockUI();
            });
        }

        private void FormDefult()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdAuthorization.UnblockUI();
            });
        }

        #endregion
    }
}
