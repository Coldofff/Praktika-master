using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using DataVis = System.Windows.Forms.DataVisualization;

namespace Praktika
{
    /// <summary>
    /// Логика взаимодействия для AnalysisWindow.xaml
    /// </summary>
    public partial class AnalysisWindow : Window
    {
        public AnalysisWindow(users currentUser)
        {
            InitializeComponent();
            chartAnalysisAdded.ChartAreas.Add(new ChartArea("Main"));
            chartAnalysisChanged.ChartAreas.Add(new ChartArea("Main"));
            chartAnalysisDeleted.ChartAreas.Add(new ChartArea("Main"));
            var seriesAdded = new Series("analysisAdded")
            {
                IsValueShownAsLabel = true
            };
            var seriesChanged= new Series("analysisChanged")
            {
                IsValueShownAsLabel = true
            };
            var seriesDeleted = new Series("analysisDeleted")
            {
                IsValueShownAsLabel = true
            };
            chartAnalysisAdded.Series.Add(seriesAdded);
            chartAnalysisDeleted.Series.Add(seriesDeleted);
            chartAnalysisChanged.Series.Add(seriesChanged);
            var analyzes = Instances.db.analyzes.Where(p => p.FK_user_id == currentUser.PK_users_id).ToList();
            for(int i = 0; i < analyzes.Count; i++)
            {
                seriesAdded.Points.AddXY(analyzes[i].date, analyzes[i].added_count);
                seriesDeleted.Points.AddXY(analyzes[i].date, analyzes[i].deleted_count);
                seriesChanged.Points.AddXY(analyzes[i].date, analyzes[i].changed_count);
            }
        }
    }
}
