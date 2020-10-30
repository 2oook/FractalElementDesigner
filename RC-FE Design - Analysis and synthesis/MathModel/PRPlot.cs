using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using FractalElementDesigner.ProjectTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using FractalElementDesigner.MVVM;

namespace FractalElementDesigner.MathModel
{
    /// <summary>
    /// График
    /// </summary>
    [Serializable]
    class PRPlot : IProjectTreeItem, INotifyPropertyChanged
    {
        public PRPlot()
        {
            SetLogarithmicAxleCommand = new RelayCommand(SetLogarithmicAxle);
            SetLinearAxleCommand = new RelayCommand(SetLinearAxle);
        }
        
        private ICommand setLogarithmicAxleCommand;
        /// <summary>
        /// Команда для установки логарифмической оси для частот
        /// </summary>
        public ICommand SetLogarithmicAxleCommand
        {
            get { return setLogarithmicAxleCommand; }
            set
            {
                setLogarithmicAxleCommand = value;
                RaisePropertyChanged(nameof(SetLogarithmicAxleCommand));
            }
        }
     
        private ICommand setLinearAxleCommand;
        /// <summary>
        /// Команда для установки линейной оси для частот
        /// </summary>
        public ICommand SetLinearAxleCommand
        {
            get { return setLinearAxleCommand; }
            set
            {
                setLinearAxleCommand = value;
                RaisePropertyChanged(nameof(SetLinearAxleCommand));
            }
        }

        // Метод для установки логарифмической оси для частот
        private void SetLogarithmicAxle(object param) 
        {
            if (Model == null)
            {
                return;
            }

            Model.Axes.Clear();

            Model.ResetAllAxes();

            Model.Axes.Add(new LinearAxis()
            {
                Title = "φ",
                MajorGridlineStyle = LineStyle.Automatic,
                MajorGridlineColor = OxyColors.LightGray,
                Position = AxisPosition.Left,
                Unit = "град",
                AxisTitleDistance = 10,
                IntervalLength = 50
            });

            Model.Axes.Add(new LogarithmicAxis()
            {
                Title = "ωRC",
                MajorGridlineStyle = LineStyle.Automatic,
                MajorGridlineColor = OxyColors.LightGray,
                Position = AxisPosition.Bottom,
                Base = 10,
                IntervalLength = 300,
                UseSuperExponentialFormat = true
            });

            Model.InvalidatePlot(false);
        }

        // Метод для установки линейной оси для частот
        private void SetLinearAxle(object param)
        {
            if (Model == null)
            {
                return;
            }

            Model.Axes.Clear();

            Model.ResetAllAxes();

            Model.Axes.Add(new LinearAxis()
            {
                Title = "φ",
                MajorGridlineStyle = LineStyle.Automatic,
                MajorGridlineColor = OxyColors.LightGray,
                Position = AxisPosition.Left,
                Unit = "град",
                AxisTitleDistance = 10,
                IntervalLength = 50
            });

            Model.Axes.Add(new LinearAxis()
            {
                Title = "ωRC",
                MajorGridlineStyle = LineStyle.Automatic,
                MajorGridlineColor = OxyColors.LightGray,
                Position = AxisPosition.Bottom,
                IntervalLength = 50
            });

            Model.InvalidatePlot(false);
        }

        // Метод для инициализации графика
        public void InitializatePhaseResponsePlot(List<(double, double)> points)
        {
            var series = new LineSeries() { InterpolationAlgorithm = InterpolationAlgorithms.CatmullRomSpline };

            foreach (var point in points)
            {
                series.Points.Add(new DataPoint(point.Item1, point.Item2));
            }

            Model = new PlotModel() { Title = "ФЧХ" };

            Model.Axes.Add(new LinearAxis()
            {
                Title = "φ",
                MajorGridlineStyle = LineStyle.Automatic,
                MajorGridlineColor = OxyColors.LightGray,
                Position = AxisPosition.Left,
                Unit = "град",
                AxisTitleDistance = 10,
                IntervalLength = 50
            });

            Model.Axes.Add(new LogarithmicAxis()
            {
                Title = "ωRC",
                MajorGridlineStyle = LineStyle.Automatic,
                MajorGridlineColor = OxyColors.LightGray,
                Position = AxisPosition.Bottom,
                Base = 10,
                IntervalLength = 300,
                UseSuperExponentialFormat = true
            });

            Model.Series.Add(series);         
        }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; } = "График";

        [NonSerialized]
        private PlotModel model = null;
        /// <summary>
        /// Модель графика
        /// </summary>
        public PlotModel Model 
        { 
            get => model;
            set 
            {
                model = value;
                RaisePropertyChanged(nameof(Model));
            } 
        }

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Метод для поднятия события изменения свойства
        /// </summary>
        /// <param name="propName">Имя свойства</param>
        protected virtual void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
