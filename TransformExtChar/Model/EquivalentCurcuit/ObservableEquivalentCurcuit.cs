using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TransformExtChar.Infrastructure;

namespace TransformExtChar.Model
{
    public class ObservableEquivalentCurcuit : OnPropertyChangedClass, IEquivalentCurcuit
    {
        #region Свойства
        public double K 
        {
            get => _equivalentCurcuit.K; 
            set
            {
                _equivalentCurcuit.K = value;
                OnPropertyChanged();
            }
        }
        public double R1
        {
            get => _equivalentCurcuit.R1;
            set
            {
                _equivalentCurcuit.R1 = value;
                OnPropertyChanged();
            }
        }
        public double R2_Corrected 
        { 
            get => _equivalentCurcuit.R2_Corrected;
            set
            { 
                _equivalentCurcuit.R2_Corrected = value;
                OnPropertyChanged();
            }
        }
        public double Rm 
        { 
            get => _equivalentCurcuit.Rm;
            set 
            { 
                _equivalentCurcuit.Rm = value;
                OnPropertyChanged();
            } 
        }
        public double X1 
        { 
            get => _equivalentCurcuit.X1;
            set 
            {
                _equivalentCurcuit.X1 = value;
                OnPropertyChanged();
            } 
        }
        public double X2_Corrected 
        { 
            get => _equivalentCurcuit.X2_Corrected;
            set 
            {
                _equivalentCurcuit.X2_Corrected = value;
                OnPropertyChanged();
            } 
        }
        public double Xm 
        { 
            get => _equivalentCurcuit.Xm;
            set 
            {
                _equivalentCurcuit.Xm = value;
                OnPropertyChanged();
            } 
        }
        public Complex Z1 
        { 
            get => _equivalentCurcuit.Z1;
            set 
            {
                _equivalentCurcuit.Z1 = value;
                OnPropertyChanged();
            }
        }
        public Complex Z2_Сorrected 
        { 
            get => _equivalentCurcuit.Z2_Сorrected;
            set 
            { 
                _equivalentCurcuit.Z2_Сorrected = value;
                OnPropertyChanged();
            } 
        }
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

        public ObservableEquivalentCurcuit() : this(new EquivalentCurcuit()) { }
        public ObservableEquivalentCurcuit(EquivalentCurcuit equivalentCurcuit)
        {
            _equivalentCurcuit = equivalentCurcuit;
            RegisterCohesionedProperties(nameof(Z1), nameof(R1), nameof(X1));
            RegisterCohesionedProperties(nameof(Z2_Сorrected), nameof(R2_Corrected), nameof(X2_Corrected));
            RegisterCohesionedProperties(nameof(Zm), nameof(Rm), nameof(Xm));
        } 

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
    }
}