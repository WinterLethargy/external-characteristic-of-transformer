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
        private bool CalcExtCharFromEquivalentCurcuit_CanExecuted(object p) => Transformer.EquivalentCurcuit.Error == string.Empty;
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
        private bool CalcExtCharFromDataSheet_CanExecuted(object p) => DataSheet.Error == string.Empty;

        #endregion

        #region Посчитать параметры схемы замещения из паспортных данных трансформатора (CalcParamFromDataSheetCommand)
        public ICommand CalcTransformerFromDataSheetCommand { get; }
        public void CalcTransformerFromDataSheet_Executed(object p)
        {
            if (DataSheet.TryGetTransformer(out var transformer))
                Transformer = transformer;
        }
        private bool CalcTransformerFromDataSheet_CanExecuted(object p) => DataSheet.Error == string.Empty;
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
            setting.Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args) 
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
                        Transformer =  transformer;
                };
            }
            else if (p == DataSheet)
                deserialize = (path) =>
                {
                    var dataDheet = JsonConvert.DeserializeObject<TransformerDatasheet>(File.ReadAllText(path), setting);
                    if (deserializeIsSuccess)
                        DataSheet = dataDheet;
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

        

        #endregion

        #region Тип трансформатора и обмоток
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


        private TransformerTypeEnum _transformerType;
        public TransformerTypeEnum TransformerType
        {
            get => _transformerType;
            set
            {
                Set(ref _transformerType, value);

                Transformer.TransformerType = TransformerType;
                DataSheet.TransformerType = TransformerType;

                if (value != TransformerTypeEnum.ThreePhase)
                {
                    FirstWinding = StarOrTriangles[0];
                    SecondWinding = StarOrTriangles[0];
                    StarOrTriangleEnabled = false;
                }
                else
                    StarOrTriangleEnabled = true;
            }
        }

        private StarOrTriangleEnum _firstWinding;
        public StarOrTriangleEnum FirstWinding
        {
            get => _firstWinding;
            set
            {
                Set(ref _firstWinding, value);

                Transformer.FirstWinding = FirstWinding;
                DataSheet.FirstWinding = FirstWinding;
            }
        }

        private StarOrTriangleEnum _secondWinding;
        public StarOrTriangleEnum SecondWinding
        {
            get => _secondWinding;
            set
            {
                Set(ref _secondWinding, value);

                Transformer.SecondWinding = SecondWinding;
                DataSheet.SecondWinding = SecondWinding;
            }
        }

        private bool _starOrTriangleEnabled;
        public bool StarOrTriangleEnabled
        {
            get => _starOrTriangleEnabled;
            set => Set(ref _starOrTriangleEnabled, value);
        }
        #endregion

        private Transformer _transformer = new Transformer(new EquivalentCurcuit()
                                                           {
                                                               R1 = 0.67,
                                                               X1 = 0.11,
                                                               R2_Corrected = 0.67,
                                                               X2_Corrected = 0.11,
                                                               Rm = 45.013,
                                                               Xm = 285.95,
                                                               K = 1.91
                                                           });
        public Transformer Transformer
        {
            get => _transformer;
            set => Set(ref _transformer, value);
        }
        private TransformerDatasheet _dataSheet = new TransformerDatasheet();
        public TransformerDatasheet DataSheet 
        {
            get => _dataSheet; 
            set => Set(ref _dataSheet, value);
        }
        public ModeParameters ModeParameters { get; set; } = new ModeParameters();
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
            CalcExtCharFromEquivalentCurcuitCommand = new RelayCommand(CalcExtCharFromEquivalentCurcuit_Executed, CalcExtCharFromEquivalentCurcuit_CanExecuted, "Построить внешнюю характеристику из Т схемы замещения");
            CalcExtCharFromDataSheetCommand = new RelayCommand(CalcExtCharFromDataSheet_Executed, CalcExtCharFromDataSheet_CanExecuted, "Построить внешнюю характеристику из паспортных данных");
            CalcTransformerFromDataSheetCommand = new RelayCommand(CalcTransformerFromDataSheet_Executed, CalcTransformerFromDataSheet_CanExecuted, "Пересчитать Т схему замещения");
            SaveTransToJsonCommand = new RelayCommand(SaveTransToJson_Executed, null, "Сохранить в файл");
            OpenTransFromJsonCommand = new RelayCommand(OpenTransFromJson_Executed, null, "Загрузить из файла");
            TransformerType = TransformerTypes.First();
        }
        #endregion
    }
}
