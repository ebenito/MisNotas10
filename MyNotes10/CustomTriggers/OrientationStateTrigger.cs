using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.Xaml;

namespace MyNotes10.CustomTriggers
{
    /// <summary>
    /// Trigger for switching when the screen orientation changes
    /// </summary>
	public class OrientationStateTrigger : StateTriggerBase
    {
        private static string _orientacion;

        public OrientationStateTrigger()
        {
            _orientacion = DisplayInformation.GetForCurrentView().CurrentOrientation.ToString();
        }

        public string Orientacion
        {
            get { return (string)GetValue(OrientacionProperty); }
            set { SetValue(OrientacionProperty, value); }
        }

        public static readonly DependencyProperty OrientacionProperty =
        DependencyProperty.Register("Orientacion", typeof(string), typeof(OrientationStateTrigger),
        new PropertyMetadata(true, OnOrientacionPropertyChanged));

        private static void OnOrientacionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (OrientationStateTrigger)d;
            var orientacion = (string)e.NewValue;

            if (_orientacion.Equals("Landscape"))
                obj.SetActive(orientacion == "Landscape");
            else
                obj.SetActive(orientacion == "Portrait");
        }
    }
}
