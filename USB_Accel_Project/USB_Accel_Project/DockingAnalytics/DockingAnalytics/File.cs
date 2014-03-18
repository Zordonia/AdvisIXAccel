using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;
using System.Numerics;

namespace DockingAnalytics
{

    public class File
    {
        public String FileName { get; set; }
        public AccelerationInfo AccelerationData { get; set; }
        public MagnitudeInfo MagnitudeData { get; set; }
        public PowerInfo PowerData { get; set; }
        public VelocityInfo VelocityData { get; set; }
        public List<CurveInfo> InfoList { get; set; }
        public _xmlFile XMLFile { get; set; }


        public File(String fileName)
        {
            FileName = fileName;
            XMLFile = new _xmlFile(fileName);
            ProcessFile();
            InfoList = new List<CurveInfo>();
            InfoList.Add(AccelerationData);
            InfoList.Add(MagnitudeData);
            InfoList.Add(PowerData);
            InfoList.Add(VelocityData);
        }

        //public File(_xmlFile file)
        //{
        //    FileName = file.fileName;
        //    XMLFile = file;
        //    MakeAccelerationCurve();
        //    MakeVelocityCurve();
        //    MakeFFTCurves();
        //    InfoList = new List<CurveInfo>();
        //    InfoList.Add(AccelerationData);
        //    InfoList.Add(MagnitudeData);
        //    InfoList.Add(PowerData);
        //    InfoList.Add(VelocityData);
        //}

        private void ProcessFile()
        {
            XMLFile.readXML();

            MakeAccelerationCurve();
            MakeVelocityCurve();
            MakeFFTCurves();
            //TODO This might need to be named something else
            //XMLFile.fileName = "TEST2.xml";
            //XMLFile.XmlWrite();
        }

        private void MakeAccelerationCurve()
        {
            PointPairList ppl = XMLFile.dsSentry_data.ACCPPL;

            double xS = XMLFile.dsSentry_data.accel_xCalibration;
            double yS = XMLFile.dsSentry_data.accel_yCalibration;
            double yOff = XMLFile.dsSentry_data.offset_Calibration;
            AccelerationData = new AccelerationInfo(ppl, xS, yS,yOff);
        }

        private void MakeVelocityCurve()
        {
            PointPairList ppl = new PointPairList();
            decimal vtn_1 = 0.0m;
            decimal vtn = 0.0m;
            decimal fres;
            if (XMLFile.dsSentry_data.sampling_Freq != 0)
            {
                fres = 1.0m / (decimal)XMLFile.dsSentry_data.sampling_Freq;
            }
            else { fres = 1; }
            decimal freq = 0.0m;
            double velXScale = 1, velYScale = 1;
            if (VelocityData != null)
            {
                velXScale = VelocityData.Scale.X;
                velYScale = VelocityData.Scale.Y;
            }
            PointPairList velVals = new PointPairList();
            for (int p = 0; p < AccelerationData.Values.Count; p++)
            {
                freq = p * fres;
                vtn_1 = vtn;
                vtn = (decimal)vtn_1 + AccelerationData.Values[p] * (decimal)(1.0 / 16); // V(tN) = V(t(N-1))+A(t(N-1))*dT(converted to in/s)
                velVals.Add((double)freq * velXScale, (double)vtn * velYScale);
            }
            VelocityData = new VelocityInfo(velVals, velXScale, velYScale);
        }

