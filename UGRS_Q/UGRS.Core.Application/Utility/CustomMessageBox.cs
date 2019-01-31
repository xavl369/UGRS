using System.Windows;
using UGRS.Core.Application.Forms.Base;
using UGRS.Core.Application.UC.Message;

namespace UGRS.Core.Application.Utility
{
    public static class CustomMessageBox
    {
        public static bool? Show(string pStrMessage)
        {
            return Show("", pStrMessage);
        }

        public static bool? Show(string pStrTitle, string pStrMessage)
        {
            BaseForm lObjBaseForm = GetMessageForm(pStrTitle, pStrMessage);
            return lObjBaseForm.ShowDialog();
        }

        public static bool? Show(string pStrTitle, string pStrMessage, Window pFrmWindow)
        {
            BaseForm lObjBaseForm = GetMessageForm(pStrTitle, pStrMessage, pFrmWindow);
            return lObjBaseForm.ShowDialog();
        }

        public static bool? ShowOption(string pStrMessage, string pStrOption1, string pStrOption2, string pStrOption3)
        {
            return ShowOption("", pStrMessage, pStrOption1, pStrOption2, pStrOption3);
        }

        public static bool? ShowOption(string pStrTitle, string pStrMessage, string pStrOption1, string pStrOption2, string pStrOption3)
        {
            BaseForm lObjBaseForm = GetOptionForm(pStrTitle, pStrMessage, pStrOption1, pStrOption2, pStrOption3);
            return lObjBaseForm.ShowDialog();
        }

        public static bool? ShowOption(string pStrTitle, string pStrMessage, string pStrOption1, string pStrOption2, string pStrOption3, Window pFrmWindow)
        {
            BaseForm lObjBaseForm = GetOptionForm(pStrTitle, pStrMessage, pStrOption1, pStrOption2, pStrOption3);
            lObjBaseForm.Owner = pFrmWindow;
            return lObjBaseForm.ShowDialog();
        }

        private static BaseForm GetMessageForm(string pStrTitle, string pStrMessage, Window pFrmWindow)
        {
            return ConfigureMessageForm(pStrTitle, pStrMessage, new BaseForm(pFrmWindow));
        }

        private static BaseForm GetMessageForm(string pStrTitle, string pStrMessage)
        {
            return ConfigureMessageForm(pStrTitle, pStrMessage, new BaseForm());
        }

        private static BaseForm ConfigureMessageForm(string pStrTitle, string pStrMessage, BaseForm pFrmBase)
        {
            UCMessage lObjUCMessage = new UCMessage();

            lObjUCMessage.tblMessage.Text = pStrMessage;
            pFrmBase.tblTitle.Text = pStrTitle;
            pFrmBase.Width = 400;
            pFrmBase.grdContainer.Children.Add(lObjUCMessage);
            pFrmBase.SizeToContent = System.Windows.SizeToContent.Height;
            pFrmBase.ResizeMode = ResizeMode.NoResize;
            lObjUCMessage.btnOk.Focus();
            return pFrmBase;
        }

        private static BaseForm GetOptionForm(string pStrTitle, string pStrMessage, string pStrOption1, string pStrOption2, string pStrOption3)
        {
            return ConfigureOptionForm(pStrTitle, pStrMessage, pStrOption1, pStrOption2, pStrOption3, new BaseForm());
        }

        private static BaseForm GetOptionForm(string pStrTitle, string pStrMessage, string pStrOption1, string pStrOption2, string pStrOption3, Window pFrmWindow)
        {
            return ConfigureOptionForm(pStrTitle, pStrMessage, pStrOption1, pStrOption2, pStrOption3, new BaseForm(pFrmWindow));
        }

        private static BaseForm ConfigureOptionForm(string pStrTitle, string pStrMessage, string pStrOption1, string pStrOption2, string pStrOption3, BaseForm pFrmBase)
        {
            UCOption lObjUCOption = new UCOption();

            lObjUCOption.tblMessage.Text = pStrMessage;
            pFrmBase.tblTitle.Text = pStrTitle;
            pFrmBase.Width = 400;
            pFrmBase.grdContainer.Children.Add(lObjUCOption);
            pFrmBase.SizeToContent = System.Windows.SizeToContent.Height;
            pFrmBase.ResizeMode = ResizeMode.NoResize;

            lObjUCOption.btnOption1.Content = pStrOption1;
            lObjUCOption.btnOption2.Content = pStrOption2;
            lObjUCOption.btnOption3.Content = pStrOption3;

            if (string.IsNullOrEmpty(pStrOption1))
            {
                lObjUCOption.btnOption1.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrEmpty(pStrOption2))
            {
                lObjUCOption.btnOption2.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrEmpty(pStrOption3))
            {
                lObjUCOption.btnOption3.Visibility = Visibility.Collapsed;
            }

            return pFrmBase;
        }
    }
}
