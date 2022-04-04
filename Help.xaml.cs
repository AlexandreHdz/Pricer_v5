using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace Pricer_v5
{
    /// <summary>
    /// Logique d'interaction pour Help.xaml
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            try
            {
                InitializeComponent();
                string fileName = Environment.CurrentDirectory;
                string[] pathDir = fileName.Split("\\bin");
                string path = pathDir[0] + @"\Doc\Pricer_Doc_v2.xps";
                XpsDocument doc = new XpsDocument(path, System.IO.FileAccess.Read);
                DocView.Document = doc.GetFixedDocumentSequence();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

    }
}
