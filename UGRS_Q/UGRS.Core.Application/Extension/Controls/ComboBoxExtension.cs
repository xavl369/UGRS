using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using UGRS.Core.DTO.Utility;
using UGRS.Core.Utility;

namespace UGRS.Core.Application.Extension.Controls
{
    public static class ComboBoxExtension
    {
        public static void LoadDataSource(this ComboBox pObjComboBox, IList<EnumDTO> pLstObjDataSource)
        {
            pObjComboBox.ItemsSource = pLstObjDataSource;
            pObjComboBox.DisplayMemberPath = "Text";
            pObjComboBox.SelectedValuePath = "Value";
            pObjComboBox.IsEditable = true;
            pObjComboBox.IsReadOnly = true;
            pObjComboBox.Text = "Seleccionar";
        }

        public static void LoadDataSource<T>(this ComboBox pObjComboBox) where T :struct, IConvertible
        {
            pObjComboBox.LoadDataSource(EnumUtility.GetEnumList<T>());
        }
        
        public static void SelectValue(this ComboBox pObjComboBox, int pIntValue)
        {
            for (int i = 0; i < pObjComboBox.Items.Count; i++)
            {
                if (((EnumDTO)pObjComboBox.Items[i]).Value == pIntValue)
                {
                    pObjComboBox.SelectedIndex = i;
                }
            }
        }

        public static bool ValidRequired(this ComboBox pObjComboBox)
        {
            if (pObjComboBox.SelectedValue == null || string.IsNullOrEmpty(pObjComboBox.SelectedValue.ToString()))
            {
                pObjComboBox.BorderBrush = Brushes.Red;
                return false;
            }
            else
            {
                pObjComboBox.BorderBrush = Brushes.Black;
                return true;
            }
        }
    }
}
