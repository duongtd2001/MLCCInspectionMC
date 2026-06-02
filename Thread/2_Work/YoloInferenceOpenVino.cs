using OpenCvSharp;
using OpenVinoSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MLCCInspectionMC
{
    public class YoloInference
    {
        public class Detection
        {
            public int ClassId { get; set; }
            public string ClassName { get; set; }
            public float Confidence { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
        }

        private readonly Core _core;
        private readonly Model _model;
        private readonly CompiledModel _compiledModel;
        private readonly InferRequest _inferRequest;

        private readonly int _inputHeight;
        private readonly int _inputWidth;
        private readonly int _numClasses;
        private readonly List<string> _classNames = new List<string> { "Capacitor" };

        private float _ratio = 1.0f;
        private float _dw = 0f;
        private float _dh = 0f;

        public YoloInference(string modelPath)
        {
            _core = new Core();
            _model = _core.read_model(modelPath);

            var inputShape = _model.inputs()[0].get_shape();
            _inputHeight = (int)inputShape[2];
            _inputWidth = (int)inputShape[3];  

            var outputShape = _model.outputs()[0].get_shape();
            _numClasses = (int)outputShape[1] - 4;

            _compiledModel = _core.compile_model(_model, "CPU");

            _inferRequest = _compiledModel.create_infer_request();
        }

        public List<Detection> RunInference(Mat mat, float confidenceThreshold = 0.5f, float nmsThreshold = 0.55f)
        {
            float[] inputData = PreprocessToArray(mat);

            var inputTensor = _inferRequest.get_input_tensor();
            inputTensor.set_data(inputData);

            _inferRequest.infer();

            var outputTensor = _inferRequest.get_output_tensor();
            int outputSize = (int)outputTensor.get_size();
            float[] outputData = outputTensor.get_data<float>(outputSize);

            return Postprocess(outputData, confidenceThreshold, nmsThreshold, mat.Width, mat.Height);
        }
        private float[] PreprocessToArray(Mat mat)
        {
            int w = mat.Width;
            int h = mat.Height;
            _ratio = Math.Min((float)_inputWidth / w, (float)_inputHeight / h);
            int newUnpadW = (int)Math.Round(w * _ratio);
            int newUnpadH = (int)Math.Round(h * _ratio);
            _dw = (_inputWidth - newUnpadW) / 2f;
            _dh = (_inputHeight - newUnpadH) / 2f;

            int volImg = _inputHeight * _inputWidth;
            float[] chwData = new float[3 * volImg];
            using (Mat resized = new Mat())
            {
                Cv2.Resize(mat, resized, new Size(newUnpadW, newUnpadH));

                using (Mat padded = new Mat())
                {
                    int top = (int)Math.Round(_dh - 0.1);
                    int bottom = (int)Math.Round(_dh + 0.1);
                    int left = (int)Math.Round(_dw - 0.1);
                    int right = (int)Math.Round(_dw + 0.1);
                    Cv2.CopyMakeBorder(resized, padded, top, bottom, left, right, BorderTypes.Constant, new Scalar(114, 114, 114));

                    using (Mat rgb = new Mat())
                    {
                        Cv2.CvtColor(padded, rgb, ColorConversionCodes.BGR2RGB);

                        byte[] imageData = new byte[rgb.Total() * rgb.ElemSize()];
                        Marshal.Copy(rgb.Data, imageData, 0, imageData.Length);

                        for (int y = 0; y < _inputHeight; y++)
                        {
                            for (int x = 0; x < _inputWidth; x++)
                            {
                                int idx = (y * _inputWidth + x) * 3;
                                chwData[0 * volImg + y * _inputWidth + x] = imageData[idx + 0] / 255.0f;
                                chwData[1 * volImg + y * _inputWidth + x] = imageData[idx + 1] / 255.0f;
                                chwData[2 * volImg + y * _inputWidth + x] = imageData[idx + 2] / 255.0f;
                            }
                        }
                    }
                }
            }
            return chwData;
        }

        private List<Detection> Postprocess(float[] output, float confidenceThreshold, float nmsThreshold, int orgWidth, int orgHeight)
        {
            var detections = new List<Detection>();
            int numAnchors = output.Length / (4 + _numClasses);

            for (int i = 0; i < numAnchors; i++)
            {
                float maxScore = 0;
                int classId = -1;
                for (int c = 0; c < _numClasses; c++)
                {
                    float score = output[(4 + c) * numAnchors + i];
                    if (score > maxScore) { maxScore = score; classId = c; }
                }

                if (maxScore > confidenceThreshold)
                {
                    float cx = output[0 * numAnchors + i];
                    float cy = output[1 * numAnchors + i];
                    float w = output[2 * numAnchors + i];
                    float h = output[3 * numAnchors + i];

                    float x = (cx - w / 2f - _dw) / _ratio;
                    float y = (cy - h / 2f - _dh) / _ratio;
                    float width = w / _ratio;
                    float height = h / _ratio;

                    detections.Add(new Detection
                    {
                        ClassId = classId,
                        ClassName = classId < _classNames.Count ? _classNames[classId] : $"ID:{classId}",
                        Confidence = maxScore,
                        X = x,
                        Y = y,
                        Width = width,
                        Height = height
                    });
                }
            }
            return NonMaximumSuppression(detections, nmsThreshold);
        }

        private List<Detection> NonMaximumSuppression(List<Detection> detections, float threshold)
        {
            var sortedDetections = detections.OrderByDescending(d => d.Confidence).ToList();
            var results = new List<Detection>();
            var isActive = new bool[sortedDetections.Count];
            for (int i = 0; i < isActive.Length; i++) isActive[i] = true;

            for (int i = 0; i < sortedDetections.Count; i++)
            {
                if (!isActive[i]) continue;
                var best = sortedDetections[i];
                results.Add(best);
                for (int j = i + 1; j < sortedDetections.Count; j++)
                {
                    if (isActive[j] && CalculateIoU(best, sortedDetections[j]) > threshold)
                        isActive[j] = false;
                }
            }
            return results;
        }

        private float CalculateIoU(Detection a, Detection b)
        {
            float x1 = Math.Max(a.X, b.X);
            float y1 = Math.Max(a.Y, b.Y);
            float x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            float y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);
            float inter = Math.Max(0, x2 - x1) * Math.Max(0, y2 - y1);
            float union = (a.Width * a.Height) + (b.Width * b.Height) - inter;
            return inter / union;
        }

        public void Dispose()
        {
            _inferRequest?.Dispose();
            _compiledModel?.Dispose();
            _model?.Dispose();
            _core?.Dispose();
        }
    }
}