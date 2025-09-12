using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace autoupdate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string currentVersion = "1.0.0";
        private readonly string versionFileUrl = "https://github.com/BLXTR27/myapp/blob/main/version.json";
        public MainWindow()
        {
            InitializeComponent();
            VersionText.Text = $"Current Version: {currentVersion}";
        }
        private async void CheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(versionFileUrl);

                var data = JsonSerializer.Deserialize<UpdateInfo>(json);

                if (IsNewerVersion(data.LatestVersion, currentVersion))
                {
                    var result = MessageBox.Show(
                        $"A new version ({data.LatestVersion}) is available. Do you want to update?",
                        "Update Available",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Download and run installer
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = data.DownloadUrl,
                            UseShellExecute = true
                        });
                        Application.Current.Shutdown(); // Close current app
                    }
                }
                else
                {
                    MessageBox.Show("You are already using the latest version.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking updates: {ex.Message}");
            }
        }

        private bool IsNewerVersion(string latest, string current)
        {
            Version vLatest = new Version(latest);
            Version vCurrent = new Version(current);
            return vLatest > vCurrent;
        }
    }

    public class UpdateInfo
    {
        public string LatestVersion { get; set; }
        public string DownloadUrl { get; set; }
    }
}