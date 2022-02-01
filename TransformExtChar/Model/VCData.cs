using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TransformExtChar.Model
{
    public struct VCData
    {
        public double Current { get; set; }
        public double Voltage { get; set; }
        public Complex Z_load { get; set; }
    }
}
