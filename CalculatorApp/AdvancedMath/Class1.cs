using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace AdvancedMath
{
    public static class Adv
    {
        // basic
        public static double Add(double a, double b) => a + b;
        public static double Subtract(double a, double b) => a - b;
        public static double Multiply(double a, double b) => a * b;
        public static double Divide(double a, double b) => b == 0 ? double.NaN : a / b;

        // unary
        public static double Sqrt(double x) => x < 0 ? double.NaN : Math.Sqrt(x);
        public static double Square(double x) => x * x;
        public static double Reciprocal(double x) => x == 0 ? double.NaN : 1.0 / x;
        public static double Negate(double x) => -x;

        // scientific
        public static double Pow(double a, double b) => Math.Pow(a, b);
        public static double PercentOf(double a, double b) => a * b / 100.0; // "b% of a"
        public static double Log10(double x) => x <= 0 ? double.NaN : Math.Log10(x);
        public static double Ln(double x) => x <= 0 ? double.NaN : Math.Log(x);
        public static double Exp(double x) => Math.Exp(x);

        // trig in Degrees
        private static double D2R(double d) => d * Math.PI / 180.0;
        public static double SinDeg(double d) => Math.Sin(D2R(d));
        public static double CosDeg(double d) => Math.Cos(D2R(d));
        public static double TanDeg(double d) => Math.Tan(D2R(d));

        // simple factorial with guard (n=0..20)
        public static double Factorial(double x)
        {
            if (x < 0) return double.NaN;
            int n = (int)Math.Round(x);
            if (Math.Abs(x - n) > 1e-9) return double.NaN; // not an integer
            if (n > 20) return double.PositiveInfinity;
            long f = 1; for (int i = 2; i <= n; i++) f *= i;
            return f;
        }
    }
}

