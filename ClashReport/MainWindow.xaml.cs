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
using System.Windows.Navigation;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ClashReport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private exchange _report;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddClashClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "Navisworks report (*.xml)|*.xml";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string filePath in dialog.FileNames)
                {
                    _report = ReadReport(filePath);
                    FillClashTestList(filePath);
                }
            }
        }

        private void FillClashTestList(string reportPath)
        {
            foreach (object item in _report.Items)
            {
                if (item.GetType() == typeof(batchtest))
                {
                    batchtest batchTest = item as batchtest;

                    foreach (clashtest clashTest in batchTest.clashtests)
                    {
                        clashTest.ReportDirectory = Path.GetDirectoryName(reportPath);
                        clashtestList.Items.Add(clashTest);
                    }
                }
            }
        }

        private exchange ReadReport(string filePath)
        {
            exchange reportRoot = new exchange();

            using (Stream objStream = new FileStream(filePath, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(exchange));

                StreamReader sr = new StreamReader(objStream);

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;


                using (XmlReader reader = XmlReader.Create(sr, settings))
                {
                    reportRoot = (exchange)serializer.Deserialize(reader);
                    reportRoot.filename = filePath;
                    reportRoot.filepath = Path.GetDirectoryName(filePath);
                }
            }

            return reportRoot;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xlsx";
            saveDialog.AddExtension = true;

            saveDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";

            // If the file name is not an empty string open it for saving.
            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (saveDialog.FileName != "")
                {
                    foreach (object item in clashtestList.Items)
                    {
                        if (item.GetType() == typeof(clashtest))
                        {
                            clashtest clashTest = item as clashtest;
                            clashTest.WriteToExcel(saveDialog.FileName);
                        }
                    }
                }
            }
            
        }
    }
}
