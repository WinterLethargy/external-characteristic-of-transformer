using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace TransformExtChar.Model
{
    public interface IEquivalentCurcuit
    {
        double K { get; set; }
        double R1 { get; set; }
        double R2_Corrected { get; set; }
        double Rm { get; set; }
        double X1 { get; set; }
        double X2_Corrected { get; set; }
        double Xm { get; set; }
        Complex Z1 { get; set; }
        Complex Z2_Сorrected { get; set; }
        Complex Zm { get; set; }

        List<VCPointData> GetExternalCharacteristic(double fi2_rad = 0, double I2_correctedStart = 0, double I2_correctedEnd = 0, double U1 = 0, double I2_step = 0.01);
        Task<List<VCPointData>> GetExternalCharacteristicAsync(double fi2_rad = 0, double I2_correctedStart = 0, double I2_correctedEnd = 0, double U1 = 0, double I2_step = 0.01);
        List<VCPointData> GetFullExternalCharacteristic(double fi2_rad = 0, double U1 = 0, double I2_step = 0.01);
        Task<List<VCPointData>> GetFullExternalCharacteristicAsync(double fi2_rad = 0, double U1 = 0, double I2_step = 0.1);
    }
}