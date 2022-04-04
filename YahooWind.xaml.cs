using Dll_Calcul;
using System;
using System.Data;
using System.Windows;
using YahooFinanceApi;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
namespace Pricer_v5
{
    /// <summary>
    /// Logique d'interaction pour Yahoo.xaml
    /// </summary>
    public partial class YahooWind : Window
    {
        public YahooWind()
        {
            InitializeComponent();
            GraphPlot.IsEnabled = false;
            HistoView.IsEnabled = false;
        }
        YahooData DataFromYahoo = new YahooData();
        String Symbol = "";
        String StartDate = "";
        String EndDate = "";

        string[] ListCol = new string[] { "Date", "Open", "High", "Low", "Close", "AdjClos", "Volume" };
        private ModelView.ModelView viewModel;

        private void BtnGetData_Click(object sender, RoutedEventArgs e)
        {
            Symbol = Ticker.Text;
            StartDate = DD.Text;
            EndDate = DF.Text;
            string err = "";
            Utilitaire Test = new Utilitaire();
            string IsDateED = Test.CheckDate(EndDate);
            string IsDateDD = Test.CheckDate(StartDate);
            if (Symbol == "")
            {
                string caption = "Erreur détécté";
                MessageBoxButtons Ok = MessageBoxButtons.OK;
                DialogResult result;
                err = "La valeur pour 'Ticker' n'est pas correct. \n";
                result = System.Windows.Forms.MessageBox.Show(err, caption, Ok);
            }
            else if (IsDateDD != "")
            {
                string caption = "Erreur détécté";
                MessageBoxButtons Ok = MessageBoxButtons.OK;
                DialogResult result;
                err = "La valeur pour 'Date début' n'est pas correct. \n";
                result = System.Windows.Forms.MessageBox.Show(err + IsDateDD, caption, Ok);
            }
            else if (IsDateED != "")
            {
                string caption = "Erreur détécté";
                err = "La valeur pour 'Date de fin' n'est pas correct. \n";
                MessageBoxButtons Ok = MessageBoxButtons.OK;
                DialogResult result;
                result = System.Windows.Forms.MessageBox.Show(err + IsDateED, caption, Ok);
            }
            else
            {
                Displaydata();
            }
        }

        public class RowData
        {
            public string Da { get; set; }
            public string Open { get; set; }
            public string High { get; set; }
            public string Low { get; set; }
            public string Close { get; set; }
            public string AdjClos { get; set; }
            public string Vol { get; set; }
        }

        private async void Displaydata()
        {
            DataTable Histo = new DataTable();
            Histo.Clear();
            YahooDataList.ItemsSource = null;
            foreach (string s in ListCol)
            {
                DataColumn x = new DataColumn();
                x.ColumnName = s;
                x.DataType = System.Type.GetType("System.String");
                Histo.Columns.Add(x);
            }
            try
            {
                var test = await DataFromYahoo.ReceiveData(Symbol, StartDate, EndDate, Histo);
            }
            catch (Exception ex)
            {
                string err = "Aucune donée existente pour ce Ticker";
                string caption = "Erreur détécté";
                MessageBoxButtons Ok = MessageBoxButtons.OK;
                DialogResult result;
                result = System.Windows.Forms.MessageBox.Show(err, caption, Ok);
                return;
            }
            Histo.Clear();
            var awaiter = await DataFromYahoo.ReceiveData(Symbol, StartDate, EndDate, Histo);
            var points = new List<Measurement>();
            if (awaiter == 1)
            {

                List<RowData> list = new List<RowData>();
                for (int j = Histo.Rows.Count - 1; j >= 0; j--)
                {
                    DataRow dr = Histo.Rows[j];
                    list.Add(new RowData()
                    {
                        Da = dr["Date"].ToString(),
                        Open = dr["Open"].ToString(),
                        High = dr["High"].ToString(),
                        Low = dr["Low"].ToString(),
                        Close = dr["Close"].ToString(),
                        AdjClos = dr["AdjClos"].ToString(),
                        Vol = dr["Volume"].ToString()
                    });

                    //Création de la courbe
                    points.Add(new Measurement() { DateTime = Convert.ToDateTime(dr["Date"].ToString()), Value = Convert.ToDouble(dr["Close"].ToString()) });
                }
                //Ajout des éléments dans la table 
                YahooDataList.ItemsSource = list;
            }
            points.Sort((m1, m2) => m1.DateTime.CompareTo(m2.DateTime));
            NbData.Text = Histo.Rows.Count.ToString();

            var lines = new LineSeries
            {
                StrokeThickness = 2,
                MarkerSize = 3,
                CanTrackerInterpolatePoints = false,
            };
            //Graph
            points.ForEach(d => lines.Points.Add(new DataPoint(DateTimeAxis.ToDouble(d.DateTime), d.Value)));
            viewModel = new ModelView.ModelView();
            viewModel.SetUpYahoo(Convert.ToDateTime(DD.Text), Convert.ToDateTime(DF.Text));
            viewModel.AddLines(lines);
            DataContext = viewModel;
            GraphPlot.IsEnabled = true;
            HistoView.IsEnabled = true;
            Histo.Clear();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                BtnGetData_Click(sender, e);
            }
        }

        private void HistoView_Click(object sender, RoutedEventArgs e)
        {
            if (YahooDataList.Visibility == Visibility.Visible)
            {
                return;
            }
            else
            {
                PlotHisto.Visibility = Visibility.Hidden;
                YahooDataList.Visibility = Visibility.Visible;
                return;
            }
        }

        private void GraphPlot_Click(object sender, RoutedEventArgs e)
        {
            if (PlotHisto.Visibility == Visibility.Visible)
            {
                return;
            }
            else
            {
                YahooDataList.Visibility = Visibility.Hidden;
                PlotHisto.Visibility = Visibility.Visible;
                return;
            }
        }
        public class Measurement
        {
            public double Value { get; set; }
            public DateTime DateTime { get; set; }
        }
    }
}
