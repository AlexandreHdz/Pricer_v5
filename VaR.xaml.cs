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
    /// Logique d'interaction pour VaR.xaml
    /// </summary>
    public partial class VaR : Window
    {
        public VaR()
        {
            InitializeComponent();
        }
        YahooData DataFromYahoo = new YahooData();
        String Symbol = "";
        String Hor = "";
        String Alp = "";

        string[] ListCol = new string[] { "Date", "Open", "High", "Low", "Close", "AdjClos", "Volume" };
        private ModelView.ModelView viewModel;

        private async void Calcul_Click(object sender, RoutedEventArgs e)
        {
            Symbol = Ticker.Text;
            Hor = Horizon.Text;
            Alp = Alpha.Text;
            Displaydata();
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
            Symbol = Ticker.Text;
            Hor = Horizon.Text;
            Alp = Alpha.Text;
            foreach (string s in ListCol)
            {
                DataColumn x = new DataColumn();
                x.ColumnName = s;
                x.DataType = System.Type.GetType("System.String");
                Histo.Columns.Add(x);
            }
            try
            {
                var test = await DataFromYahoo.GetVaRData(Symbol, Histo);
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
            var awaiter = await DataFromYahoo.GetVaRData(Symbol, Histo);
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
                }
                //Ajout des éléments dans la table 
                YahooDataList.ItemsSource = list;
            }
            Utilitaire ut = new Utilitaire();
            double VarHis = Math.Round(ut.VaRHisto(Histo, double.Parse(Alp, CultureInfo.InvariantCulture) / 100.0, double.Parse(Hor, CultureInfo.InvariantCulture)),6);
            double VarMC = Math.Round(ut.VaRMC(Histo, double.Parse(Alp, CultureInfo.InvariantCulture) / 100.0, double.Parse(Hor, CultureInfo.InvariantCulture)),6);
            double VarPara = Math.Round(ut.VaRPara(Histo, double.Parse(Alp, CultureInfo.InvariantCulture) / 100.0, double.Parse(Hor, CultureInfo.InvariantCulture)),6);

            VaRHistInp.Text = VarHis.ToString();
            VaRMCInp.Text = VarMC.ToString();
            VarParaInp.Text = VarPara.ToString();

            Histo.Clear();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Calcul_Click(sender, e);
            }
        }

        public class Measurement
        {
            public double Value { get; set; }
            public DateTime DateTime { get; set; }
        }
    }
}
