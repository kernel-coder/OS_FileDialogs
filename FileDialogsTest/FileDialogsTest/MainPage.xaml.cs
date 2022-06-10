using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FileDialogsTest
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            // Enter construction logic here...
        }

        private async void Text_File_Button_Click(object sender, RoutedEventArgs e)
        {
            //var cd = new ChildWindow1();
            //cd.Show();
            var dialog = new FileDialogs.FileUploaderDialog();
            dialog.Filter = "Comma-delimited files (*.csv)|*.csv|Text files (*.txt)|*.txt";

            if (await dialog.ShowDialog())
            {
                var result = dialog.Files.FirstOrDefault();

                var sfd = new FileDialogs.SaveFileDialogEx()
                {
                    Filter = "Comma-delimited files (*.csv)|*.csv|Text files (*.txt)|*.txt",
                    DefaultExt = "txt",
                    DefaultFileName = "Test-download-text-file"
                };

                if (await sfd.ShowDialog())
                {
                    FileDialogs.SaveFileDialogEx.SaveContentToFile(result.Text, sfd.SafeFileName);
                }
            }
        }

        private async void ImagePDF_File_Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FileDialogs.FileUploaderDialog();
            dialog.Filter = "Images and PDFs (.png, .pdf, .jpg)|*.png;*.pdf;*.jpeg;*.jpg";
            dialog.ResultKind = FileDialogs.ResultKind.DataURL;
            

            if (await dialog.ShowDialog())
            {
                var result = dialog.Files.FirstOrDefault();
                string ext = result.Extension;
                BitmapImage bi = new BitmapImage();
                MemoryStream ms = new MemoryStream(result.Buffer);
                bi.SetSource(ms);
                imageView.Source = bi;

                var sfd = new FileDialogs.SaveFileDialogEx()
                {
                    Filter = "Images and PDFs (.png, .pdf, .jpg)|*.png;*.pdf;*.jpeg;*.jpg",
                    DefaultExt = "png",
                    DefaultFileName = "Test-download-image-file"
                };

                if (await sfd.ShowDialog())
                {
                    FileDialogs.SaveFileDialogEx.SaveBase64Content(result.Data, sfd.SafeFileName, "image/png;base64");
                }
            }
        }
    }
}
