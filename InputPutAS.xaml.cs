using Dll_Calcul;
using System;
using System.Globalization;
using System.Windows;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Pricer_v5
{
    /// <summary>
    /// Logique d'interaction pour Input.xaml
    /// </summary>
    public partial class InputPutAS : Window
    {
        public InputPutAS()
        {
            InitializeComponent();
            DateTime Date1 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
            if (Date1.Month < 10)
            {
                if (Date1.Day < 10)
                {
                    AsofInp.Text = "0" + Date1.Day.ToString() + "/0" + Date1.Month.ToString() + "/" + Date1.Year.ToString();
                }
                else
                {
                    AsofInp.Text = Date1.Day.ToString() + "/0" + Date1.Month.ToString() + "/" + Date1.Year.ToString();
                }
            }
            else
            {
                if (Date1.Day < 10)
                {
                    AsofInp.Text = "0" + Date1.Day.ToString() + "/" + Date1.Month.ToString() + "/" + Date1.Year.ToString();
                }
                else
                {
                    AsofInp.Text = Date1.Day.ToString() + "/" + Date1.Month.ToString() + "/" + Date1.Year.ToString();
                }
            }
        }

        string Spot = "";
        string Strike = "";
        string Taux = "";
        string Vol = "";
        string Mat = "";
        string DateAf = "";
        Utilitaire cal = new Utilitaire();
        private ModelView.ModelView viewModel;
        private void Price_Click(object sender, RoutedEventArgs e)
        {
            string err = "";
            Spot = SpotInp.Text;
            Strike = StrikeInp.Text;
            Taux = TauxInp.Text;
            Vol = VolInp.Text;
            Mat = MatInp.Text;
            DateAf = AsofInp.Text;
            if (cal.CheckInput("Spot", Spot) != "")
            {
                string caption = "Erreur détectée";
                MessageBoxButtons Ok = MessageBoxButtons.OK;
                DialogResult result;
                err = "La valeur pour 'Spot' n'est pas correct. \n";
                result = System.Windows.Forms.MessageBox.Show(err + cal.CheckInput("Spot", Spot), caption, Ok);
            }
            else if (cal.CheckInput("Strike", Strike) != "")
            {
                string caption = "Erreur détectée";
                MessageBoxButtons Ok = MessageBoxButtons.OK;
                DialogResult result;
                err = "La valeur pour 'Strike' n'est pas correct. \n";
                result = System.Windows.Forms.MessageBox.Show(err + cal.CheckInput("Strike", Strike), caption, Ok);
            }
            else if (cal.CheckInput("Taux", Taux) != "")
            {
                string caption = "Erreur détectée";
                MessageBoxButtons Ok = MessageBoxButtons.OK;
                DialogResult result;
                err = "La valeur pour 'taux' n'est pas correct. \n";
                result = System.Windows.Forms.MessageBox.Show(err + cal.CheckInput("Taux", Taux), caption, Ok);
            }
            else if (cal.CheckInput("Vol", Vol) != "")
            {
                string caption = "Erreur détectée";
                MessageBoxButtons Ok = MessageBoxButtons.OK;
                DialogResult result;
                err = "La valeur pour 'Volatility' n'est pas correct. \n";
                result = System.Windows.Forms.MessageBox.Show(err + cal.CheckInput("Vol", Vol), caption, Ok);
            }
            else if (cal.CheckInput("Mat", Mat) != "")
            {
                string caption = "Erreur détectée";
                MessageBoxButtons Ok = MessageBoxButtons.OK;
                DialogResult result;
                err = "La valeur pour 'Maturité' n'est pas correct. \n";
                result = System.Windows.Forms.MessageBox.Show(err + cal.CheckInput("Mat", Mat), caption, Ok);
            }
            else if (cal.CheckInput("Asof", DateAf) != "")
            {
                string caption = "Erreur détectée";
                MessageBoxButtons Ok = MessageBoxButtons.OK;
                DialogResult result;
                err = "La valeur pour 'Date Pricing' n'est pas correct. \n";
                result = System.Windows.Forms.MessageBox.Show(err + cal.CheckInput("Asof", DateAf), caption, Ok);
            }
            else
            {
                DisplayDataCallEU();
            }

        }

        private async void DisplayDataCallEU()
        {
            PriceInp.Visibility = Visibility.Hidden;
            Spot = SpotInp.Text;
            Strike = StrikeInp.Text;
            Taux = TauxInp.Text;
            Vol = VolInp.Text;
            Mat = MatInp.Text;
            DateAf = AsofInp.Text;
            YahooData yh = new YahooData();
            DateTime DateAsof = yh.ConvertDate(DateAf);
            DateTime DateT = yh.ConvertDate(Mat);
            int nbDays = yh.NbWorkingDays(DateAsof, DateT);
            int nbJoursTot = yh.TotalDays(DateAsof, DateT);

            double S = double.Parse(Spot, CultureInfo.InvariantCulture);
            double K = double.Parse(Strike, CultureInfo.InvariantCulture);
            double r = double.Parse(Taux, CultureInfo.InvariantCulture) / 100.0;
            double v = double.Parse(Vol, CultureInfo.InvariantCulture) / 100.0;
            //double T = double.Parse(Mat, CultureInfo.InvariantCulture);
            double T = (double)nbDays;
            double Tfa = T / (double)nbJoursTot;

            decimal Prime = 0.0m;

            PbStatus.Visibility = Visibility.Visible;
            PbCal.Visibility = Visibility.Visible;
            PbStatus.Value = 0;
            var points = new List<Simu>();
            if (Euler.IsChecked == true)
            {
                await Task.Run(() =>
                {
                    for (int i = 0; i < 100000; i++)
                    {
                        if (i % 1000 == 0)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                PbStatus.Value = (int)i / 1000;
                            });
                            decimal[] traji = cal.Euler(S, r, v, T, nbJoursTot);
                            for (int s = 0; s < traji.Length - 1; s++)
                            {
                                points.Add(new Simu() { id = i, pas = s, Value = traji[s] });
                            }
                            decimal somme1 = (decimal)Math.Max(K - (double)cal.SommeTraj(traji) / traji.Length, 0.0);
                            decimal somme2 = (decimal)Math.Max(K - Math.Exp((double)cal.LogSommeTraj(traji) / traji.Length), 0.0);
                            Prime = Prime + somme1 - somme2;

                        }
                        else
                        {
                            decimal[] traj = cal.Euler(S, r, v, T, nbJoursTot);

                            decimal somme1 = (decimal)Math.Max(K - (double)cal.SommeTraj(traj) / traj.Length, 0.0);
                            decimal somme2 = (decimal)Math.Max(K - Math.Exp((double)cal.LogSommeTraj(traj) / traj.Length), 0.0);
                            Prime = Prime + somme1 - somme2;
                        }

                    }
                    var AllData = points.GroupBy(m => m.id).ToList();
                    viewModel = new ModelView.ModelView();
                    viewModel.SetBackground(190, 190, 190);
                    foreach (var data in AllData)
                    {
                        var lines = new LineSeries
                        {
                            StrokeThickness = 2,
                            MarkerSize = 3,
                            CanTrackerInterpolatePoints = false,
                        };
                        data.ToList().ForEach(d => lines.Points.Add(new DataPoint(d.pas, Convert.ToDouble(d.Value))));
                        viewModel.AddLines(lines);
                    }
                });
            }
            else if (MBG.IsChecked == true)
            {
                await Task.Run(() =>
                {
                    for (int i = 0; i < 100000; i++)
                    {
                        if (i % 1000 == 0)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                PbStatus.Value = (int)i / 1000;
                            });
                            decimal[] traji = cal.MBG(S, r, v, T, nbJoursTot);
                            for (int s = 0; s < traji.Length - 1; s++)
                            {
                                points.Add(new Simu() { id = i, pas = s, Value = traji[s] });
                            }
                            decimal somme1 = (decimal)Math.Max(K - (double)cal.SommeTraj(traji) / traji.Length, 0.0);
                            decimal somme2 = (decimal)Math.Max(K - Math.Exp((double)cal.LogSommeTraj(traji) / traji.Length), 0.0);
                            Prime = Prime + somme1 - somme2;

                        }
                        else
                        {
                            decimal[] traj = cal.MBG(S, r, v, T, nbJoursTot);

                            decimal somme1 = (decimal)Math.Max(K - (double)cal.SommeTraj(traj) / traj.Length, 0.0);
                            decimal somme2 = (decimal)Math.Max(K - Math.Exp((double)cal.LogSommeTraj(traj) / traj.Length), 0.0);
                            Prime = Prime + somme1 - somme2;
                        }

                    }
                    var AllData = points.GroupBy(m => m.id).ToList();
                    viewModel = new ModelView.ModelView();
                    viewModel.SetBackground(190, 190, 190);
                    foreach (var data in AllData)
                    {
                        var lines = new LineSeries
                        {
                            StrokeThickness = 2,
                            MarkerSize = 3,
                            CanTrackerInterpolatePoints = false,
                        };
                        data.ToList().ForEach(d => lines.Points.Add(new DataPoint(d.pas, Convert.ToDouble(d.Value))));
                        viewModel.AddLines(lines);
                    }
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    for (int i = 0; i < 100000; i++)
                    {
                        if (i % 1000 == 0)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                PbStatus.Value = (int)i / 1000;
                            });
                            decimal[] traji = cal.Milstein2(S, r, v, T, nbJoursTot);
                            for (int s = 0; s < traji.Length - 1; s++)
                            {
                                points.Add(new Simu() { id = i, pas = s, Value = traji[s] });
                            }
                            decimal somme1 = (decimal)Math.Max(K - (double)cal.SommeTraj(traji) / traji.Length, 0.0);
                            decimal somme2 = (decimal)Math.Max(K - Math.Exp((double)cal.LogSommeTraj(traji) / traji.Length), 0.0);
                            Prime = Prime + somme1 - somme2;

                        }
                        else
                        {
                            decimal[] traj = cal.Milstein2(S, r, v, T, nbJoursTot);

                            decimal somme1 = (decimal)Math.Max(K - (double)cal.SommeTraj(traj) / traj.Length, 0.0);
                            decimal somme2 = (decimal)Math.Max(K - Math.Exp((double)cal.LogSommeTraj(traj) / traj.Length), 0.0);
                            Prime = Prime + somme1 - somme2;
                        }

                    }
                    var AllData = points.GroupBy(m => m.id).ToList();
                    viewModel = new ModelView.ModelView();
                    viewModel.SetBackground(190, 190, 190);
                    foreach (var data in AllData)
                    {
                        var lines = new LineSeries
                        {
                            StrokeThickness = 2,
                            MarkerSize = 3,
                            CanTrackerInterpolatePoints = false,
                        };
                        data.ToList().ForEach(d => lines.Points.Add(new DataPoint(d.pas, Convert.ToDouble(d.Value))));
                        viewModel.AddLines(lines);
                    }
                });
            }
            PlotSimu.DataContext = viewModel;

            PbStatus.Visibility = Visibility.Collapsed;
            PbCal.Visibility = Visibility.Collapsed;
            PriceInp.Visibility = Visibility.Visible;
            Prime = Prime / 100000m + cal.EzPut(S, r, v, Tfa, K, 1.0);
            Prime = Math.Round(Prime, 5);
            decimal PrimeFA = Math.Round(cal.PutAS(S, r, v, Tfa, K), 5);
            FAInp.Text = PrimeFA.ToString();
            MCInp.Text = Prime.ToString();
            MCInp.Visibility = Visibility.Visible;
            MC.Visibility = Visibility.Visible;
            DeltaInp.Text = Math.Round(cal.DeltaASFDM(S, r, v, Tfa, K, "Put"), 5).ToString();
            GammaInp.Text = Math.Round(cal.GammaASFDM(S, r, v, Tfa, K, "Put"), 5).ToString();
            VegaInp.Text = Math.Round(cal.VegaASFDM(S, r, v, Tfa, K, "Put"), 5).ToString();
            ThetaInp.Text = Math.Round(cal.ThetaASFDM(S, r, v, Tfa, K, "Put"), 5).ToString();
            RhoInp.Text = Math.Round(cal.RhoASFDM(S, r, v, Tfa, K, "Put"), 5).ToString();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Price_Click(sender, e);
            }
        }
        public class Simu
        {
            public decimal Value { get; set; }
            public int id { get; set; }
            public int pas { get; set; }
        }
    }
}

