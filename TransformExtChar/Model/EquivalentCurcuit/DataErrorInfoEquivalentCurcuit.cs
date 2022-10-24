using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TransformExtChar.Infrastructure.DataErrorVerify;

namespace TransformExtChar.Model
{
    public class DataErrorInfoEquivalentCurcuit : DataErrorInfoClass, IEquivalentCurcuit
    {
        #region свойства из IEquivalentCurcuit
        [JsonProperty(Required = Required.Always)]
        public double K
        {
            get => _equivalentCurcuit.K;
            set
            {
                _equivalentCurcuit.K = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public double R1
        {
            get => _equivalentCurcuit.R1;
            set
            {
                _equivalentCurcuit.R1 = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public double R2_Corrected
        {
            get => _equivalentCurcuit.R2_Corrected;
            set
            {
                _equivalentCurcuit.R2_Corrected = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public double Rm
        {
            get => _equivalentCurcuit.Rm;
            set
            {
                _equivalentCurcuit.Rm = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public double X1
        {
            get => _equivalentCurcuit.X1;
            set
            {
                _equivalentCurcuit.X1 = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public double X2_Corrected
        {
            get => _equivalentCurcuit.X2_Corrected;
            set
            {
                _equivalentCurcuit.X2_Corrected = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public double Xm
        {
            get => _equivalentCurcuit.Xm;
            set
            {
                _equivalentCurcuit.Xm = value;
                OnPropertyChanged();
            }
        }
        [JsonProperty(Required = Required.Always)]
        public Complex Z1
        {
            get => _equivalentCurcuit.Z1;
            set
            {
                _equivalentCurcuit.Z1 = value;
                OnPropertyChanged();
            }
        }
        [JsonProperty(Required = Required.Always)]
        public Complex Z2_Сorrected
        {
            get => _equivalentCurcuit.Z2_Сorrected;
            set
            {
                _equivalentCurcuit.Z2_Сorrected = value;
                OnPropertyChanged();
            }
        }
        [JsonProperty(Required = Required.Always)]
        public Complex Zm
        {
            get => _equivalentCurcuit.Zm;
            set
            {
                _equivalentCurcuit.Zm = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private EquivalentCurcuit _equivalentCurcuit;
        public DataErrorInfoEquivalentCurcuit() : this(new EquivalentCurcuit()) { }
        public DataErrorInfoEquivalentCurcuit(EquivalentCurcuit equivalentCurcuit) 
        {
            _equivalentCurcuit = equivalentCurcuit;

            RegisterCohesionedProperties(nameof(Z1), nameof(R1), nameof(X1));
            RegisterCohesionedProperties(nameof(Z2_Сorrected), nameof(R2_Corrected), nameof(X2_Corrected));
            RegisterCohesionedProperties(nameof(Zm), nameof(Rm), nameof(Xm));

            RegisterRule_PropMustBeAboveOrEqualZero(() => Z1.Real, new string[] { nameof(Z1), nameof(R1) });
            RegisterRule_PropMustBeAboveOrEqualZero(() => Z2_Сorrected.Real, new string[] { nameof(Z2_Сorrected), nameof(R2_Corrected) });
            RegisterRule_PropMustBeAboveOrEqualZero(() => Zm.Real, new string[] { nameof(Zm), nameof(Rm) });
            RegisterRule(() => Zm.Magnitude != 0, "В ветви намагничивания должно быть сопротивление", new string[] { nameof(Zm), nameof(Rm), nameof(Xm) });
            RegisterRule_PropMustBeAboveZero(() => K);

            CheckAllRulesAndSetError();
            var propNames = GetType().GetProperties().Select(p => p.Name);
            DefaultPropertyChanged(propNames);
        }

        #region методы из IEquivalentCurcuit
        public List<VCPointData> GetExternalCharacteristic(double fi2_rad = 0, double I2_correctedStart = 0, double I2_correctedEnd = 0, double U1 = 0, double I2_step = 0.01)
        {
            return _equivalentCurcuit.GetExternalCharacteristic(fi2_rad, I2_correctedStart, I2_correctedEnd, U1, I2_step);
        }

        public Task<List<VCPointData>> GetExternalCharacteristicAsync(double fi2_rad = 0, double I2_correctedStart = 0, double I2_correctedEnd = 0, double U1 = 0, double I2_step = 0.01)
        {
            return _equivalentCurcuit.GetExternalCharacteristicAsync(fi2_rad, I2_correctedStart, I2_correctedEnd, U1, I2_step);
        }

        public List<VCPointData> GetFullExternalCharacteristic(double fi2_rad = 0, double U1 = 0, double I2_step = 0.01)
        {
            return _equivalentCurcuit.GetFullExternalCharacteristic(fi2_rad, U1, I2_step);
        }

        public Task<List<VCPointData>> GetFullExternalCharacteristicAsync(double fi2_rad = 0, double U1 = 0, double I2_step = 0.1)
        {
            return _equivalentCurcuit.GetExternalCharacteristicAsync(fi2_rad, U1, I2_step);
        }
        #endregion
    }
}
