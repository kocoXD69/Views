using ProjectMIBA_CS357.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace ProjectMIBA_CS357.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext  = new ItemDetailViewModel();
        }
       
    }
}