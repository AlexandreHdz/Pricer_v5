using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Legends;


using System.ComponentModel;


namespace Pricer_v5.ModelView
{
    public class ModelView : INotifyPropertyChanged
    {
        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; OnPropertyChanged("PlotModel"); }
        }

        public ModelView()
        {
            PlotModel = new PlotModel();
        }
        public void AddLines(LineSeries values)
        {
            PlotModel.Series.Add(values);
        }
        public void SetUpYahoo(DateTime DD,DateTime DF)
        {
            PlotModel.Title = "Historique";
            var minValue = DateTimeAxis.ToDouble(DD);
            var maxValue = DateTimeAxis.ToDouble(DF);
            PlotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom,Minimum = minValue, Maximum = maxValue, StringFormat = "MM/yy" ,IntervalLength=20});
        }

        public void SetTitleSimu()
        {
            PlotModel.Title = "Simulation";
        }
        public void SetBackground(byte r, byte g, byte b)
        {
            PlotModel.Background = OxyColor.FromRgb(r, g, b);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PayoffView : INotifyPropertyChanged
    {
        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; OnPropertyChanged("PlotModel"); }
        }

        public PayoffView()
        {
            PlotModel = new PlotModel();
        }
        public void AddLines(LineSeries values)
        {
            PlotModel.Series.Add(values);
        }
        public void SetUpAxis(decimal Prime, double K)
        {
            PlotModel.Title = "Payoff";
            var minY = - (double)Prime - 10.0;
            var maxY = (double)Prime + 10.0;
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left,Minimum = minY, Maximum = maxY });
            PlotModel.Legends.Add(new Legend() { LegendTitle = "Legend", LegendPosition = LegendPosition.TopLeft });
        }
        public void SetBackground(byte r, byte g, byte b)
        {
            PlotModel.Background = OxyColor.FromRgb(r, g, b);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
