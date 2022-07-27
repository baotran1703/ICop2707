//using System;
//using Windows.UI;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Shapes;
using System;
using System.IO.Ports;
using System.Windows.Threading;
using System.IO;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Threading.Tasks;

namespace ICOP_3
{
    public class ConveyorPart
    {
        public SolidColorBrush On_Color = new SolidColorBrush( Colors.Green);
        public SolidColorBrush Off_Color = new SolidColorBrush(Colors.Red);
        public SolidColorBrush NonSign = new SolidColorBrush(Colors.Gray);

        public Ellipse StatusEllip;

        public bool IsOn = false;
       
        public void updateStatus(char statusChar)
        {
            if (statusChar == '1')
            {
                StatusEllip.Fill = On_Color;
                IsOn = true;
            }
            else if (statusChar == '0')
            {
                StatusEllip.Fill = Off_Color;
                IsOn = false;
            }
            else
            {
                StatusEllip.Fill = NonSign;
            }
        }
    }


    public class Conveyor
    {
        public enum StatusCommand
        {
            None,
            ModeNG = 48,
            ModeOut = 49,
            ModeIn = 50,
            ModePass = 52,
            ModeUp = 53,
            ModeSlient = 54,
        }
        public event EventHandler StartTestRequest;
        public event EventHandler ReadyTestRequest;
        public ConveyorPart InSensor = new ConveyorPart();
        public ConveyorPart OutSensor = new ConveyorPart();
        public ConveyorPart TestSensor = new ConveyorPart();
        public ConveyorPart InCylinder = new ConveyorPart();
        public ConveyorPart OutCylinder = new ConveyorPart();
        public bool IsReady = true;
        private bool LastBoard = false;
        public Conveyor() { }
        public void UpdateStatus(string statusStr)
        {
            //Sensor_1_Status + Stopper_1_Status + Sensor_3_Status + Stopper_2_Status + Switch_Pass_Work_Status + Sensor_4_Status + Sensor_2_Status ;
            if (statusStr.Length >= 7)
            {
                InSensor.updateStatus(statusStr[0]);
                OutSensor.updateStatus(statusStr[5]);
                InCylinder.updateStatus(statusStr[1]);
                OutCylinder.updateStatus(statusStr[3]);  
                TestSensor.updateStatus(statusStr[2]);    // = 0  (have board) => IsOn = false
            }

            if (!TestSensor.IsOn && !IsReady)   /////  TestSensor.IsOn= true và  IsReady=false
            {
                ReadyTestRequest?.Invoke("Ready reciver board", null);  // board nhận sẳn sàng
                IsReady = true;
            }

            if (TestSensor.IsOn && IsReady)    /////  TestSensor.IsOn= false  và  IsReady=true
            {
                StartTestRequest?.Invoke("Board come", null);
                IsReady = false;
            }

            if (!TestSensor.IsOn && LastBoard) /////  TestSensor.IsOn= true 
            {
                ReadyTestRequest?.Invoke("Board Remover", null);
            }

            LastBoard = TestSensor.IsOn;

        }
    }

}