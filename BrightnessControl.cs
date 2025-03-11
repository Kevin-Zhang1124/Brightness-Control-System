using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace StatusSwitchButton
{
    public class BrightnessControl
    {
        private static VideoCapture cap = null;
        private static bool _initialized = false;
        private static bool _initializationFailed = false;

        public static void SetBrightness(double newBrightness)
        {
            // 设置控制台输出编码为UTF-8
            Console.OutputEncoding = Encoding.UTF8;

            // 检查亮度值范围 (check the range of the brightness value)
            if (newBrightness < -64 || newBrightness > 64)
            {
                Console.WriteLine("亮度值超出范围，请输入-64到64之间的值。");
                return;
            }

            // 如果初始化已经失败，直接返回 (if initialization fails, return directly)
            if (_initializationFailed)
            {
                Console.WriteLine("摄像头初始化失败，无法调节亮度。");
                return;
            }

            // 如果未初始化，则进行初始化 (initalize if it's not)
            if (!_initialized)
            {
                // 打开摄像头 (open the camera)
                VideoCaptureAPIs[] apis = { VideoCaptureAPIs.DSHOW, VideoCaptureAPIs.MSMF };

                foreach (var api in apis)
                {
                    cap = new VideoCapture(0, api);
                    
                    if (cap.IsOpened())
                    {
                        Console.WriteLine($"成功使用 API {api} 打开摄像头");

                        // 检查摄像头是否支持亮度调节 (check if the camera supports brightness control)
                        double brightnessSupport = cap.Get(VideoCaptureProperties.Brightness);
                        if (brightnessSupport == -1)
                        {
                            Console.WriteLine("摄像头不支持亮度调节");
                            cap.Dispose();
                            cap = null;
                            _initializationFailed = true;
                            return;
                        }

                        _initialized = true;
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"使用 API {api} 打开摄像头失败");
                        cap.Dispose();
                        cap = null;
                    }
                }

                if (!_initialized)
                {
                    Console.WriteLine("无法打开摄像头");
                    _initializationFailed = true;
                    return;
                }
            }

            // 已完成初始化 (already initialized)
            // 设置亮度 (set brightness)
            bool setSuccess = cap.Set(VideoCaptureProperties.Brightness, newBrightness);

            // 获取并打印更新后的亮度值 (obtain and print the updated brightness value)
            double updatedBrightness = cap.Get(VideoCaptureProperties.Brightness);
            Console.WriteLine($"亮度已更新为: {updatedBrightness}");
            
        }

        // 用于程序退出时释放资源 (release the resource when logging out the program)
        public static void ReleaseCamera()
        {
            if (cap != null)
            {
                cap.Dispose();
                cap = null;
                _initialized = false;
                _initializationFailed = false;
                Console.WriteLine("摄像头资源已释放");
            }
        }
      
    }
}
