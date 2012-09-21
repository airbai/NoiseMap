namespace DBMeasurer.Rules
{
    using System;

    public class FreqAnalyzer
    {
        public static double[] Cmp2Mdl(Complex[] input)
        {
            double[] numArray = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].Real > 0.0)
                {
                    numArray[i] = -input[i].ToModul();
                }
                else
                {
                    numArray[i] = input[i].ToModul();
                }
            }
            return numArray;
        }

        private static Complex[] FFT(Complex[] input, bool invert)
        {
            if (input.Length == 1)
            {
                return new Complex[] { input[0] };
            }
            int length = input.Length;
            int num2 = length / 2;
            Complex[] complexArray = new Complex[length];
            double num3 = -6.2831853071795862 / ((double) length);
            if (invert)
            {
                num3 = -num3;
            }
            Complex[] complexArray2 = new Complex[num2];
            for (int i = 0; i < num2; i++)
            {
                complexArray2[i] = input[2 * i];
            }
            Complex[] complexArray3 = FFT(complexArray2, invert);
            Complex[] complexArray4 = new Complex[num2];
            for (int j = 0; j < num2; j++)
            {
                complexArray4[j] = input[(2 * j) + 1];
            }
            Complex[] complexArray5 = FFT(complexArray4, invert);
            for (int k = 0; k < num2; k++)
            {
                double num7 = num3 * k;
                Complex complex = complexArray5[k] * new Complex(Math.Cos(num7), Math.Sin(num7));
                complexArray[k] = complexArray3[k] + complex;
                complexArray[k + num2] = complexArray3[k] - complex;
            }
            return complexArray;
        }

        public static Complex[] FFT(double[] input, bool invert)
        {
            Complex[] complexArray = new Complex[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                complexArray[i] = new Complex(input[i]);
            }
            return (complexArray = FFT(complexArray, invert));
        }

        private static double[] FreqFilter(double[] data, int Nc, Complex[] Hd)
        {
            int length = data.Length;
            Complex[] input = FFT(data, false);
            for (int i = 0; i < length; i++)
            {
                input[i] = Hd[i] * input[i];
            }
            data = Cmp2Mdl(FFT(input, true));
            for (int j = 0; j < length; j++)
            {
                data[j] = -data[j] / ((double) length);
            }
            return data;
        }

        public static uint GetLowerPowerOfTwo(uint top)
        {
            uint num = 2;
            while (num < top)
            {
                num = num << 1;
            }
            return (num >> 1);
        }

        public static uint GetUperPowerOfTow(uint bottom)
        {
            uint num = 2;
            while (num < bottom)
            {
                num = num << 1;
            }
            return num;
        }

        public static bool IsPowerOfTwo(uint p_nX)
        {
            if (p_nX < 2)
            {
                return false;
            }
            if ((p_nX & (p_nX - 1)) != null)
            {
                return false;
            }
            return true;
        }
    }
}

