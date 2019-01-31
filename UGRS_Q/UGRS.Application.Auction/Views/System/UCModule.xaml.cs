using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    public partial class UCModule : UserControl
    {
        #region Attributes

        SystemServicesFactory mObjServiceFactory = new SystemServicesFactory();
        ListCollectionView mLcvListData = null;

        long mLonId = 0;
        int mIntPosition = 0;
        private Thread mObjWorker;

        #endregion

        #region Constructor

        public UCModule()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        #region UserControl

        /// <summary>
        /// Evento al terminar de cargar la pantalla.
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mObjWorker = new Thread(() => LoadDataGrid());
            mObjWorker.Start();
            clearControls();
        }

        #endregion

        #region TextBox

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
                mLcvListData.Filter = new Predicate<object>(o => ((Module)o).Name.ToUpper().Contains(txtSearch.Text.ToUpper()));
            }
            dgModules.ItemsSource = mLcvListData;
        }

        /// <summary>
        /// Solo números.
        /// </summary>
        private void txtPosition_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion

        #region DataGrid

        /// <summary>
        /// Carga los datos al dar doble clic en en el DataGrid.
        /// </summary>
        private void dgModules_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Module lObjModule = dgModules.SelectedItem as Module;

            if (lObjModule != null)
            {
                LoadDataInControls(lObjModule);
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

        #region Button

        /// <summary>
        /// Guardar datos.
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (validateControls())
            {
                mObjWorker = new Thread(() => SaveOrUpdate());
                mObjWorker.Start();
            }
            else
            {
                CustomMessageBox.Show("Módulo", "Favor de completar los campos.", this.GetParent());
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
            PositionUp((dgModules.SelectedItem as Module).Id);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            PositionDown((dgModules.SelectedItem as Module).Id);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Carga los datos en el DataGrid.
        /// </summary>
        public void LoadDataGrid()
        {
            FormLoading();
            try
            {
                this.Dispatcher.Invoke(() => { dgModules.ItemsSource = null; });
                List<Module> lLstModules = mObjServiceFactory.GetModuleService().GetList().Where(x => x.Active == true && x.Removed == false).ToList();
                mLcvListData = new ListCollectionView(lLstModules);
                this.Dispatcher.Invoke(() => { dgModules.ItemsSource = mLcvListData; });
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

        private void FormLoading(bool pBolForSave = false)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                if (!pBolForSave)
                {
                    grdList.BlockUI();
                    txtSearch.IsEnabled = false;
                }
                else
                {
                    grdPrincipal.BlockUI();
                }
            });
        }

        private void FormDefault(bool pBolForSave = false)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                if (!pBolForSave)
                {
                    grdList.UnblockUI();
                    txtSearch.IsEnabled = true;   
                }
                else
                {
                    grdPrincipal.UnblockUI();
                }
            });
        }

        /// <summary>
        /// Recarga el menú en la ventana padre MainAuction.
        /// </summary>
        private void LoadMenuParent()
        {
            List<Module> lLstModules = mObjServiceFactory.GetModuleService().GetList().Where(x => x.Active == true && x.Removed == false).ToList();
            MainAuction lobjMainAuction = (MainAuction)Window.GetWindow(this);
            //lobjMainAuction.LoadMenu(lLstModules);
        }


        /// <summary>
        /// Coloca los controles a su estado original.
        /// </summary>
        private void clearControls()
        {
            grdModule.ClearControl();

            btnSave.Content = "Guardar";
            lbltitle.Content = "Nuevo registro";
            mLonId = 0;

            lblMessage.Visibility = Visibility.Collapsed;
            btnDelete.Visibility = Visibility.Collapsed;
            btnNew.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Carga los datos seleccionados en los controles.
        /// </summary>
        private void LoadDataInControls(Module pObjModule)
        {
            txtName.Text = pObjModule.Name;
            txtDescription.Text = pObjModule.Description;
            txtIcon.Text = pObjModule.Icon;
            mIntPosition = pObjModule.Position;
            //txtPath.Text = pObjModule.Path;
        }

        /// <summary>
        /// Valida los controles.
        /// </summary>
        private bool validateControls()
        {
            return txtName.ValidRequired();
        }

        private Module GetModuleObject()
        {
            Module lObjModule = null;
            this.Dispatcher.Invoke(() =>
            {
                lObjModule = new Module()
                {
                    Id = mLonId,
                    Name = txtName.Text,
                    Description = txtDescription.Text,
                    Position = mIntPosition,
                    Icon = txtIcon.Text
                };
            });

            return lObjModule;
        }

        private void SaveOrUpdate()
        {
            FormLoading(true);
            try
            {
                mObjServiceFactory.GetModuleService().SaveOrUpdate(GetModuleObject());

                this.Dispatcher.Invoke(() => clearControls());

                FormDefault(true);

                mObjWorker = new Thread(() => LoadDataGrid());
                mObjWorker.Start();

                this.Dispatcher.Invoke(() => LoadMenuParent());
            }
            catch (Exception lObjException)
            {
                FormDefault(true);
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void Remove()
        {
            try
            {
                mObjServiceFactory.GetModuleService().Remove(mLonId);

                mObjWorker = new Thread(() => LoadDataGrid());
                mObjWorker.Start();

                clearControls();
                LoadMenuParent();
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void PositionUp(long pLonModuleId)
        {
            FormLoading(true);
            try
            {
                mObjServiceFactory.GetModuleService().PositionUp(pLonModuleId);
                mObjWorker = new Thread(() => LoadDataGrid());
                mObjWorker.Start();
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
            finally
            {
                FormDefault(true);
            }
        }

        private void PositionDown(long pLonModuleId)
        {
            FormLoading(true);
            try
            {
                mObjServiceFactory.GetModuleService().PositionDown(pLonModuleId);
                mObjWorker = new Thread(() => LoadDataGrid());
                mObjWorker.Start();
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
            finally
            {
                FormDefault(true);
            }
        }

        #endregion
    }
}
