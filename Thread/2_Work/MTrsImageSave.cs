using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MLCCInspectionMC
{
    public class MTrsImageSave
    {
        private readonly object _lockSaveImage = new object();
        private ConcurrentQueue<Mat> _queueSaveImage = new ConcurrentQueue<Mat>();
        public MTrsImageSave()
        {
            MSystem._mRun2 = new Thread(() => SaveImg());
            MSystem._mRun2.IsBackground = true;
            MSystem._mRun2.Start();
        }
        public void SaveImg()
        {
            while (true)
            {
                Thread.Sleep(100);
                try
                {
                    Mat bmp;
                    lock (_lockSaveImage)
                    {
                        if (_queueSaveImage.Count > 0)
                        {
                            if (_queueSaveImage.TryDequeue(out bmp))
                            {
                                if (bmp == null)
                                    return;
                                SaveImageBMP(bmp);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!Directory.Exists(Config.VS_LOG))
                        Directory.CreateDirectory(Config.VS_LOG);
                    string filePath = Path.Combine(Config.VS_LOG, "LogSaveIMG.txt");
                    File.WriteAllText(filePath, ex.ToString());
                }
            }

        }
        public void SaveImageBMP(Mat bmp)
        {
            string path = @"D:\\FA\\MLCCInspectionMC\\IMG_BMP";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + ".bmp";
            string fullPath = Path.Combine(path, fileName);
            Cv2.ImWrite(fullPath, bmp);
        }
        public void AddBMP(Mat bmp)
        {
            if (bmp == null)
                return;
            _queueSaveImage.Enqueue(bmp);
        }
        public void AddBMP(Bitmap bmp)
        {
            if (bmp == null)
                return;
            var img = BitmapConverter.ToMat(bmp);
            _queueSaveImage.Enqueue(img);
        }
    }
}
