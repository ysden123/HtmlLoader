using HtmlAgilityPack;
using System.Text;
using System.Windows;

namespace HtmlLoader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HtmlWeb htmlWeb = new();
        HtmlDocument? htmlDocument;
        public MainWindow()
        {
            InitializeComponent();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            if (fvi != null && fvi.FileVersion != null)
            {
                string version = fvi.FileVersion;
                Title = $"{Title} {version}";
            }
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            string theUrl = URLTextBox.Text;
            await Task.Run(async () =>
            {
                //var htmlWeb = new HtmlWeb();
                try
                {
                    htmlDocument = await htmlWeb.LoadFromWebAsync(theUrl);
                    if (htmlDocument != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            HtmlTextBox.Text = htmlDocument.DocumentNode.OuterHtml;
                        });
                    }
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Error loading HTML: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            });
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (htmlDocument != null && !string.IsNullOrEmpty(FileNameTextBox.Text))
            {
                string fileName = FileNameTextBox.Text;
                try
                {
                    htmlDocument.Save(fileName, Encoding.UTF8);
                    MessageBox.Show($"HTML saved to {fileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving HTML: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No HTML document to save. Please load a URL first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void URLTextBoxChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            LoadButton.IsEnabled = !string.IsNullOrWhiteSpace(URLTextBox.Text);
        }

        private void FileNameTextBoxChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            SaveButton.IsEnabled = !string.IsNullOrWhiteSpace(FileNameTextBox.Text);
        }
    }
}