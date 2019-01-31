using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UGRS.Core.Utility;
using UGRS.Object.WeighingMachine;

namespace UGRS.Test.Ports
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WeighingMachineServerObject mObjWeighingMachine;
        private Guid mObjConnection;
        Thread mObjInternalWorker;
        SerialPort mObjserialPort1;
        public MainWindow()
        {
            InitializeComponent();
            mObjserialPort1 = GetSerialPort();
            mObjserialPort1.Open();
            //Servicio
            //ConnectRemoteAccess();
            //mObjInternalWorker = new Thread(GetRemoteObject);
            //mObjInternalWorker.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //servicio
                //mObjWeighingMachine = new WeighingMachineServerObject();
                //mObjWeighingMachine.WriteSerialPort("Test servicio");
                WriteLineToSerialPort("Test Directo");
                mObjWeighingMachine.WriteSerialPort("Test");
                mObjWeighingMachine.WriteSerialPort("Folio: " + "Folio");
                mObjWeighingMachine.WriteSerialPort("UNION GANADERA REGIONAL DE SONORA");
                mObjWeighingMachine.WriteSerialPort("Cliente: " + "BPCode");
                mObjWeighingMachine.WriteSerialPort("Chofer " + "Driver");
                mObjWeighingMachine.WriteSerialPort("Placas " + "CarTag");

                mObjWeighingMachine.WriteSerialPort("Prod: " + "Item");
                mObjWeighingMachine.WriteSerialPort("Fecha: " + DateTime.Now.ToShortDateString());
                mObjWeighingMachine.WriteSerialPort("Peso Ent: " + "FirstWT");
                mObjWeighingMachine.WriteSerialPort("Peso Sal: " + "SecondWT");

                mObjWeighingMachine.WriteSerialPort("");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        
          
            //WriteLineToSerialPort("Test Directo");
        }

        private static bool ConnectRemoteAccess()
        {
            try
            {
                RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false); //Desconectar al cerrar
                //mObjConnection = mObjWeighingMachine.Connect();
                return true;

            }

            catch (Exception lObjException)
            {
                return false;
            }
        }

        private void GetRemoteObject()
        {

            try
            {
                //Objects
                mObjWeighingMachine = (WeighingMachineServerObject)Activator.GetObject(typeof(WeighingMachineServerObject), "http://localhost:8810/WeighingMachine");
              
                mObjConnection = mObjWeighingMachine.Connect();


            }
            catch (Exception lObjException)
            {
                MessageBox.Show(lObjException.Message);
                //UIApplication.ShowError(string.Format("Obtener bascula: {0}", lObjException.Message));
                // return false;
            }

        }

        private void WriteLineToSerialPort(string outText)
        {
            if (mObjserialPort1.IsOpen)
            {
                mObjserialPort1.WriteLine(outText);
            }
        }

        private SerialPort GetSerialPort()
        {
            SerialPort lObjResult = new SerialPort();

            //Get and set properties
            lObjResult.PortName = GetPortName();
            lObjResult.BaudRate = GetBaudRate();
            lObjResult.DataBits = GetDataBits();
            lObjResult.Parity = GetParity();
            lObjResult.StopBits = GetStopBits();
            lObjResult.ReadTimeout = GetReadTimeout();

            //AddEvent
           // lObjResult.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            return lObjResult;
        }

        #region Configuration

        private string GetPortName()
        {
            return ConfigurationUtility.GetValue<string>("PortName");
        }

        private int GetBaudRate()
        {
            return ConfigurationUtility.GetValue<int>("BaudRate");
        }

        private int GetDataBits()
        {
            return ConfigurationUtility.GetValue<int>("DataBits");
        }

        private Parity GetParity()
        {
            return ConfigurationUtility.GetValue<Parity>("Parity");
        }

        private StopBits GetStopBits()
        {
            return ConfigurationUtility.GetValue<StopBits>("StopBits");
        }

        private int GetReadTimeout()
        {
            return ConfigurationUtility.GetValue<int>("ReadTimeout");
        }

        #endregion

    }
}
