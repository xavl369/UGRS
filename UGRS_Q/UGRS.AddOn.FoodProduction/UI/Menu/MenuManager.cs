using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI.FoodProduction.Utilities;
using UGRS.AddOn.FoodProduction.Forms;
using UGRS.Core.Services;

namespace UGRS.AddOn.FoodProduction.UI.Menu
{
    public class MenuManager
    {
        #region Attributes

        private IList<Module> mLstObjModules;

        #endregion

        #region Properties

        public IList<Module> Menu
        {
            get
            {
                if (mLstObjModules == null)
                {
                    mLstObjModules = new List<Module>();
                }
                return mLstObjModules;
            }
            private set { mLstObjModules = value; }
        }

        #endregion

        #region Construct

        public MenuManager()
        {
            Menu.Add(new Module()
            {
                Type = SAPbouiCOM.BoMenuType.mt_POPUP,
                Image = PathUtilities.GetCurrent("Resources\\plant.bmp"),
                UniqueID = "PlantaAlimentos",
                String = "Planta de Alimentos",
                Enable = true,
                Position = -1,
                Sections = new List<Section>()
                {
                    new Section()
                    {
                        Type =SAPbouiCOM.BoMenuType.mt_STRING,
                        UniqueID = "TicketForm",
                        String = "Ticket",
                    },
                    new Section()
                    {
                        Type =SAPbouiCOM.BoMenuType.mt_STRING,
                        UniqueID = "TicketsListForm",
                        String = "Lista de tickets"
                    },
                    new Section()
                    {
                        Type =SAPbouiCOM.BoMenuType.mt_STRING,
                        UniqueID = "ReceptionForm",
                        String = "Recepción de Alimento",
                    },
                }
            });
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            //Initialize UI API objects
            Menus lObjMenus = null;
            MenuItem lObjMenuItem = null;
            MenuCreationParams lObjCreationPackage = null;

            //Get SAP B1 menus
            lObjMenus = UIApplication.GetMenus();

            //Get menu creation params
            lObjCreationPackage = UIApplication.GetMenuCreationParams();

            //Get modules menu of SAP B1 menu
            lObjMenuItem = UIApplication.GetMenus().Item("43520");

            //Add add-on menu
            foreach (Module lObjModule in Menu)
            {
                if(AddModule(lObjModule, lObjMenus, lObjMenuItem, lObjCreationPackage))
                {
                    foreach (Section lObjSection in lObjModule.Sections)
                    {
                        AddSection(lObjModule, lObjSection, lObjMenus, lObjMenuItem, lObjCreationPackage);
                    }
                }
            }
            //Add add-on Form: ReceptionForm Form in the Menu Inventario (3072), Operaciones de stock (43540)            
            //bool result = AddForm(lObjMenus, lObjMenuItem, lObjCreationPackage);
        }

