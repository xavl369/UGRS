using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Forms.Base;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Data.Auctions.Factories;
using WPF.MDI;

namespace UGRS.Application.Auctions.Extensions
{
    public static class UserControlExtension
    {
        public static Auction ShowAuctionChooseDialog(this UserControl pObjUserControl, object pObjSender, KeyEventArgs pObjArgs, FilterEnum pEnmFilter, AuctionSearchModeEnum pEnmSearchMode)
        {
            Auction lObjAuction = null;
            TextBox lObjTextBox = (pObjSender as TextBox);
            List<Auction> lLstObjAuctions = new AuctionsServicesFactory().GetAuctionService().SearchAuctions(lObjTextBox.Text, pEnmFilter, pEnmSearchMode);

            if (lLstObjAuctions.Count == 1)
            {
                lObjAuction = lLstObjAuctions[0];
            }
            else
            {
                lObjTextBox.Focusable = false;
                UserControl lUCAuction = new UCSearchAuction(lObjTextBox.Text, lLstObjAuctions, pEnmFilter, pEnmSearchMode);
                lObjAuction = FunctionsUI.ShowWindowDialog(lUCAuction, Window.GetWindow(pObjUserControl)) as Auction;
                lObjTextBox.Focusable = true;
            }
            lObjTextBox.Focus();

            if (lObjAuction == null)
                return null;

            MoveToNextUIElement(pObjArgs);
            return lObjAuction;
        }

        public static Partner ShowPartnerChooseDialog(this UserControl pObjUserControl, object pObjSender, KeyEventArgs pObjArgs)
        {
            Partner lObjPartner = null;
            TextBox lObjTextBox = (pObjSender as TextBox);
            List<Partner> lLstObjPartners = new BusinessServicesFactory().GetPartnerService().SearchPartner(lObjTextBox.Text, FilterEnum.ACTIVE);

            if (lLstObjPartners.Count == 1)
            {
                lObjPartner = lLstObjPartners[0];
            }
            else
            {
                lObjTextBox.Focusable = false;
                UserControl lUCPartner = new UCSearchBusinessPartner(lObjTextBox.Text, lLstObjPartners, FilterEnum.ACTIVE);
                lObjPartner = FunctionsUI.ShowWindowDialog(lUCPartner, Window.GetWindow(pObjUserControl)) as Partner;
                lObjTextBox.Focusable = true;
            }
            lObjTextBox.Focus();

            if (lObjPartner == null)
                return null;

            MoveToNextUIElement(pObjArgs);
            return lObjPartner;
        }

        public static Batch ShowBatchChooseDialog(this UserControl pObjUserControl, object pObjSender, KeyEventArgs pObjArgs, long pLonAuctionId)
        {
            Batch lObjBatch = null;
            TextBox lObjTextBox = (pObjSender as TextBox);
            List<Batch> lLstObjBatches = new AuctionsServicesFactory().GetBatchService().SearchBatches(lObjTextBox.Text, pLonAuctionId).Where(x => !x.Unsold && x.Quantity > 0).ToList();

            if (lLstObjBatches.Count == 1)
            {
                lObjBatch = lLstObjBatches[0];
            }
            else
            {
                lObjTextBox.Focusable = false;
                UserControl lUCSearchBatch = new UCSearchBatch(lObjTextBox.Text, lLstObjBatches, pLonAuctionId);
                lObjBatch = FunctionsUI.ShowWindowDialog(lUCSearchBatch, Window.GetWindow(pObjUserControl)) as Batch;
                lObjTextBox.Focusable = true;
            }
            lObjTextBox.Focus();

            if (lObjBatch == null)
                return null;

            MoveToNextUIElement(pObjArgs);
            return lObjBatch;
        }

        public static void ShowMessage(this UserControl pObjUserControl, string pStrTitle, string lStrMessage)
        {
            pObjUserControl.Dispatcher.Invoke(() => CustomMessageBox.Show(pStrTitle, lStrMessage, pObjUserControl.GetParent()));
        }

        public static void ShowCustomerStockDetails(this UserControl pObjUserControl, long pLonAuctionId, long pLonCustomerId)
        {
            UCStockDetails lObjStockDetails = new UCStockDetails(pLonAuctionId, pLonCustomerId);
            BaseForm lFrmBase = new BaseForm();
            lFrmBase.Owner = pObjUserControl.GetParent();
            lFrmBase.SizeToContent = SizeToContent.WidthAndHeight;
            lFrmBase.ResizeMode = ResizeMode.NoResize;
            lFrmBase.tblTitle.Text = "Detalles del Stock";
            lFrmBase.grdContainer.Children.Add(lObjStockDetails);
            lFrmBase.ShowDialog();
        }

        public static void ModifyControlColorRed(this UserControl pObjUserControl, List<Control> pLstcontrol)
        {
            foreach (Control lObjControl in pLstcontrol)
            {
                lObjControl.BorderBrush = Brushes.Red;
            }
        }

        public static void ModifyControlColorBlack(this UserControl pObjUserControl, List<Control> pLstcontrol)
        {
            foreach (Control lObjControl in pLstcontrol)
            {
                lObjControl.BorderBrush = Brushes.Black;
            }
        }

        public static MdiChild GetInternalParent(this UserControl pObjUserControl)
        {
            return pObjUserControl.FindParent<MdiChild>();
        }

        public static void CloseForm(this UserControl pObjUserControl)
        {
            pObjUserControl.GetParent().Close();
        }

        public static void CloseInternalForm(this UserControl pObjUserControl)
        {
            try
            {
                pObjUserControl.FindParent<MdiChild>().Close();
            }
            catch
            {
                pObjUserControl.GetParent().Close();
            }
        }

        private static void MoveToNextUIElement(KeyEventArgs pObjArgs)
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
