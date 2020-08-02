using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using RC_FE_Design___Analysis_and_synthesis.ProjectTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_FE_Design___Analysis_and_synthesis.MathModel
{
    /// <summary>
    /// График
    /// </summary>
    class PRPlot : IProjectTreeItem
    {
        // Метод для инициализации графика
        public static void InitializatePhaseResponsePlot(List<(double, double)> points, PRPlot plot) 
        {
            if (plot != null)
            {
                var series = new LineSeries() { InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline };

                foreach (var point in points)
                {
                    series.Points.Add(new DataPoint(point.Item1, point.Item2));
                }

                plot.Model = new PlotModel() { Title = "ФЧХ" }; 

                plot.Model.Axes.Add(new LinearAxis() { 
                    Title = "φ", 
                    MajorGridlineStyle = LineStyle.Automatic, 
                    MajorGridlineColor = OxyColors.LightGray, 
                    Position = AxisPosition.Left, 
                    Unit = "град", 
                    AxisTitleDistance = 10 
                });

                plot.Model.Axes.Add(new LogarithmicAxis() { 
                    Title = "ωRC", 
                    MajorGridlineStyle = LineStyle.Automatic,
                    MajorGridlineColor = OxyColors.LightGray, 
                    Position = AxisPosition.Bottom, 
                    Base = 10 
                });

                plot.Model.Series.Add(series);
            }
        }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; } = "График";

        /// <summary>
        /// Модель графика
        /// </summary>
        public PlotModel Model { get; set; }
    }
}
