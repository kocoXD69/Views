using Plugin.Media;
using Plugin.Media.Abstractions;
using ProjectMIBA_CS357.Models;
using ProjectMIBA_CS357.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProjectMIBA_CS357.Views
{
    public partial class NewItemPage : ContentPage
    {
        
        Services.LGDataStore lgdatastore = new Services.LGDataStore();
        MediaFile file;
        byte[] imageAsBytes = null;
        string fileName = "";

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
        public byte[] StreamToByteArray(Stream pSource)
        {
            using (var memoryStream = new MemoryStream())
            {
                pSource.CopyTo(memoryStream);
                imageAsBytes = memoryStream.ToArray();
            }
            return imageAsBytes;
        }
        private async void BtnTakePhoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Test",
                SaveToAlbum = true,
                CompressionQuality = 75,
                CustomPhotoSize = 50,
                PhotoSize = PhotoSize.MaxWidthHeight,
                MaxWidthHeight = 2000,
                DefaultCamera = CameraDevice.Rear
            }); ;

            if (file == null)
                return;

            await DisplayAlert("File Location", file.Path, "OK");

            imgChoosed.Source = ImageSource.FromStream(() =>
            {
                fileName = Path.GetFileName(file.Path);
                var imageAsBytes = StreamToByteArray(file.GetStream());
                file.Dispose();
                var stream = new MemoryStream(imageAsBytes);
                return stream;
            });
        }
        private async void BtnPick_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });

                if (file == null)
                    return;

                imgChoosed.Source = ImageSource.FromStream(() =>
                {
                    fileName = Path.GetFileName(file.Path);
                    var imageAsBytes = StreamToByteArray(file.GetStream());
                    file.Dispose();
                    var stream = new MemoryStream(imageAsBytes);
                    return stream;
                });

                if (imageAsBytes != null)
                {
                    using (var streamF = new MemoryStream(imageAsBytes))
                    {   
                        await lgdatastore.UploadFile(streamF, fileName);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

     
    }
}