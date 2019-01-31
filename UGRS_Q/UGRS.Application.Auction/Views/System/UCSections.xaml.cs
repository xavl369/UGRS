using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.System;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCSections.xaml
    /// </summary>
    public partial class UCSections : UserControl
    {
        SystemServicesFactory mObjServicesFactory = new SystemServicesFactory();
        ListCollectionView mLcvListData = null; //Filtros
        long mLonId = 0;//bandera si se requiere modificar/eliminar
        int mIntPosition = 0;
        private Thread mObjWorker;

        public UCSections()
        {
            InitializeComponent();
        }

        #region Controls

        /// <summary>
        /// Evento al terminar de cargar la pantalla.
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mObjWorker = new Thread(() => LoadDataGrid());
            mObjWorker.Start();              
            loadCombobox();
            clearControls();
        }

        /// <summary>
        /// Guardar datos.
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (grdSection.Valid())
            {
                SaveOrUpdate();
            }
            else
            {
                CustomMessageBox.Show("Sección", "Favor de completar los campos.", this.GetParent());
            }
        }

        /// <summary>
        /// Inicializa controles.
        /// </summary>
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            clearControls();
        }

        /// <summary>
        /// Elimina un registro.
        /// </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CustomMessageBox.ShowOption("Eliminar", "¿Desea eliminar el registro?", "Si", "No", "", Window.GetWindow(this)) == true)
            {
                Remove();
            }
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            PositionUp
            (
                (dgSections.SelectedItem as UGRS.Core.Auctions.Entities.System.Section).Id, 
                (dgSections.SelectedItem as UGRS.Core.Auctions.Entities.System.Section).ModuleId
            );
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            PositionDown
            (
                (dgSections.SelectedItem as UGRS.Core.Auctions.Entities.System.Section).Id,
                (dgSections.SelectedItem as UGRS.Core.Auctions.Entities.System.Section).ModuleId
            );
        }

        /// <summary>
        /// Realiza una busqueda.
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtSearch.Text))
            {
                mLcvListData.Filter = null;
            }
            else
            {
                mLcvListData.Filter = new Predicate<object>(o => ((UGRS.Core.Auctions.Entities.System.Section)o).Name.ToUpper().Contains(txtSearch.Text.ToUpper()));
            }
            dgSections.ItemsSource = mLcvListData;
        }


        /// <summary>
        /// Carga los datos al dar doble clic en en el DataGrid.
        /// </summary>
        private void dgSections_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UGRS.Core.Auctions.Entities.System.Section lObjModule = dgSections.SelectedItem as UGRS.Core.Auctions.Entities.System.Section;

            if (lObjModule != null)
            {
                loadControls(lObjModule);
                btnSave.Content = "Modificar";
                lbltitle.Content = "Modificar registro";
                lblMessage.Visibility = Visibility.Collapsed;
                btnNew.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
                txtName.BorderBrush = Brushes.Black;
                txtDescription.BorderBrush = Brushes.Black;
                mLonId = lObjModule.Id;
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Carga información en el combox.
        /// </summary>
        private void loadCombobox()
        {
            List<Module> lLstModules = mObjServicesFactory.GetModuleService().GetList().Where(x => x.Active == true && x.Removed == false).ToList();
            cbModule.ItemsSource = lLstModules;
            cbModule.DisplayMemberPath = "Name";
            cbModule.SelectedValuePath = "Id";
        }

        /// <summary>
        /// Carga los datos en el DataGrid.
        /// </summary>
        public void LoadDataGrid()
        {
            FormLoading();
            try
            {
                this.Dispatcher.Invoke(() => { dgSections.ItemsSource = null; });
                List<UGRS.Core.Auctions.Entities.System.Section> lLstModules = mObjServicesFactory.GetSectionService().GetList().Where(x => x.Active == true && x.Removed == false).ToList();
                mLcvListData = new ListCollectionView(lLstModules);
                this.Dispatcher.Invoke(() => { dgSections.ItemsSource = mLcvListData; });
            }
            catch (Exception lObjException)
            {
                FormDefault();
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
            finally
            {
                FormDefault();
            }
        }

        private void FormLoading()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdList.BlockUI();
                txtSearch.IsEnabled = false;
            });
        }

        private void FormDefault()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdList.UnblockUI();
                txtSearch.IsEnabled = true;
            });
        }

        /// <summary>
        /// Recarga el menú en la ventana padre MainAuction.
        /// </summary>
        private void LoadMenuParent()
        {
            List<Module> lLstModules = mObjServicesFactory.GetModuleService().GetList().Where(x => x.Active == true && x.Removed == false).ToList();
            MainAuction lobjMainAuction = (MainAuction)Window.GetWindow(this);
           // lobjMainAuction.LoadMenu(lLstModules);
        }

        /// <summary>
        /// Coloca los controles a su estado original.
        /// </summary>
        private void clearControls()
        {
            mLonId = 0;
            grdSection.ClearControl();

            btnSave.Content = "Guardar";
            lbltitle.Content = "Nuevo registro";
            
            lblMessage.Visibility = Visibility.Collapsed;
            btnDelete.Visibility = Visibility.Collapsed;
            btnNew.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Carga los datos seleccionados en los controles.
        /// </summary>
        private void loadControls(UGRS.Core.Auctions.Entities.System.Section pObjSection)
        {
            txtName.Text = pObjSection.Name;
            txtDescription.Text = pObjSection.Description;
            txtPath.Text = pObjSection.Path;
            cbModule.SelectedValue = pObjSection.ModuleId;
            mIntPosition = pObjSection.Position;
        }

        private UGRS.Core.Auctions.Entities.System.Section GetSectionObject()
        {
            return new UGRS.Core.Auctions.Entities.System.Section()
            {
                Id = mLonId,
                Name = txtName.Text,
                Description = txtDescription.Text,
                Path = txtPath.Text,
                Position = mIntPosition,
                ModuleId = Convert.ToInt32(cbModule.SelectedValue)
            };
        }

        private void SaveOrUpdate()
        {
            try
            {
                mObjServicesFactory.GetSectionService().SaveOrUpdate(GetSectionObject());

                loadCombobox();
                clearControls();
                LoadDataGrid();
                LoadMenuParent();
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void Remove()
        {
            try
            {
                mObjServicesFactory.GetSectionService().Remove(mLonId);

                loadCombobox();
                clearControls();
                LoadDataGrid();
                LoadMenuParent();
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void PositionUp(long pLonSectionId, long pLonModuleId)
        {
            FormLoading();
            try
            {
                mObjServicesFactory.GetSectionService().PositionUp(pLonSectionId, pLonModuleId);
                mObjWorker = new Thread(() => LoadDataGrid());
                mObjWorker.Start();
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
            finally
            {
                FormDefault();
            }
        }

        private void PositionDown(long pLonSectionId, long pLonModuleId)
        {
            FormLoading();
            try
            {
                mObjServicesFactory.GetSectionService().PositionDown(pLonSectionId, pLonModuleId);
                mObjWorker = new Thread(() => LoadDataGrid());
                mObjWorker.Start();
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
            finally
            {
                FormDefault();
            }
        }

        #endregion
    }
}