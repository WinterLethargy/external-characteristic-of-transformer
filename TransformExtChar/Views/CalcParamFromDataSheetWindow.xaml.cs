using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TransformExtChar.Infrastructure;
using TransformExtChar.Model;
using TransformExtChar.ViewModel;

namespace TransformExtChar.Views
{
    public partial class CalcParamFromDataSheetWindow : Window
    {

        public CalcParamFromDataSheetWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBus.Bus += Close;
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            MessageBus.Bus -= Close;
        }
        private void Close(object message, object p)
        {
            if (!(message is MessageEnum mes))
                return;

            if (mes == MessageEnum.CalcParamFromDataSheet_Close && IsVisible == true)
                Close();
        }
    }
}