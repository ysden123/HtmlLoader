using HtmlAgilityPack;
using System.IO;
using System.Text;
using System.Text.Json;
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

            try
            {
                LastValues? lastValues = JsonSerializer.Deserialize<LastValues>(File.ReadAllText("LastValues.json"));
                if (lastValues != null)
                {
                    URLTextBox.Text = lastValues.Url;
                    FileNameTextBox.Text = lastValues.Filename;
                }
            }
            catch (Exception)
            {
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            string theUrl = URLTextBox.Text;

            try
            {
                htmlDocument = htmlWeb.Load(theUrl);
                if (htmlDocument != null)
                {
                    HtmlTextBox.Text = htmlDocument.DocumentNode.OuterHtml;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading HTML: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void OnMainWindowClosed(object sender, EventArgs e)
        {
            LastValues lastValues = new()
            {
                Url = URLTextBox.Text,
                Filename = FileNameTextBox.Text
            };

            try
            {
                File.WriteAllText("LastValues.json", JsonSerializer.Serialize(lastValues));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving last values: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Close();
        }
    }
}