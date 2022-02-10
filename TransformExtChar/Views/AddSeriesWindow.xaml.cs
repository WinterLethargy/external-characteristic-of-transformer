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
using System.Windows.Shapes;
using TransformExtChar.Infrastructure;
using TransformExtChar.ViewModel;

namespace TransformExtChar.Views
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class AddSeriesWindow : Window
    {
        public AddSeriesWindow(MessageEnum fixedOrUsers)
        {
            InitializeComponent();
            DataContext = new AddSeriesVM(fixedOrUsers);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBus.Bus += Close;
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            MessageBus.Bus -= Close;
        }
        private void Window_Activated(object? sender, EventArgs e)
        {
            text.SelectAll();
            text.Focus();
        }
        private void Close(object message)
        {
            if (!(message is MessageEnum mes))
                return;

            if (mes == MessageEnum.AddSeries_Close && IsVisible == true)
                Close();
        }
    }
}
