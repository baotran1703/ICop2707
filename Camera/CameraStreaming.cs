using ImageProcessor;
using ImageProcessor.Imaging;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace HVT.VTM.Base
{
    public class CameraStreaming : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //public event EventHandler ImageUpdate;

        public System.Windows.Media.Imaging.BitmapSource LastFrame { get; private set; }
        private Task _previewTask;

        private CancellationTokenSource _cancellationTokenSource;
        public System.Windows.Controls.Image _imageControlForRendering;

        private readonly int _frameWidth;
        private readonly int _frameHeight;

        public int CameraDeviceId { get; private set; }

        public byte[] LastPngFrame { get; private set; }

        public CameraSetting cameraSetting = new CameraSetting();

        public VideoCapture videoCapture = new VideoCapture();

        public bool IsStarted = false;

        public enum VideoProperties
        {
            Exposure,
            Brightness,
            Contrast,
            Satuation,
            WhiteBalance,
            Sharpness,
            Focus,
            Zoom,
            Reset,
        }

        public CameraStreaming(
            System.Windows.Controls.Image imageControlForRendering,
            //System.Windows.Controls.Image imageControlForCrop = null,
            int frameWidth,
            int frameHeight,
            int cameraDeviceId)
        {
            _imageControlForRendering = imageControlForRendering;
            //_imageControlForCropRendering = imageControlForCrop;
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            CameraDeviceId = cameraDeviceId;
        }

        public async Task Start()
        {
            // Never run two parallel tasks for the webcam streaming
            if (_previewTask != null && !_previewTask.IsCompleted)
                return;

            _cancellationTokenSource = new CancellationTokenSource();
            _previewTask = Task.Run(async () =>
            {
                try
                {
                    videoCapture = new VideoCapture();
                    if (!videoCapture.Open(CameraDeviceId)) 
                        return ;               
                    Console.WriteLine("Set frame width:" + videoCapture.Set(VideoCaptureProperties.FrameWidth, _frameWidth));
                    Console.WriteLine("Set frame height:" + videoCapture.Set(VideoCaptureProperties.FrameHeight, _frameHeight));
                    Console.WriteLine("frame width:" + videoCapture.Get(VideoCaptureProperties.FrameWidth));
                    Console.WriteLine("frame height:" + videoCapture.Get(VideoCaptureProperties.FrameHeight));
                    Console.WriteLine("Set FPS" + videoCapture.Set(VideoCaptureProperties.Fps,24));
                    Console.WriteLine("FPS " + videoCapture.Get(VideoCaptureProperties.Fps));

                    using (Mat frame = new Mat())
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            //var timeStart = DateTime.Now;
                            videoCapture.Read(frame);
                            //Console.WriteLine(videoCapture.FrameCount);
                            if (!frame.Empty())
                            {
                                IsStarted = true;
                                LastFrame = frame.ToBitmapSource();
                                //System.Windows.Media.Imaging.BitmapSource lastFrameCropBitmapImage = _lastCropFrame.ToBitmapSource();
                                //lastFrameCropBitmapImage.Freeze();
                                LastFrame.Freeze();
                                _imageControlForRendering.Dispatcher.Invoke(new Action(() => _imageControlForRendering.Source = LastFrame), DispatcherPriority.DataBind);
                                //_imageControlForCropRendering.Dispatcher.Invoke(() => _imageControlForCropRendering.Source = lastFrameCropBitmapImage);
                                //ImageUpdate?.Invoke(lastFrameBitmapImage, null);
                            }
                            // 30 FPS
                            await Task.Delay(33);
                            //Console.WriteLine(DateTime.Now - timeStart);
                        }
                    }
                    videoCapture?.Dispose();
                }
                finally
                {
                  
                }

            }, _cancellationTokenSource.Token);

            if (_previewTask.IsFaulted)
            {
                // To let the exceptions exit
                await _previewTask;
            }
        }

        public async Task Stop()
        {
            if (_cancellationTokenSource != null)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                    return;
                // If "Dispose" gets called before Stop
                if (!_previewTask.IsCompleted)
                {
                    _cancellationTokenSource.Cancel();
                // Wait for it, to avoid conflicts with read/write of _lastFrame
                    await _previewTask;
                }
            }
        }

        public Bitmap Capture()
        {
            return null;
        }

        public void SetParammeter(VideoProperties properties, int Value, bool InTest)
        {
            if (videoCapture == null)
            {
                return;
            }
            switch (properties)
            {
                case VideoProperties.Exposure:
                    videoCapture.Set(VideoCaptureProperties.Exposure, Value);
                    break;
                case VideoProperties.Brightness:
                    videoCapture.Set(VideoCaptureProperties.Brightness, Value);
                    break;
                case VideoProperties.Contrast:
                    videoCapture.Set(VideoCaptureProperties.Contrast, Value);
                    break;
                //case VideoProperties.Satuation:
                //    videoCapture.Set(VideoCaptureProperties.Saturation, Value);
                //    break;
                case VideoProperties.WhiteBalance:
                    videoCapture.Set(VideoCaptureProperties.WBTemperature, Value);
                    break;
                case VideoProperties.Sharpness:
                    videoCapture.Set(VideoCaptureProperties.Sharpness, Value);
                    break;
                case VideoProperties.Focus:
                    videoCapture.Set(VideoCaptureProperties.Focus, Value);
                    break;
                case VideoProperties.Zoom:
                    videoCapture.Set(VideoCaptureProperties.Zoom, Value);
                    break;
                default:
                    break;
            }
        }

        public bool SetParammeter(CameraSetting cameraSetting)
        {
            videoCapture.Set(VideoCaptureProperties.Exposure, cameraSetting.Exposure);

            videoCapture.Set(VideoCaptureProperties.Brightness, cameraSetting.Brightness);

            videoCapture.Set(VideoCaptureProperties.Contrast, cameraSetting.Contrast);

       //     videoCapture.Set(VideoCaptureProperties.Saturation, cameraSetting.Saturation);

            if (videoCapture.Set(VideoCaptureProperties.WhiteBalanceBlueU, cameraSetting.WBTemperature))
                Console.WriteLine("white balance set " + cameraSetting.WBTemperature);

            videoCapture.Set(VideoCaptureProperties.Sharpness, cameraSetting.Sharpness);

            videoCapture.Set(VideoCaptureProperties.Focus, cameraSetting.Focus);

            videoCapture.Set(VideoCaptureProperties.Zoom, cameraSetting.Zoom);

            return true;
  
        }

        public void SetParammeter(VideoProperties properties, int Value)
        {
            if (videoCapture == null)
            {
                return;
            }
            switch (properties)
            {
                case VideoProperties.Exposure:
                    videoCapture.Set(VideoCaptureProperties.Exposure, Value);
                    cameraSetting.Exposure = Value;
                    break;
                case VideoProperties.Brightness:
                    videoCapture.Set(VideoCaptureProperties.Brightness, Value);
                    cameraSetting.Brightness = Value;
                    break;
                case VideoProperties.Contrast:
                    videoCapture.Set(VideoCaptureProperties.Contrast, Value);
                    cameraSetting.Contrast = Value;
                    break;
                //case VideoProperties.Satuation:
                //    videoCapture.Set(VideoCaptureProperties.Saturation, Value);
                //    cameraSetting.Saturation = Value;
                //    break;
                case VideoProperties.WhiteBalance:
                    if (videoCapture.Set(VideoCaptureProperties.WhiteBalanceBlueU, Value))
                        Console.WriteLine("white balance set " + Value);
                    // videoCapture.Set(VideoCaptureProperties.WhiteBalanceRedV,6000- Value);
                    cameraSetting.WBTemperature = Value;
                    break;
                case VideoProperties.Sharpness:
                    videoCapture.Set(VideoCaptureProperties.Sharpness, Value);
                    cameraSetting.Sharpness = Value;
                    break;
                case VideoProperties.Focus:
                    videoCapture.Set(VideoCaptureProperties.Focus, Value);
                    cameraSetting.Focus = Value;
                    break;
                case VideoProperties.Zoom:
                    videoCapture.Set(VideoCaptureProperties.Zoom, Value);
                    cameraSetting.Zoom = Value;
                    break;
                default:
                    break;
            }

        }

        public int GetParammeter(VideoProperties properties)
        {
            if (videoCapture == null)
            {
                return 0;
            }
            switch (properties)
            {
                case VideoProperties.Exposure:
                    return (int)videoCapture.Get(VideoCaptureProperties.Exposure);
                case VideoProperties.Brightness:
                    return (int)videoCapture.Get(VideoCaptureProperties.Brightness);
                case VideoProperties.Contrast:
                    return (int)videoCapture.Get(VideoCaptureProperties.Contrast);
                //case VideoProperties.Satuation:
                //    return (int)videoCapture.Get(VideoCaptureProperties.Saturation);
                case VideoProperties.WhiteBalance:
                    return (int)videoCapture.Get(VideoCaptureProperties.WBTemperature);
                case VideoProperties.Sharpness:
                    return (int)videoCapture.Get(VideoCaptureProperties.Sharpness);
                case VideoProperties.Focus:
                    return (int)videoCapture.Get(VideoCaptureProperties.Focus);
                case VideoProperties.Zoom:
                    return (int)videoCapture.Get(VideoCaptureProperties.Zoom);
                default:
                    return 0;
            }
        }
        public CameraSetting GetParammeter()
        {
            return cameraSetting;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}





// Giá như mùa hè ấy ta đừng nói thương nhau, thì giờ đây cả hai chẳng phải tổn thương nhau. Thương anh, em đau lòng quá! Còn anh, có thương em không?
//Chiều nay tan làm, em thấy cây phượng sau trường bắt đầu nở rồi. 
//Từng mảng kí ức theo cánh phượng đỏ trở lại trước mắt em...
//Kí ức của một thời thanh xuân rực rỡ...
//Chúng ta lựa chọn rời xa có đúng không anh?
// 