namespace TransformExtChar.Model
{
    public interface ITransformerDatasheet
    {
        double I0 { get; set; }
        double I0_Percent { get; set; }
        double I1r { get; set; }
        double P0 { get; set; }
        double Psc { get; set; }
        ITransformerConfig TransformerConfig { get; }
        double U1r { get; set; }
        double U1sc { get; set; }
        double U1sc_Percent { get; set; }
        double U2r { get; set; }

        bool TryGetTransformer(out Transformer transformer);
    }
}