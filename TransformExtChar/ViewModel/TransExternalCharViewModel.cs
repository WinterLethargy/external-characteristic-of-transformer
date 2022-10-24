using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TransformExtChar.Infrastructure;
using TransformExtChar.Infrastructure.Command;
using TransformExtChar.Model;

namespace TransformExtChar.ViewModel
{
    internal class TransExternalCharViewModel : OnPropertyChangedClass
    {
        #region Команды

        #region Построить график внешней характерискики из Т схемы (CalcExtCharFromEquivalentCurcuitCommand)
        public ICommand CalcExtCharFromEquivalentCurcuitCommand { get; }

        private void CalcExtCharFromEquivalentCurcuit_Executed(object p)
        {
            Task.Run(() =>
            {
                var newCharacteristic = CalcExtChar();
                PlotterVM.UpdateEditedSeries(newCharacteristic);   // посчитать график
            });
        }
        private List<VCPointData> CalcExtChar()
        {
            const double toRad = Math.PI / 180;
            return Transformer.GetExternalCharacteristic(Fi2 * toRad, I2_start, I2_end, U1, I2_step);
        }
        private bool CalcExtCharFromEquivalentCurcuit_CanExecuted(object p) => Transformer.EquivalentCurcuit.Error == string.Empty && Transformer.TransformerConfig.Error == string.Empty && ModeParameters.Error == string.Empty;
        #endregion

        #region Построить график внешней характерискики из паспортных данных (CalcExtCharFromEquivalentCurcuitCommand)
        public ICommand CalcExtCharFromDataSheetCommand { get; }

        private void CalcExtCharFromDataSheet_Executed(object p)
        {
            Task.Run(() =>
            {
                if (!DataSheet.TryGetTransformer(out var transformer))
                {
                    PlotterVM.UpdateEditedSeries(new List<VCPointData>());
                    return;
                }

                const double toRad = Math.PI / 180;
                var newCharacteristic = transformer.GetExternalCharacteristic(Fi2 * toRad, I2_start, I2_end, U1, I2_step);
                PlotterVM.UpdateEditedSeries(newCharacteristic);
            });
        }
        private bool CalcExtCharFromDataSheet_CanExecuted(object p) => DataSheet.Error == string.Empty && DataSheet.TransformerConfig.Error == string.Empty && ModeParameters.Error == string.Empty;

        #endregion

        #region Посчитать параметры схемы замещения из паспортных данных трансформатора (CalcParamFromDataSheetCommand)
        public ICommand CalcTransformerFromDataSheetCommand { get; }
        public void CalcTransformerFromDataSheet_Executed(object p)
        {
            if (DataSheet.TryGetTransformer(out var transformer))
                Transformer = new DataErrorInfoTransformer(transformer);
        }
        private bool CalcTransformerFromDataSheet_CanExecuted(object p) => DataSheet.Error == string.Empty && DataSheet.TransformerConfig.Error == string.Empty;
        #endregion

        #region Сохранение в файл и загрузка из файла трансформатора и его паспортных данных

        #region Сохранить параметры трансформатора в файл

        public ICommand SaveTransToJsonCommand { get; }

        private void SaveTransToJson_Executed(object p)
        {
            Action<string> serialize;

            if (p == Transformer)
                serialize = (path) => File.WriteAllText(path, JsonConvert.SerializeObject(Transformer, Formatting.Indented, new ComplexJsonConverter()));
            else if (p == DataSheet)
                serialize = (path) => File.WriteAllText(path, JsonConvert.SerializeObject(DataSheet, Formatting.Indented));
            else throw new Exception();

            var dlg = new SaveFileDialog
            {
                Filter = "JSON|*.json;*.txt",
                DefaultExt = ".json"
            };
            if (dlg.ShowDialog().Value)
            {
                serialize(dlg.FileName);
            }
        }

        #endregion

        #region Загрузить трансформатор из файла

        public ICommand OpenTransFromJsonCommand { get; }

