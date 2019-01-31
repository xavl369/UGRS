using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using UGRS.Core.Application.Forms.Log;
using UGRS.Core.DTO.Log;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Utility;

namespace UGRS.Core.Application.Utility
{
    public static class LogViewer
    {
        public static bool? Show(string pStrTitle, string pStrLogPath, Window pFrmWindow)
        {
            LogViewerForm lObjForm = new LogViewerForm();

            try
            {
                Thread lObjThread = new Thread(() => LoadLog(lObjForm, pStrLogPath));
                lObjThread.Start();
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message);
            }

            lObjForm.tblTitle.Text = pStrTitle;
            lObjForm.Width = 800;
            lObjForm.Height = 600;
            lObjForm.Owner = pFrmWindow;

            return lObjForm.ShowDialog();
        }

        private static void LoadLog(LogViewerForm pObjForm, string pStrLogPath)
        {
            pObjForm.grdContainer.BlockUI();

            try
            {
                IList<LogDTO> lLstObj = GetLog(pObjForm, pStrLogPath).OrderByDescending(x => x.Date).ToList();

                pObjForm.Dispatcher.Invoke((Action)delegate
                {
                    pObjForm.SetLog(lLstObj);
                });
            }
            catch (Exception lObjException)
            {
                pObjForm.grdContainer.UnblockUI();
                pObjForm.Dispatcher.Invoke((Action)delegate
                {
                    CustomMessageBox.Show("Error", lObjException.Message);
                });
            }
            finally
            {
                pObjForm.grdContainer.UnblockUI();
            }
        }

        private static IList<LogDTO> GetLog(LogViewerForm pObjForm, string pStrLogPath)
        {
            IList<LogDTO> lLstObjLog = new List<LogDTO>();
            string[] lArrStrLines = File.ReadAllLines(pStrLogPath, Encoding.UTF8);
            LogDTO lObjLogDTO = new LogDTO();

            if (lArrStrLines.Length > 0)
            {
                int lIntMaxLogView = GetMaxLoagView();
                int lIntCountLogView = lArrStrLines.Length > lIntMaxLogView ? lIntMaxLogView : lArrStrLines.Length;
                int lIntEndLogView = lArrStrLines.Length - lIntCountLogView;
                int lIntIndex = 1;

                for (int i = lArrStrLines.Length - 1; i >= lIntEndLogView; i--)
                {
                    pObjForm.grdContainer.SetWaitMessage(string.Format("Procesando {0} de {1}", lIntIndex, lIntCountLogView));
                    DateTime? lDtmDate = GetDate(lArrStrLines[i]);

                    if (lDtmDate != null)
                    {
                        lObjLogDTO = ParseToDTO(lArrStrLines[i]);
                        lLstObjLog.Add(lObjLogDTO);
                    }
                    else
                    {
                        lObjLogDTO.Message += string.Format(" {0}", lArrStrLines[i]);
                    }

                    lIntIndex++;
                }
            }

            return lLstObjLog;
        }

        private static LogDTO ParseToDTO(string pStrLine)
        {
            LogDTO lObjResult = new LogDTO();

            int lIntIndexOne = pStrLine.IndexOf("[");
		    int lIntIndexTwo = pStrLine.IndexOf("]");

		    int lIntLengthOne = Math.Abs(lIntIndexOne-lIntIndexTwo);
		    int lIntLengthTwo = Math.Abs((pStrLine.Length) - (lIntIndexTwo +2));

            lObjResult.Date = GetDate(pStrLine) ?? DateTime.MinValue;
            lObjResult.Type = pStrLine.Substring(lIntIndexOne+1, lIntLengthOne-1);
            lObjResult.Message = pStrLine.Substring(lIntIndexTwo+2, lIntLengthTwo);

            return lObjResult;
        }

        private static string GetStringDate(string pStrLine)
        {
            return string.Format("{0:19}", pStrLine).Substring(0, 19);
        }

        private static DateTime? GetDate(string pStrLine)
        {
            try
            {
                return DateTime.ParseExact(GetStringDate(pStrLine), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        private static int GetMaxLoagView()
        {
            try 
	        {
                return ConfigurationUtility.GetValue<int>("MaxLogView");
	        }
	        catch
	        {
                return 1000;
	        }
        }
    }
}
