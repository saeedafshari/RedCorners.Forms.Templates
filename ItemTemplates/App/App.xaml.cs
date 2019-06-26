using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RedCorners.Forms;
using System.Threading.Tasks;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace $rootnamespace$
{
    public partial class $safeitemname$ : Application2
    {
        public override void InitializeSystems()
        {
            InitializeComponent();
            base.InitializeSystems();
        }

        public override Page GetFirstPage() => 
            new Views.MainPage();
    }
}
