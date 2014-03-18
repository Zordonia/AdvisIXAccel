using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DockingAnalytics
{
    public class FFT
    {
﻿  ﻿  

        public enum WindowType
        {
            Hamming, 
            Hanning,
            Normal
        }

        public static WindowType Window = WindowType.Hamming;

    ﻿  ﻿  public static decimal num_flops(int N)
    ﻿  ﻿  {
    ﻿  ﻿  ﻿  decimal Nd = (decimal)N;
    ﻿  ﻿  ﻿  decimal logN = (decimal)log2(N);
    ﻿  ﻿  ﻿  
    ﻿  ﻿  ﻿  return (5.0m * Nd - 2) * logN + 2 * (Nd + 1);
    ﻿  ﻿  }
﻿  ﻿  
        public static decimal[] Hamming(decimal[] iwv)
        {
          int N = iwv.Length;
          for (int n = 0; n < N; n += 2)
              iwv[n] *= (decimal)0.54d - (decimal)0.46 * (decimal)Math.Cos((2 * Math.PI * n) / (N - 1));

          return iwv;
        }

        public static decimal[] Hann(decimal[] iwv)
        {
            int N = iwv.Length;

            for (int n = 1; n < N; n += 2)
                iwv[n] *= (decimal)0.5 - (decimal)0.5 * (decimal)Math.Cos((2 * Math.PI * n) / (N - 1));

            return iwv;
        }
﻿  ﻿  
﻿  ﻿  /// <summary>
﻿  ﻿  /// Compute Fast Fourier Transform of (complex) data, in place.
﻿  ﻿  /// </summary>
﻿  ﻿  public static void transform(decimal[] data)
﻿  ﻿  {
          switch (Window)
          {
              case WindowType.Normal:
                  Window = WindowType.Hamming;
                  break;
              case WindowType.Hamming:
                  Window = WindowType.Hanning;
                  Hamming(data);
                  break;
              case WindowType.Hanning:
                  Window = WindowType.Normal;
                  Hann(data);
                  break;
          }

﻿  ﻿  ﻿      transform_internal(data, -1);
﻿  ﻿  }
﻿  ﻿  
﻿  ﻿  /// <summary>
﻿  ﻿  /// Compute Inverse Fast Fourier Transform of (complex) data, in place.
﻿  ﻿  /// </summary>
﻿  ﻿  public static void inverse(decimal[] data)
﻿  ﻿  {
﻿  ﻿  ﻿  transform_internal(data, +1);
﻿  ﻿  ﻿  // Normalize
﻿  ﻿  ﻿  int nd = data.Length;
﻿  ﻿  ﻿  int n = nd / 2;
﻿  ﻿  ﻿  decimal norm = 1 / ((decimal)n);
﻿  ﻿  ﻿   for (int i = 0; i < nd; i++)
﻿  ﻿  ﻿  ﻿  data[i] *= norm;
﻿  ﻿  }
﻿  ﻿  
﻿  ﻿  /// <summary>
﻿  ﻿  /// Accuracy check on FFT of data. Make a copy of data, Compute the FFT, then
﻿  ﻿  /// the inverse and compare to the original.  Returns the rms difference.
﻿  ﻿  /// </summary>
﻿  ﻿  public static decimal test(decimal[] data)
﻿  ﻿  {
﻿  ﻿  ﻿  int nd = data.Length;
﻿  ﻿  ﻿  // Make duplicate for comparison
﻿  ﻿  ﻿  decimal[] copy = new decimal[nd];
﻿  ﻿  ﻿  Array.Copy(data, 0, copy, 0, nd);
﻿  ﻿  ﻿  // Transform & invert
﻿  ﻿  ﻿  transform(data);
﻿  ﻿  ﻿  inverse(data);
﻿  ﻿  ﻿  // Compute RMS difference.
﻿  ﻿  ﻿  decimal diff = 0.0m;
﻿  ﻿  ﻿   for (int i = 0; i < nd; i++)
﻿  ﻿  ﻿  {
﻿  ﻿  ﻿  ﻿  decimal d = data[i] - copy[i];
﻿  ﻿  ﻿  ﻿  diff += d * d;
﻿  ﻿  ﻿  }
﻿  ﻿  ﻿  return (decimal)Math.Sqrt((double)diff / (double)nd);
﻿  ﻿  }

      public static decimal[] PadWithZeros(decimal[] array, int n)
      {
          decimal[] newArray = new decimal[n];
          if (n > array.Length)
          { // Pad up
              for (int i = 0; i < array.Length; i++)
              {
                  newArray[i] = array[i];
              }
          }
          else
          { // Pad Down
              for (int j = 0; j < n; j++)
              {
                  newArray[j] = array[j];
              }
          }
          return newArray;
      }

      public static bool IsPowerOfTwo(int n)
      {
          int log = 0;
﻿  ﻿  ﻿     for (int k = 1; k < n; k *= 2, log++)
﻿  ﻿  ﻿  ﻿      ;
﻿  ﻿  ﻿         if (n != (1 << log))
                {
                    return false;
                }
                else
                {
                    return true;
                }
      }

      public static int NextPowerOfTwo(int n)
      {
          int higher = 1;
          int orig = n;
          while (n > 0)
          {
              n = n >> 1;
              higher = higher << 1;
          }
          if (orig * 2 == higher)
          {
              return orig;
          }
          else
          {
              return higher;
          }
      }
﻿  ﻿  
﻿  ﻿  /// <summary>
﻿  ﻿  /// Make a random array of n (complex) elements. 
﻿  ﻿  /// </summary>
﻿  ﻿  public static decimal[] makeRandom(int n)
﻿  ﻿  {
﻿  ﻿  ﻿  int nd = 2 * n;
﻿  ﻿  ﻿  decimal[] data = new decimal[nd];
﻿  ﻿  ﻿  System.Random r = new System.Random();
﻿  ﻿  ﻿  for (int i = 0; i < nd; i++)
﻿  ﻿  ﻿  ﻿  data[i] = (decimal)r.NextDouble();
﻿  ﻿  ﻿  return data;
﻿  ﻿  }
﻿  ﻿  
﻿  ﻿  protected internal static int log2(int n)
﻿  ﻿  {
﻿  ﻿  ﻿  int log = 0;
﻿  ﻿  ﻿   for (int k = 1; k < n; k *= 2, log++)
﻿  ﻿  ﻿  ﻿  ;
﻿  ﻿  ﻿  if (n != (1 << log))
﻿  ﻿  ﻿  ﻿  throw new ApplicationException("FFT: Data length is not a power of 2!: " + n);
﻿  ﻿  ﻿  return log;
﻿  ﻿  }
﻿  ﻿  
﻿  ﻿  protected internal static void transform_internal(decimal[] data, int direction)
﻿  ﻿  {
﻿  ﻿  ﻿  if (data.Length == 0)
﻿  ﻿  ﻿  ﻿  return;
﻿  ﻿  ﻿  int n = data.Length / 2;
﻿  ﻿  ﻿  if (n == 1)
﻿  ﻿  ﻿  ﻿  return;
﻿  ﻿  ﻿  // Identity operation!
﻿  ﻿  ﻿  int logn = log2(n);
﻿  ﻿  ﻿  
﻿  ﻿  ﻿  /* bit reverse the input data for decimation in time algorithm */
﻿  ﻿  ﻿  bitreverse(data);
﻿  ﻿  ﻿  
﻿  ﻿  ﻿  /* apply fft recursion */
﻿  ﻿  ﻿  /* this loop executed log2(N) times */
﻿  ﻿  ﻿   for (int bit = 0, dual = 1; bit < logn; bit++, dual *= 2)
﻿  ﻿  ﻿  {
﻿  ﻿  ﻿  ﻿  decimal w_real = 1.0m;
﻿  ﻿  ﻿  ﻿  decimal w_imag = 0.0m;
﻿  ﻿  ﻿  ﻿  
﻿  ﻿  ﻿  ﻿  decimal theta = (decimal)2.0 * direction * (decimal)Math.PI / ((decimal)2.0 * (decimal)dual);
﻿  ﻿  ﻿  ﻿  decimal s = (decimal)Math.Sin((double)theta);
﻿  ﻿  ﻿  ﻿  decimal t = (decimal)Math.Sin((double)theta / 2.0);
﻿  ﻿  ﻿  ﻿  decimal s2 = 2.0m * t * t;
﻿  ﻿  ﻿  ﻿  
﻿  ﻿  ﻿  ﻿  /* a = 0 */
﻿  ﻿  ﻿  ﻿   for (int b = 0; b < n; b += 2 * dual)
﻿  ﻿  ﻿  ﻿  {
﻿  ﻿  ﻿  ﻿  ﻿  int i = 2 * b;
﻿  ﻿  ﻿  ﻿  ﻿  int j = 2 * (b + dual);
﻿  ﻿  ﻿  ﻿  ﻿  
﻿  ﻿  ﻿  ﻿  ﻿  decimal wd_real = data[j];
﻿  ﻿  ﻿  ﻿  ﻿  decimal wd_imag = data[j + 1];
﻿  ﻿  ﻿  ﻿  ﻿  
﻿  ﻿  ﻿  ﻿  ﻿  data[j] = data[i] - wd_real;
﻿  ﻿  ﻿  ﻿  ﻿  data[j + 1] = data[i + 1] - wd_imag;
﻿  ﻿  ﻿  ﻿  ﻿  data[i] += wd_real;
﻿  ﻿  ﻿  ﻿  ﻿  data[i + 1] += wd_imag;
﻿  ﻿  ﻿  ﻿  }
﻿  ﻿  ﻿  ﻿  
﻿  ﻿  ﻿  ﻿  /* a = 1 .. (dual-1) */
﻿  ﻿  ﻿  ﻿   for (int a = 1; a < dual; a++)
﻿  ﻿  ﻿  ﻿  {
﻿  ﻿  ﻿  ﻿  ﻿  /* trignometric recurrence for w-> exp(i theta) w */
﻿  ﻿  ﻿  ﻿  ﻿  {
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  decimal tmp_real = w_real - s * w_imag - s2 * w_real;
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  decimal tmp_imag = w_imag + s * w_real - s2 * w_imag;
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  w_real = tmp_real;
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  w_imag = tmp_imag;
﻿  ﻿  ﻿  ﻿  ﻿  }
﻿  ﻿  ﻿  ﻿  ﻿   for (int b = 0; b < n; b += 2 * dual)
﻿  ﻿  ﻿  ﻿  ﻿  {
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  int i = 2 * (b + a);
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  int j = 2 * (b + a + dual);
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  decimal z1_real = data[j];
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  decimal z1_imag = data[j + 1];
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  decimal wd_real = w_real * z1_real - w_imag * z1_imag;
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  decimal wd_imag = w_real * z1_imag + w_imag * z1_real;
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  data[j] = data[i] - wd_real;
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  data[j + 1] = data[i + 1] - wd_imag;
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  data[i] += wd_real;
﻿  ﻿  ﻿  ﻿  ﻿  ﻿  data[i + 1] += wd_imag;
﻿  ﻿  ﻿  ﻿  ﻿  }
﻿  ﻿  ﻿  ﻿  }
﻿  ﻿  ﻿  }
﻿  ﻿  }
﻿  ﻿  
﻿  ﻿  
﻿  ﻿  protected internal static void bitreverse(decimal[] data)
﻿  ﻿  {
﻿  ﻿  ﻿  /* This is the Goldrader bit-reversal algorithm */
﻿  ﻿  ﻿  int n = data.Length / 2;
﻿  ﻿  ﻿  int nm1 = n - 1;
﻿  ﻿  ﻿  int i = 0;
﻿  ﻿  ﻿  int j = 0;
﻿  ﻿  ﻿   for (; i < nm1; i++)
﻿  ﻿  ﻿  {
﻿  ﻿  ﻿  ﻿  
﻿  ﻿  ﻿  ﻿  //int ii = 2*i;
﻿  ﻿  ﻿  ﻿  int ii = i << 1;
﻿  ﻿  ﻿  ﻿  
﻿  ﻿  ﻿  ﻿  //int jj = 2*j;
﻿  ﻿  ﻿  ﻿  int jj = j << 1;
﻿  ﻿  ﻿  ﻿  
﻿  ﻿  ﻿  ﻿  //int k = n / 2 ;
﻿  ﻿  ﻿  ﻿  int k = n >> 1;
﻿  ﻿  ﻿  ﻿  
﻿  ﻿  ﻿  ﻿  if (i < j)
﻿  ﻿  ﻿  ﻿  {
﻿  ﻿  ﻿  ﻿  ﻿  decimal tmp_real = data[ii];
﻿  ﻿  ﻿  ﻿  ﻿  decimal tmp_imag = data[ii + 1];
﻿  ﻿  ﻿  ﻿  ﻿  data[ii] = data[jj];
﻿  ﻿  ﻿  ﻿  ﻿  data[ii + 1] = data[jj + 1];
﻿  ﻿  ﻿  ﻿  ﻿  data[jj] = tmp_real;
﻿  ﻿  ﻿  ﻿  ﻿  data[jj + 1] = tmp_imag;
﻿  ﻿  ﻿  ﻿  }
﻿  ﻿  ﻿  ﻿  
﻿  ﻿  ﻿  ﻿  while (k <= j)
﻿  ﻿  ﻿  ﻿  {
﻿  ﻿  ﻿  ﻿  ﻿  //j = j - k ;
﻿  ﻿  ﻿  ﻿  ﻿  j -= k;
﻿  ﻿  ﻿  ﻿  ﻿  
﻿  ﻿  ﻿  ﻿  ﻿  //k = k / 2 ; 
﻿  ﻿  ﻿  ﻿  ﻿  k >>= 1;
﻿  ﻿  ﻿  ﻿  }
﻿  ﻿  ﻿  ﻿  j += k;
﻿  ﻿  ﻿  }
﻿  ﻿  }
﻿  }
}
