using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace TransformExtChar.Infrastructure
{
    public abstract class OnPropertyChangedClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            OnPropertyChangedAction.Invoke(PropertyName);
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }
        public OnPropertyChangedClass()
        {
            OnPropertyChangedAction = DefaultPropertyChanged;
        }
        protected void RegisterCohesionedProperties(params string[] propertyNames)
        {
            if (propertyNames.Length < 2)
                throw new ArgumentException("связываться должны несколько свойств");

            cohesionedProperties ??= new Dictionary<string, List<string>>();

            if(OnPropertyChangedAction != CohesionedPropertyChanged)
                OnPropertyChangedAction = CohesionedPropertyChanged;

            var thisObjectPropertyNames = GetType().GetProperties().Select(p => p.Name);

            foreach (var prop in propertyNames)
            {
                if (!thisObjectPropertyNames.Contains(prop))
                    throw new ArgumentException("передано имя свойства не этого типа");

                if(!cohesionedProperties.ContainsKey(prop))
                    cohesionedProperties[prop] = new List<string>();
                
                cohesionedProperties[prop].AddRange(propertyNames);
            }
        }
        private Action<string> OnPropertyChangedAction;
        public void DefaultPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void DefaultPropertyChanged(IEnumerable<string> propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                DefaultPropertyChanged(propertyName);
            }
        }
        private void CohesionedPropertyChanged(string propertyName)
        {
            if (cohesionedProperties.TryGetValue(propertyName, out var properties))
                foreach (var property in properties)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            else
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private Dictionary<string, List<string>> cohesionedProperties;
    }
}
