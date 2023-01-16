using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TransformExtChar.Model;

namespace TransformExtChar.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl2.xaml
    /// </summary>
    public partial class TransformerConfigSelector : UserControl
    {
        public List<TransformerTypeEnum> TransformerTypes { get; set; } = new List<TransformerTypeEnum>
        {
            TransformerTypeEnum.None,
            TransformerTypeEnum.OnePhase,
            TransformerTypeEnum.ThreePhase
        };
        public List<StarOrTriangleEnum> StarOrTriangles { get; set; } = new List<StarOrTriangleEnum>
        {
            StarOrTriangleEnum.None,
            StarOrTriangleEnum.Star,
            StarOrTriangleEnum.Triangle
        };
        public TransformerConfigSelector()
        {
            InitializeComponent();
        }
        void combobox_MouseWheel(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }
    }
}
