using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TransformExtChar.Model
{
    public class TransformerDatasheetSpecifications
    {
        #region Property
        public double U1_Rated { get; set; }
        public double U2_Rated { get; set; }
        public double I1_Rated { get; set; }
        public double I1_Unload { get; set; }
        public double P_Unload { get; set; }
        public double U1_ShortCircuit { get; set; } // при номинальном токе первичное обмотки
        public double P_ShortCircuit { get; set; }
        #endregion

        public bool TryGetEquivalentCurcuitParameters(out (Complex Zm, Complex Z1, Complex Z2_Сorrected, double K) equivalentCurcuitParameters)
        {
            equivalentCurcuitParameters = (0, 0, 0, 0);
            if (U1_Rated <= 0 || U2_Rated <= 0 || I1_Unload <= 0 || I1_Rated <= 0 || U1_ShortCircuit <= 0 || U1_Rated < U1_ShortCircuit) return false;

            double Z0 = U1_Rated / I1_Unload;                               //полное сопротивление намагничивающей ветви
            double R0 = P_Unload / Math.Pow(I1_Unload, 2);                  //активное сопротивление намагничивающей ветви
            double X0 = Math.Sqrt(Math.Pow(Z0, 2) - Math.Pow(R0, 2));       //реактивное сопротивление намагничивающей ветви
            if (double.IsNaN(X0)) 
                return false;                                               //активная мощность не может быть больше полной мощности (P_Unload < U1_Rated * I1_Unload) 
            double Z_ShortCircuit = U1_ShortCircuit / I1_Rated;             //полное сопротивление короткого замыкания
            double R_ShortCircuit = P_ShortCircuit / Math.Pow(I1_Rated, 2); //активное сопротивление короткого замыкания
            double X_ShortCircuit = Math.Sqrt(Math.Pow(Z_ShortCircuit, 2) - Math.Pow(R_ShortCircuit, 2)); //реактивное сопротивление короткого замыкания
            if (double.IsNaN(X_ShortCircuit)) 
                return false;                                               //активная мощность не может быть больше полной мощности (P_ShortCircuit < U1_ShortCircuit * I1_Rated)
            double R12 = R_ShortCircuit / 2;                                //активное сопротивление рассеяния
            double X12 = X_ShortCircuit / 2;                                //реактивное сопротивление рассеяния
            double K = U1_Rated / U2_Rated;                                 //коэффициент трансформации

            Complex Zm = new Complex(R0, X0);
            Complex Z1 = new Complex(R12, X12);
            Complex Z2_Corrected = new Complex(R12, X12);

            equivalentCurcuitParameters = (Zm, Z1, Z2_Corrected, K);
            return true;
        }
    }
}

