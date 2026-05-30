using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using System.Collections.Generic;
using System.Threading;
using System;

namespace MLCCInspectionMC
{
    public class YoloManager : MSystem
    {
        private YoloInference _yoloModel;
        private Thread _yoloThread;
        private bool _isYoloRunning = false;

        //public event Action<int, bool, Bitmap, Bitmap> OnItemInspected;
        public event Action<int, bool, Bitmap, Bitmap> OnItemInspected;

        public Bitmap DrawResults(Bitmap source, List<YoloInference.Detection> results, bool isOK)
        {
            if (source == null) return null;

            Bitmap displayImg = new Bitmap(source);

            using (Graphics g = Graphics.FromImage(displayImg))
            {
                if (results != null && results.Count > 0)
                {
                    using (Pen boxPen = new Pen(Color.Lime, 3))
                    using (Font font = new Font("Arial", 14, FontStyle.Bold))
                    using (SolidBrush brush = new SolidBrush(Color.Lime))
                    {
                        foreach (var item in results)
                        {
                            g.DrawRectangle(boxPen, item.X, item.Y, item.Width, item.Height);
                            g.DrawString($"{item.ClassName}: {item.Confidence:0.00}", font, brush, item.X, item.Y - 25);
                        }
                    }
                }

                using (Font statusFont = new Font("Arial", 48, FontStyle.Bold))
                using (SolidBrush statusBrush = new SolidBrush(isOK ? Color.Lime : Color.Red))
                {
                    string statusText = isOK ? "OK" : "NG";
                    g.DrawString(statusText, statusFont, statusBrush, 20, 20);
                }
            }

            return displayImg;
        }

        public YoloManager(string modelPath)
        {
            _yoloModel = new YoloInference(modelPath);
            _isYoloRunning = true;
            _yoloThread = new Thread(ProcessYoloJob);
            _yoloThread.IsBackground = true;
            _yoloThread.Start();
        }

        private void ProcessYoloJob()
        {
            while (_isYoloRunning)
            {
                if (m_qImage[0].Count > 0 && m_qImage[1].Count > 0)
                {
                    bool hasImg0 = m_qImage[0].TryDequeue(out ImageData data0);
                    bool hasImg1 = m_qImage[1].TryDequeue(out ImageData data1);

                    if (hasImg0 && hasImg1)
                    {
                        int itemIndex = data0.TrayIndex;

                        try
                        {
                            if (data0.Image == null || data1.Image == null)
                            {
                                OnItemInspected?.Invoke(itemIndex, false, null, null);
                            }

                            var results0 = CheckImageDetails(data0.Image);
                            var results1 = CheckImageDetails(data1.Image);

                            bool isCam0_OK = results0.Count > 0;
                            bool isCam1_OK = results1.Count > 0;
                            bool isItemOK = isCam0_OK && isCam1_OK;

                            //Bitmap drawn0 = DrawResults(data0.Image, results0, isItemOK);
                            //Bitmap drawn1 = DrawResults(data1.Image, results1, isItemOK);

                            Bitmap drawn0 = DrawResults(data0.Image, results0, isCam0_OK);
                            Bitmap drawn1 = DrawResults(data1.Image, results1, isCam1_OK);

                            OnItemInspected?.Invoke(itemIndex, isItemOK, drawn0, drawn1);
                        }
                        catch { }
                        finally
                        {
                            data0.Image?.Dispose();
                            data1.Image?.Dispose();
                        }
                    }
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
        }
        private void ProcessYoloJob2()
        {
            while (_isYoloRunning)
            {
                if (m_qImage[0].Count > 0 && m_qImage[1].Count > 0)
                {
                    bool hasImg0 = m_qImage[0].TryDequeue(out ImageData data0);
                    bool hasImg1 = m_qImage[0].TryDequeue(out ImageData data1);

                    if (hasImg0 && hasImg1)
                    {
                        int itemIndex = data0.TrayIndex;

                        try
                        {
                            if (data0.Image == null)
                            {
                                OnItemInspected?.Invoke(itemIndex, false, null, null);
                            }

                            var results0 = CheckImageDetails(data0.Image);
                            var results1 = CheckImageDetails(data1.Image);

                            bool isCam0_OK = results0.Count > 0;
                            bool isCam1_OK = results1.Count > 0;
                            bool isItemOK = isCam0_OK && isCam1_OK;

                            Bitmap drawn0 = DrawResults(data0.Image, results0, isCam0_OK);
                            Bitmap drawn1 = DrawResults(data1.Image, results1, isCam1_OK);
                            //Bitmap drawn0 = DrawResults(data0.Image, results0, isItemOK);
                            //Bitmap drawn1 = DrawResults(data1.Image, results1, isItemOK);
                            OnItemInspected?.Invoke(itemIndex, isItemOK, drawn0, drawn1);
                        }
                        catch { }
                        finally
                        {
                            data0.Image?.Dispose();
                        }
                    }
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
        }

        public List<YoloInference.Detection> CheckImageDetails(Bitmap img)
        {
            if (img == null)
            {
                return new List<YoloInference.Detection>();
            }

            using (Mat cvMat = BitmapConverter.ToMat(img))
            {
                var results = _yoloModel.RunInference(cvMat);
                return results ?? new List<YoloInference.Detection>();
            }
        }

        public void sendEvent(int index, bool isItemOK, Bitmap img1, Bitmap img2)
        {
            OnItemInspected?.Invoke(index, isItemOK, img1, img2);
        }

        public void Stop()
        {
            _isYoloRunning = false;
        }
    }
}