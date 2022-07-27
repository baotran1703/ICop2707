using System;
using System.Windows;
using System.IO.Ports;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Media;
using System.IO;
using System.Text.Json;

namespace ICOP_3
{
    public partial class MainWindow : Window
    {
        // Valriable
        SerialPort SerialPort = new SerialPort();
        string RecievedData;

        public void Serial_Init()
        {
            Conveyor_Init();
        }
        private void bt_Save_Com_Click(object sender, RoutedEventArgs e)
        {
            SerialPort SerialPort = new SerialPort();
            SerialPort.PortName = cbb_COM_PORT.Text;
            SerialPort.BaudRate = Convert.ToInt32(cbb_BAUD_RATE.Text);
            SerialPort.Parity = (Parity)Enum.Parse(typeof(Parity), cbb_PARITY.Text);
            SerialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cbb_STOPBIT.Text);
            SerialPort.ReadTimeout = Convert.ToInt32(tb_ReadTimeout.Text);
            SerialPort.WriteTimeout = Convert.ToInt32(tb_WriteTimeout.Text);
            MessageBox.Show("Save parameter setting sucessfull.");
            string jsonString1 = JsonSerializer.Serialize(SerialPort.PortName);
            string jsonString2 = JsonSerializer.Serialize(SerialPort.BaudRate);
            string jsonString3 = JsonSerializer.Serialize(SerialPort.Parity);
            string jsonString4 = JsonSerializer.Serialize(SerialPort.StopBits);
            string jsonString5 = JsonSerializer.Serialize(SerialPort.ReadTimeout);
            string jsonString6 = JsonSerializer.Serialize(SerialPort.WriteTimeout);
            string[] jsonString = { jsonString1, jsonString2, jsonString3, jsonString4, jsonString5, jsonString6 };
            var Setting = DirectoryF.Setting + "\\Comsetting.txt";
            File.WriteAllLines(Setting, jsonString);
        }

        private delegate void UpdateUiTextDelegate(string text);

        private void RecievedData_(object sender, SerialDataReceivedEventArgs e)
        {
            Timer_TimeOut.Stop();
            if (SerialPort.IsOpen)
            {
                //  RecievedData = SerialPort.ReadLine();
                RecievedData = SerialPort.ReadLine();
                Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), RecievedData);
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Conveyor.UpdateStatus(RecievedData);

                }));
                Console.WriteLine(RecievedData.ToString());
            }
           
        }

        private void WriteData(string text)
        {
            tb_RecievedData.Text += text;
        }

        DispatcherTimer Timer_TimeOut = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(500),
        };

        private void SendData_Click(object sender, RoutedEventArgs e)
        {
            SerialSend(tb_SendData.Text);
            WriteData(tb_SendData.Text);

            //Timer_TimeOut.Tick += Timer_TimeOut_Tick;
            //Timer_TimeOut.Start();
        }

        private void Timer_TimeOut_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate
            {
                el_COM.Fill = System.Windows.Media.Brushes.Gray;
                
            }));
            Timer_TimeOut.Stop();
            Timer_TimeOut.Tick -= Timer_TimeOut_Tick;
        }

        public void SerialSend(string data)
        {
            if (SerialPort.IsOpen)
            {
                try
                {
                    SerialPort.Write(data);

                    tb_SendData.Text = data;
                    el_COM.Fill = System.Windows.Media.Brushes.Green;
                }
                catch (Exception ex)
                {
                    tb_RecievedData.Text = ex.Message;
                }
            }
            else
            {
                tb_SendData.Text = "COM Port not ready";
            }
        }

        /// <summary>
        /// //////////////////////////////////////////////////
        /// </summary>

        public Conveyor Conveyor = new Conveyor();

        public void Conveyor_Init()
        {
            Conveyor.InSensor.StatusEllip = el_InSensor;
            Conveyor.OutSensor.StatusEllip = el_InCylinder;
            Conveyor.TestSensor.StatusEllip = el_TestCensor;
            Conveyor.OutCylinder.StatusEllip = el_OutCylinder;
            Conveyor.InCylinder.StatusEllip = el_OutSensor;

            Conveyor.StartTestRequest += Conveyor_StartTestRequest;
            Conveyor.ReadyTestRequest += Conveyor_ReadyTestRequest;
        }
    }
}
