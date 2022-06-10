using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileDialogs
{
    public class FileDownloaderDialog : ChildWindow
    {
        public event EventHandler Accept;
        public event EventHandler Cancel;
        TaskCompletionSource<bool> tcs;

        TextBox fileName = new TextBox();

        public FileDownloaderDialog(string defaultText = "")
        {
            this.Title = "Save As";
            Grid grid = new Grid();
            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);

            Button okButton = new Button();
            okButton.Click += OkButton_Click;
            okButton.Width = 75;
            okButton.Height = 23;
            okButton.Margin = new Thickness(5);
            okButton.Content = "OK";

            Button cancelButton = new Button();
            cancelButton.Click += CancelButton_Click;
            cancelButton.Width = 75;
            cancelButton.Height = 23;
            cancelButton.Margin = new Thickness(5);
            cancelButton.Content = "Cancel";


            fileName.Text = defaultText;
            fileName.Width = 200;
            fileName.Height = 23;
            fileName.Margin = new Thickness(10);

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            panel.Children.Add(okButton);
            panel.Children.Add(cancelButton);
            panel.Margin = new Thickness(10);
            panel.HorizontalAlignment = HorizontalAlignment.Center;

            Grid.SetRow(fileName, 0);
            Grid.SetRow(panel, 1);
            grid.Children.Add(panel);
            grid.Children.Add(fileName);

            this.Content = grid;

            tcs = new TaskCompletionSource<bool>();
        }

        public string FileName
        {
            get { return fileName.Text; }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Cancel?.Invoke(this, new EventArgs());
            tcs.SetResult(false);
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Accept?.Invoke(this, new EventArgs());
            tcs.SetResult(true);
            this.Close();
        }

        public bool ShowDialog()
        {
            this.Show();
            return true;
        }

        public Task<bool> ShowDialogAsync()
        {
            this.Show();
            return tcs.Task;
        }
    }

    public class FileSaver
    {
        static bool JSLibraryWasLoaded;

        public static async Task SaveTextToFile(string text, string filename)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (filename == null)
                throw new ArgumentNullException("filename");

            if (await Initialize())
            {
                CSHTML5.Interop.ExecuteJavaScript(@"
                    var blob = new Blob([$0], { type: ""text/plain;charset=utf-8""});
                    saveAs(blob, $1)", text, filename);
            }
        }

        // Function accepts base64 string for zip file
        public static async Task SaveZipToFile(string text, string filename)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (filename == null)
                throw new ArgumentNullException("filename");

            if (await Initialize())
            {
                CSHTML5.Interop.ExecuteJavaScript(@"
                    var raw = window.atob($0);
                    var rawLength = raw.length;
                    uInt8Array = new Uint8Array(rawLength);
                    for (let i = 0; i < rawLength; ++i) {
                        uInt8Array[i] = raw.charCodeAt(i);
                    }
                    var blob = new Blob([uInt8Array], { type: ""application/x-zip-compressed;base64""});
                    saveAs(blob, $1)", text, filename);
            }
        }

        public static async Task SaveJavaScriptBlobToFile(object javaScriptBlob, string filename)
        {
            if (javaScriptBlob == null)
                throw new ArgumentNullException("javaScriptBlob");

            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            if (await Initialize())
            {
                CSHTML5.Interop.ExecuteJavaScript(@"saveAs($0, $1)", javaScriptBlob, filename);
            }
        }

        public static async Task SaveContentToFile(object content, string filename, string contentType = "text/plain;charset=utf-8")
        {
            if (content == null)
                throw new ArgumentNullException("content");

            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            if (await Initialize())
            {
                string javaSript = @"var blob = new Blob([$0], { type: """;
                javaSript += contentType;
                javaSript += @"""}); window.saveAs(blob, $1)";
                OpenSilver.Interop.ExecuteJavaScript(javaSript, content, filename);
            }
        }

        static async Task<bool> Initialize()
        {
            if (CSHTML5.Interop.IsRunningInTheSimulator_WorkAround)
            {
                MessageBox.Show("Saving files is currently not supported in the Simulator. Please run in the browser instead.");
                return false;
            }
            //JSLibraryWasLoaded = true;
            //return true;

            if (!JSLibraryWasLoaded)
            {
                await CSHTML5.Interop.LoadJavaScriptFile("https://cdnjs.cloudflare.com/ajax/libs/FileSaver.js/2014-11-29/FileSaver.min.js");
                JSLibraryWasLoaded = true;
            }
            return true;
        }
    }
}