        private void OpenTransFromJson_Executed(object p)
        {
            Action<string> deserialize;
            bool deserializeIsSuccess = true;

            var setting = new JsonSerializerSettings();
            setting.Error = (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args) =>
            {
                if (deserializeIsSuccess)
                {
                    MessageBox.Show("Неправильно сформатированн файл!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    deserializeIsSuccess = false;
                }
                    args.ErrorContext.Handled = true;
            };

            if (p == Transformer)
            {
                setting.Converters.Add(new ComplexJsonConverter());
                deserialize = (path) =>
                {
                    var transformer = JsonConvert.DeserializeObject<Transformer>(File.ReadAllText(path), setting);
                    if (deserializeIsSuccess)
                        Transformer = new DataErrorInfoTransformer(transformer);
                };
            }
            else if (p == DataSheet)
                deserialize = (path) =>
                {
                    var dataDheet = JsonConvert.DeserializeObject<TransformerDatasheet>(File.ReadAllText(path), setting);
                    if (deserializeIsSuccess)
                        DataSheet = new DataErrorInfoTransformerDatasheet(dataDheet);
                };
            else throw new Exception();

            var dlg = new OpenFileDialog 
            { 
                Filter = "JSON|*.json;*.txt", 
                DefaultExt = ".json" 
            };

            if (dlg.ShowDialog().Value)
            {
                deserialize(dlg.FileName);
            }
        }

        #endregion

        #endregion

        #region Очистить поля трансформатора или паспортных данных

        public ICommand ClearFieldsCommand { get; }

        private void ClearFields_Executed(object p)
        {
            Action update;
            if (p == Transformer)
            {
                update = () => Transformer = new DataErrorInfoTransformer();
            }
            else if (p == DataSheet)
            {
                update = () => DataSheet = new DataErrorInfoTransformerDatasheet();
            }
            else throw new Exception();

            var result = MessageBox.Show("Очистить поля?", "Очистить поля?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if(result == MessageBoxResult.Yes) update();
        }

        #endregion

        #endregion

        private DataErrorInfoTransformer _transformer;
        public DataErrorInfoTransformer Transformer
        {
            get => _transformer;
            set
            {
                Set(ref _transformer, value);
            }
        }
        private DataErrorInfoTransformerDatasheet _dataSheet;
        public DataErrorInfoTransformerDatasheet DataSheet 
        {
            get => _dataSheet;
            set
            {
                Set(ref _dataSheet, value);
            }
        }
        public ModeParameters ModeParameters { get; }

        private bool _fullChar;
        public bool FullChar
        {
            get => _fullChar;
            set 
            {
                if (value)
                {
                    I2_start = 0;
                    I2_end = 0;
                }
            }
        }
        private double U1 { get => ModeParameters.U1; set => ModeParameters.U1 = value; }
        private double Fi2 { get => ModeParameters.Fi2; set => ModeParameters.Fi2 = value; }
        private double I2_start { get => ModeParameters.I2_start; set => ModeParameters.I2_start = value; }
        private double I2_end { get => ModeParameters.I2_end; set => ModeParameters.I2_end = value; }
        private double I2_step { get => ModeParameters.I2_step; set => ModeParameters.I2_step = value; }

        public PlotterViewModel PlotterVM { get; set; } = new PlotterViewModel();

        #region Конструкторы
        public TransExternalCharViewModel()
        {
            Transformer = new DataErrorInfoTransformer( new Transformer(
                                                            new DataErrorInfoEquivalentCurcuit()
                                                            {
                                                                R1 = 0.67,
                                                                X1 = 0.11,
                                                                R2_Corrected = 0.67,
                                                                X2_Corrected = 0.11,
                                                                Rm = 45.013,
                                                                Xm = 285.95,
                                                                K = 1.91
                                                            },
                                                            new DataErrorInfoTransformerConfig()));

            DataSheet = new DataErrorInfoTransformerDatasheet() { U1r = 220, U2r = 115, I1r = 7.3, I0 = 0.76, P0 = 26, U1sc = 10, Psc = 72 };
            ModeParameters = new ModeParameters();

            CalcExtCharFromEquivalentCurcuitCommand = new RelayCommand(CalcExtCharFromEquivalentCurcuit_Executed, CalcExtCharFromEquivalentCurcuit_CanExecuted, "Построить внешнюю характеристику из Т схемы замещения");
            CalcExtCharFromDataSheetCommand = new RelayCommand(CalcExtCharFromDataSheet_Executed, CalcExtCharFromDataSheet_CanExecuted, "Построить внешнюю характеристику из паспортных данных");
            CalcTransformerFromDataSheetCommand = new RelayCommand(CalcTransformerFromDataSheet_Executed, CalcTransformerFromDataSheet_CanExecuted, "Пересчитать Т схему замещения");
            SaveTransToJsonCommand = new RelayCommand(SaveTransToJson_Executed, null, "Сохранить в файл");
            OpenTransFromJsonCommand = new RelayCommand(OpenTransFromJson_Executed, null, "Загрузить из файла");
            ClearFieldsCommand = new RelayCommand(ClearFields_Executed, null, "Очистить поля");
        }
        #endregion
    }
}
