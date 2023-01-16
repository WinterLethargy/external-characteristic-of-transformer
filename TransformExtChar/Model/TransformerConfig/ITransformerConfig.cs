namespace TransformExtChar.Model
{
    public interface ITransformerConfig
    {
        StarOrTriangleEnum FirstWinding { get; set; }
        StarOrTriangleEnum SecondWinding { get; set; }
        TransformerTypeEnum TransformerType { get; set; }
    }
}