        private void MakeFFTCurves()
        {
            PointPairList fftPPL = new PointPairList();
            PointPairList pwrPPL = new PointPairList();

            decimal[] dcmFFTAnswers = new decimal[AccelerationData.Values.Count * 2];
            decimal[] dcmFFTMag;
            //decimal[] dcmFFTAnswers = new decimal[AccelerationData.Values.Count];
            for (int i = 0; i < AccelerationData.Values.Count; i++)
            {
                //dcmFFTAnswers[i] = AccelerationData.Values[i];
                dcmFFTAnswers[i * 2] = AccelerationData.Values[i];
                // Pad with imaginary numbers
                dcmFFTAnswers[i * 2 + 1] = 0.0m;
            }
            decimal[] testMagVals = new decimal[dcmFFTAnswers.Length / 2];

            if (FFT.IsPowerOfTwo(dcmFFTAnswers.Length/2))
            {
                decimal d = DockingAnalytics.FFT.test(dcmFFTAnswers);
                Console.WriteLine("THIS IS THE TEST RESULT!!!: " + d);
                FFT.transform(dcmFFTAnswers);
                dcmFFTMag = new decimal[AccelerationData.Values.Count /2];
            }
            else
            {
                // Pad with zeros, then FFT.transform(dcmFFTAnswers)
                int n = FFT.NextPowerOfTwo(dcmFFTAnswers.Length);
                //int n = FFT.NextPowerOfTwo(dcmFFTAnswers.Length );
                dcmFFTAnswers = FFT.PadWithZeros(dcmFFTAnswers, n);
                decimal d = DockingAnalytics.FFT.test(dcmFFTAnswers);
                Console.WriteLine("THIS IS THE TEST RESULT!!!: " + d);
                FFT.transform(dcmFFTAnswers);
                dcmFFTMag = new decimal[n/4];
            }

            Complex temp;
            Complex length = new Complex(AccelerationData.Values.Count, 0);
            List<Complex> compList = new List<Complex>();
            for (int i = 0; i < AccelerationData.Values.Count; i++)
            {
                temp = new Complex((double)dcmFFTAnswers[i * 2], (double)dcmFFTAnswers[i * 2 + 1]);
                dcmFFTAnswers[i] = (decimal)Complex.Abs(Complex.Divide(temp, length));// dcmFFTAnswers[i] / AccelerationData.Values.Count;
                temp = Complex.Abs(Complex.Divide(temp, length));
                compList.Add(temp);
            }
            double fres;
            dcmFFTMag[0] = dcmFFTAnswers[0];
            for (int i = 1; i < dcmFFTMag.Length-1; i++)
            {
                dcmFFTMag[i] = dcmFFTAnswers[i]*2;
            }
            dcmFFTMag[dcmFFTMag.Length-1] = dcmFFTAnswers[dcmFFTMag.Length-1];

            if (XMLFile.dsSentry_data.sampling_Freq != 0)
            {
                fres = XMLFile.dsSentry_data.sampling_Freq / dcmFFTAnswers.Length; // 1.0;
            }
            else
            {
                fres = 1.0 / dcmFFTAnswers.Length; // 1.0;
            }
            double freq = 0;
            decimal first = dcmFFTMag[0];
            decimal last = dcmFFTMag[dcmFFTMag.Length - 1];
            List<double> tempList = new List<double>();
            double[] pwrVals = new double[dcmFFTAnswers.Length];
            for (int i = 0; i < dcmFFTAnswers.Length / (2 * 2); i++)
            {
                freq = fres * (i * 2);
                //testMagVals[i] = (decimal)Math.Sqrt((double)dcmFFTAnswers[2 * i] * (double)dcmFFTAnswers[2 * i] + (double)dcmFFTAnswers[2 * i + 1] * (double)dcmFFTAnswers[2 * i + 1]);
                fftPPL.Add(1 * freq, (double)dcmFFTMag[i]);//(1 * (double)testMagVals[i])/(dcmFFTAnswers.Length/(4)));
                pwrVals[i] = (double)dcmFFTMag[i] * (double)dcmFFTMag[i];
                pwrPPL.Add(1 * freq, (1 * pwrVals[i]));// / (dcmFFTAnswers.Length / (4)));
                tempList.Add((double)testMagVals[i]);
            }
            // TODO Add mirror image of FFT
            //for(int i = testMagVals.Length-1; i >= 0; i--)
            //{
            //    int ind = testMagVals.Length - 1 - i+testMagVals.Length-1;
            //    freq = fres * (ind * 2);
            //    fftPPL.Add(1 * freq, 1 * (double)testMagVals[i]);
            //    pwrPPL.Add(1 * freq, 1 * pwrVals[i]);
            //}
            MagnitudeData = new MagnitudeInfo(fftPPL, 1, 1);
            MagnitudeData.Average = (double)dcmFFTMag.Average();
            PowerData = new PowerInfo(pwrPPL, 1, 1);

            XMLFile.dsSentry_data.fft_Values.values = tempList;
        }
    }
}