        public void GetApplicationMenuEvent(ref SAPbouiCOM.MenuEvent pObjMenuEvent, out bool pObjBubbleEvent)
        {
            pObjBubbleEvent = true;

            try
            {
                if (pObjMenuEvent.BeforeAction)
                {
                    //Get menu unique id
                    //string lStrMenuUID  =  pObjMenuEvent.MenuUID;

                    //If is a section
                    //if(Menu.SelectMany(m=> m.Sections).Where(s=> s.UniqueID == lStrMenuUID).Count() > 0)
                    //{
                    //    //Get section 
                    //    Section lObjSection = Menu.SelectMany(m=> m.Sections).FirstOrDefault(s=> s.UniqueID == lStrMenuUID);
                        
                    //    //Is valid form
                    //    if(lObjSection.Form != null)
                    //    {
                    //        lObjSection.Form.
                    //    }
                    //}

                    switch (pObjMenuEvent.MenuUID)
                    {
                        case "TicketForm":

                            TicketForm lObjTicketForm = new TicketForm();
                            lObjTicketForm.UIAPIRawForm.Left = GetLeftMargin(lObjTicketForm.UIAPIRawForm);
                            lObjTicketForm.UIAPIRawForm.Top = GetTopMargin(lObjTicketForm.UIAPIRawForm);
                            lObjTicketForm.Show();

                            break;
                        case "TicketsListForm":

                            TicketsListForm lObjTicketsListFrm = new TicketsListForm();
                            lObjTicketsListFrm.UIAPIRawForm.Left = GetLeftMargin(lObjTicketsListFrm.UIAPIRawForm);
                            lObjTicketsListFrm.UIAPIRawForm.Top = GetTopMargin(lObjTicketsListFrm.UIAPIRawForm);
                            lObjTicketsListFrm.Show();

                            break;
                        case "ReceptionForm":

                            ReceptionForm lObjReceptionFrm = new ReceptionForm();
                            lObjReceptionFrm.UIAPIRawForm.Left = GetLeftMargin(lObjReceptionFrm.UIAPIRawForm);
                            lObjReceptionFrm.UIAPIRawForm.Top = GetTopMargin(lObjReceptionFrm.UIAPIRawForm);
                            lObjReceptionFrm.Show();

                            //DIApplication.DIConnect((SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany());                            
                            //Application.SBO_Application.Company
                            //ReceptionForm lObjReceptionFrm = new ReceptionForm(UI.UIApplication.Company);

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("MenuException: {0}", ex.Message));
                UIApplication.ShowMessageBox(ex.Message);
            }
        }
        //private bool AddForm(Menus pObjMenus, MenuItem pObjMenuItem, MenuCreationParams pObjCreationPackage)
        //{
        //    bool lBolResult = false;
        //    try
        //    {
        //        SAPbouiCOM.Menus lObjMenus = null;
        //        lObjMenus = pObjMenus.Item(6).SubMenus.Item(7).SubMenus.Item(4).SubMenus;
        //        pObjCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
        //        pObjCreationPackage.UniqueID = "ReceptionForm";
        //        pObjCreationPackage.String = "Recepción de Alimento";
        //        //pObjCreationPackage.Image = ;
        //        pObjCreationPackage.Position = lObjMenus.Count + 1;
        //        lObjMenus.AddEx(pObjCreationPackage);

        //        //Update result
        //        lBolResult = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        UIApplication.ShowError(string.Format("RegisterModuleException: {0}", ex.Message));
        //    }
        //    return lBolResult;
        //}

        private bool AddModule(Module pObjModule, Menus pObjMenus, MenuItem pObjMenuItem, MenuCreationParams pObjCreationPackage)
        {
            bool lBolResult = false;

            try
            {
                //Prepare module
                pObjCreationPackage.Type = BoMenuType.mt_POPUP;
                pObjCreationPackage.UniqueID = pObjModule.UniqueID;
                pObjCreationPackage.String = pObjModule.String;
                pObjCreationPackage.Enabled = pObjModule.Enable;
                pObjCreationPackage.Image = pObjModule.Image;
                pObjCreationPackage.Position = pObjModule.Position;
                pObjMenus = pObjMenuItem.SubMenus;

                //Add module
                pObjMenus.AddEx(pObjCreationPackage);

                //Update result
                lBolResult = true;
            }
            catch(Exception ex)
            {
                UIApplication.ShowError(string.Format("RegisterModuleException: {0}", ex.Message));
            }

            return lBolResult;
        }

        private bool AddSection(Module pObjModule, Section pObjSection, Menus pObjMenus, MenuItem pObjMenuItem, MenuCreationParams pObjCreationPackage)
        {
            bool lBolResult = false;

            try
            {
                //Get module menu
                pObjMenuItem = UIApplication.GetMenus().Item(pObjModule.UniqueID);
                
                //Prepare section
                pObjCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                pObjCreationPackage.UniqueID = pObjSection.UniqueID;
                pObjCreationPackage.String = pObjSection.String;
                pObjMenus = pObjMenuItem.SubMenus;

                //Add section
                pObjMenus.AddEx(pObjCreationPackage);

                //Update result
                lBolResult = true;
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("RegisterSectionException: {0}", ex.Message));
            }

            return lBolResult;
        }

        private void CenterForm(ref SAPbouiCOM.IForm pObjForm)
        {
            pObjForm.Left = GetLeftMargin(pObjForm);
            pObjForm.Top = GetTopMargin(pObjForm);
        }

        private int GetLeftMargin(SAPbouiCOM.IForm pObjForm)
        {
            try
            {
                double lDblWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                return (int)((lDblWidth / 2) - (pObjForm.Width / 2) + 100);
            }
            catch (Exception)
            {
                return 0;
              
            } 
          

        }

        private int GetTopMargin(SAPbouiCOM.IForm pObjForm)
        {
            try
            {
                double lDblHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                return (int)((lDblHeight / 2) - (pObjForm.Height / 2) - 100);
            }
            catch (Exception)
            {

                return 0;
            }
          
        }

        #endregion
    }
}
