using Plugin.Media;
using Plugin.Media.Abstractions;
using ProjectMIBA_CS357.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProjectMIBA_CS357.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateProductPage : ContentPage
    {
        Services.LGDataStore lgdatastore = new Services.LGDataStore();
        MediaFile file;
        byte[] imageAsBytes = null;
        string fileName = "";

        public UpdateProductPage()
        {
            InitializeComponent();
            BindingContext = new UpdateProductVM();
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
                        await lgdatastore.UpdateUploadFile(streamF, fileName);
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