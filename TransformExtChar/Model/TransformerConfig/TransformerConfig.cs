using System;
using System.Collections.Generic;
using System.Text;

namespace TransformExtChar.Model
{
    public class TransformerConfig : ITransformerConfig
    {
        public TransformerTypeEnum TransformerType { get; set; }
        public StarOrTriangleEnum FirstWinding { get; set; }
        public StarOrTriangleEnum SecondWinding { get; set; }
    }
}
