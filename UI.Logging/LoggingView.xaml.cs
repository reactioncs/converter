using System.Windows.Controls;

namespace UI.Logging
{
    /// <summary>
    /// Interaction logic for LoggingView.xaml
    /// </summary>
    public partial class LoggingView : UserControl
    {
        public LoggingView()
        {
            InitializeComponent();
            DataContext = new LoggingViewModel();
        }
    }
}
