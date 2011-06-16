using System.Dynamic;
using System.Windows.Controls.Primitives;
using Caliburn.Micro;
using CQRS.Sample1.Client.Domains.Products;

namespace CQRS.Sample1.Client
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive
    {
        private readonly IWindowManager windowManager;
        public ShellViewModel() //IWindowManager windowManager)
        {
            //this.windowManager = windowManager;
            ActivateItem(new ProductListViewModel());
        }

        public void OpenModeless()
        {
            windowManager.ShowWindow(new DialogViewModel(), "Modeless");
        }

        public void OpenModal()
        {
            var result = windowManager.ShowDialog(new DialogViewModel());
        }

        public void OpenPopup()
        {
            dynamic settings = new ExpandoObject();
            settings.Placement = PlacementMode.Center;
            settings.PlacementTarget = GetView(null);

            windowManager.ShowPopup(new DialogViewModel(), "Popup", settings);
        }
    }
}
