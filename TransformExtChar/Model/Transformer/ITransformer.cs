using System.Collections.Generic;

namespace TransformExtChar.Model
{
    public interface ITransformer
    {
        IEquivalentCurcuit EquivalentCurcuit { get; }
        ITransformerConfig TransformerConfig { get; }

        List<VCPointData> GetExternalCharacteristic(double fi2_rad = 0, double I2_correctedStart = 0, double I2_correctedEnd = 0, double U1 = 0, double I2_step = 0.01);
    }
}