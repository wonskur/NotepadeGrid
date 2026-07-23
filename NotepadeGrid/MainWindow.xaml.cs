using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core.AnimationMetrics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NotepadeGrid
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>

    public class Note
    {
        private StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        public string Filename { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public Note()
        {
            Filename = "notes" + DateTime.Now.ToBinary().ToString() + ".txt";
        }
        public async void SaveNote()
        {
            StorageFile file = await storageFolder.CreateFileAsync(Filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, Text);
        }
        public async Task LoadNoteAsync(string fullpathtofile)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(fullpathtofile);
            Text = await FileIO.ReadTextAsync(file);
        }
    }
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var DayButton = this.BtnDay;
            var NightButton = this.BtnNight;
            var GridButton = this.BtnGrid;
            var TimeBlock = this.TimeBlock;
            var SaveButton = this.BtnSave;
            var OpenButton = this.BtnOpenFile;
            var NoteTextBox = this.NoteText;
            var NameBlock = this.NameBox;
            var Grid = this.MainGrid;
        }

        private async void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            picker.FileTypeFilter.Add(".txt");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                Note note = new Note();
                await note.LoadNoteAsync(file.Path);
                NoteText.Text = note.Text;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                NameBox.Header = "Введите имя файла";
                return;
            }
            string fileName = NameBox.Text;
            if (!fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".txt";
            }
            string fullPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, fileName);
            System.IO.File.WriteAllText(fullPath, NoteText.Text);
        }

        private void BtnGrid_Click(object sender, RoutedEventArgs e)
        {
            if (GridLinesCanvas.Visibility == Visibility.Collapsed)
            {
                DrawGridLines();
                GridLinesCanvas.Visibility = Visibility.Visible;
                BtnGrid.Content = "Скрыть сетку";
            }
            else
            {
                GridLinesCanvas.Visibility = Visibility.Collapsed;
                BtnGrid.Content = "Показать сетку";
            }
        }


        private void BtnNight_Click(object sender, RoutedEventArgs e)
        {
            if (this.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = ElementTheme.Dark;
                if (GridLinesCanvas.Visibility == Visibility.Visible)
                {
                    DrawGridLines();
                }
            }
        }

        private void BtnDay_Click(object sender, RoutedEventArgs e)
        {
            if (this.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = ElementTheme.Light;
                if (GridLinesCanvas.Visibility == Visibility.Visible)
                {
                    DrawGridLines();
                }
            }
        }
        private void DrawGridLines()
        {
            GridLinesCanvas.Children.Clear();

            var themeBrush = (Brush)Application.Current.Resources["TextControlForeground"];
            double step = 30;
            for (double x = 0; x < 3000; x += step)
            {
                var line = new Microsoft.UI.Xaml.Shapes.Line
                {
                    X1 = x, Y1 = 0,
                    X2 = x, Y2 = 2000,
                    Stroke = themeBrush,
                    StrokeThickness = 1,
                    Opacity = 0.1
                };
                GridLinesCanvas.Children.Add(line);
            }
            for (double y = 0; y < 2000; y += step)
            {
                var line = new Microsoft.UI.Xaml.Shapes.Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = 3000,
                    Y2 = y,
                    Stroke = themeBrush,
                    StrokeThickness = 1,
                    Opacity = 0.1
                };
                GridLinesCanvas.Children.Add(line);
            }
        }
    }
}
