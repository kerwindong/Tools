
using BackBag.Business.App.Components;

namespace BackBag.Business.App.Entities
{
    public class AppInstallationModel
    {
        public string Name { set; get; }

        public InstallAppResult Result { set; get; }

        public bool IsSuccess { set; get; }
    }
}
