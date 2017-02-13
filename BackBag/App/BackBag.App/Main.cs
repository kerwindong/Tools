
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using BackBag.Business.App.Components;
using BackBag.Common.Common;
using BackBag.Common.Log;

namespace BackBag.App
{
    public partial class Main : Form
    {
        private const string INSTALLING = "Installing";

        private const string INSTALL_TEXT = "Install";

        private const string UPGRADING = "Upgrading";

        private const string UPGRADE_TEXT = "Upgrade";

        private const string OPEN_TEXT = "Run";

        private const string OPEN_CONTAINER_FOLDER_TEXT = "Open Container Folder";

        private const string NEW_VERSION_TIP = " (New!)";

        private const string APP_NAME = "Stormtrooper V1.0.0";

        private readonly static Size DEFAULT_SIZE = new Size(32, 32);

        private ContextMenuStrip stripMenu = new ContextMenuStrip();

        private string appNameSelected = string.Empty;

        private NotifyIcon notifyIcon = new NotifyIcon();

        public Main()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            Text = APP_NAME;

            var imageList = new ImageList();

            this.Resize += Main_Resize;

            BackBagComponent.Instance.Init();

            foreach (var app in BackBagComponent.Instance.BackBag.UninstalledApps)
            {
                var image = Image.FromFile(CommonEnvironment.BaseDirectory + BackBagComponent.ICONS_LOCAL + app.Path + app.Icon);

                imageList.Images.Add(app.Name, image);

                var item = new ListViewItem(app.Name, app.Name);

                item.Name = app.Name;

                UninstalledListView.Items.Add(item);
            }

            foreach (var app in BackBagComponent.Instance.BackBag.InstalledApps)
            {
                var item = new ListViewItem(app.Name, app.Name);

                item.Name = app.Name;

                if (app.HasNew)
                {
                    item.ForeColor = Color.Crimson;

                    item.Text = item.Text + NEW_VERSION_TIP;
                }

                InstalledListView.Items.Add(item);
            }

            InstalledListView.LargeImageList = imageList;

            InstalledListView.LargeImageList.ImageSize = DEFAULT_SIZE;

            InstalledListView.View = View.LargeIcon;

            InstalledListView.MouseDoubleClick += OpenApp;

            InstalledListView.MouseClick += ShowStripMenuForInstalledView;

            UninstalledListView.LargeImageList = imageList;

            UninstalledListView.LargeImageList.ImageSize = DEFAULT_SIZE;

            UninstalledListView.View = View.LargeIcon;

            UninstalledListView.MouseClick += ShowStripMenuForUninstalledView;

            MouseClick += HideStripMenu;
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon.Icon = new Icon("Stormtrooper.ico");

                notifyIcon.Text = "Stormtrooper";

                notifyIcon.Visible = true;

                notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;

                this.ShowInTaskbar = false;
            }
        }

        public void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon.Visible = false;

            this.WindowState = FormWindowState.Normal;
        }

        public void OpenApp(object sender, MouseEventArgs e)
        {
            var view = sender as ListView;

            var appItem = default(ListViewItem);

            if (view != null)
            {
                appItem = view.FocusedItem;
            }

            if (e.Button == MouseButtons.Left && appItem != null)
            {
                var app = BackBagComponent.Instance.BackBag.InstalledApps.FirstOrDefault(d => d.Name.AreEqual(appItem.Name));

                if (app != null)
                {
                    var startInfo = new ProcessStartInfo(CommonEnvironment.BaseDirectory + BackBagComponent.APPHOST_LOCAL + app.Path + app.StartApp);

                    startInfo.WindowStyle = ProcessWindowStyle.Normal;

                    Process.Start(startInfo);
                }
            }
        }

        public void HideStripMenu(object sender, MouseEventArgs e)
        {
            if (stripMenu.Visible && e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                var isInStripMenu = e.X >= stripMenu.Left &&
                                    e.X <= stripMenu.Left + stripMenu.Width &&
                                    Height - e.Y >= stripMenu.Top &&
                                    Height - e.Y <= stripMenu.Top + stripMenu.Height;

                if (!isInStripMenu)
                {
                    stripMenu.Hide();
                }
            }
        }

        public void ShowStripMenuForInstalledView(object sender, MouseEventArgs e)
        {
            var view = sender as ListView;

            var appItem = default(ListViewItem);

            if (view != null)
            {
                appItem = view.FocusedItem;
            }

            if (e.Button == MouseButtons.Right && appItem != null)
            {
                appNameSelected = appItem.Name;

                stripMenu.Hide();

                stripMenu = new ContextMenuStrip();

                var openButton = new ToolStripButton();

                openButton.Text = OPEN_TEXT;

                openButton.Name = appItem.Name;

                openButton.Padding = new Padding(0, 3, 0, 3);

                openButton.Width = 120;

                openButton.MouseDown += OpenAppFromMenu;

                stripMenu.Items.Add(openButton);

                var upgradeButton = new ToolStripButton();

                upgradeButton.Text = UPGRADE_TEXT;

                upgradeButton.Padding = new Padding(0, 3, 0, 3);

                upgradeButton.Width = 120;

                upgradeButton.Click += Upgrade;

                stripMenu.Items.Add(upgradeButton);

                var openFolderButton = new ToolStripButton();

                openFolderButton.Text = OPEN_CONTAINER_FOLDER_TEXT;

                openFolderButton.Name = appItem.Name;

                openFolderButton.Padding = new Padding(0, 3, 0, 3);

                openFolderButton.Width = 120;

                openFolderButton.MouseDown += OpenContainerFolder;

                stripMenu.Items.Add(openFolderButton);

                stripMenu.Show(view, e.Location);
            }
        }

        public void OpenAppFromMenu(object sender, MouseEventArgs e)
        {
            var button = sender as ToolStripButton;

            if (e.Button == MouseButtons.Left && button != null)
            {
                var app = BackBagComponent.Instance.BackBag.InstalledApps.FirstOrDefault(d => d.Name.AreEqual(button.Name));

                if (app != null)
                {
                    try
                    {
                        var startInfo = new ProcessStartInfo(CommonEnvironment.BaseDirectory + BackBagComponent.APPHOST_LOCAL + app.Path + app.StartApp);

                        startInfo.WindowStyle = ProcessWindowStyle.Normal;

                        startInfo.Domain = app.Name;

                        startInfo.CreateNoWindow = true;

                        startInfo.UseShellExecute = false;

                        using (var process = Process.Start(startInfo))
                        {
                            if (process != null)
                            {
                                process.WaitForExit();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        FileLogger.Instance.Log(ex);
                    }
                }
            }
        }

        public void OpenContainerFolder(object sender, MouseEventArgs e)
        {
            var button = sender as ToolStripButton;

            if (e.Button == MouseButtons.Left && button != null)
            {
                var app = BackBagComponent.Instance.BackBag.InstalledApps.FirstOrDefault(d => d.Name.AreEqual(button.Name));

                if (app != null)
                {
                    Process.Start(CommonEnvironment.BaseDirectory + BackBagComponent.APPHOST_LOCAL + app.Path);
                }
            }
        }

        public void Upgrade(object sender, EventArgs e)
        {
            stripMenu.Hide();

            var items = InstalledListView.Items.Find(appNameSelected, false);

            if (items.Length > 0)
            {
                items[0].Text = UPGRADING;

                var task = BackBagComponent.Instance.GetInstallAppTask(appNameSelected);

                task.ContinueWith(t =>
                {
                    var itemsInstalled = InstalledListView.Items.Find(appNameSelected, false);

                    if (itemsInstalled.Length > 0)
                    {
                        var itemInstalled = itemsInstalled[0];

                        itemInstalled.Text = appNameSelected;

                        if (t.Result.IsSuccess)
                        {
                            itemInstalled.ForeColor = Color.Black;

                            itemInstalled.Text = itemInstalled.Name;
                        }

                        appNameSelected = string.Empty;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());

                task.Start();
            }
        }

        public void ShowStripMenuForUninstalledView(object sender, MouseEventArgs e)
        {
            var view = sender as ListView;

            var appItem = default(ListViewItem);

            if (view != null)
            {
                appItem = view.FocusedItem;
            }

            if (e.Button == MouseButtons.Right && appItem != null)
            {
                appNameSelected = appItem.Name;

                stripMenu.Hide();

                stripMenu = new ContextMenuStrip();

                var installButton = new ToolStripButton();

                installButton.Text = INSTALL_TEXT;

                installButton.Padding = new Padding(0, 3, 0, 3);

                installButton.Width = 45;

                installButton.Click += Install;

                stripMenu.Items.Add(installButton);

                stripMenu.Show(view, e.Location);
            }
        }

        public void Install(object sender, EventArgs e)
        {
            stripMenu.Hide();

            var items = UninstalledListView.Items.Find(appNameSelected, false);

            if (items.Length > 0)
            {
                items[0].Text = INSTALLING;

                var task = BackBagComponent.Instance.GetInstallAppTask(appNameSelected);

                task.ContinueWith(t =>
                {
                    var itemsUninstall = UninstalledListView.Items.Find(appNameSelected, false);

                    if (itemsUninstall.Length > 0)
                    {
                        var itemUninstall = itemsUninstall[0];

                        itemUninstall.Text = appNameSelected;

                        if (t.Result.IsSuccess)
                        {
                            var item = new ListViewItem(itemUninstall.Name, itemUninstall.Name);

                            item.Name = itemUninstall.Name;

                            var itemsInstalled = InstalledListView.Items.Find(itemUninstall.Name, false);

                            if (itemsInstalled.Length > 0)
                            {
                                var itemInstalled = itemsInstalled[0];

                                itemInstalled.ForeColor = Color.Black;

                                itemInstalled.Text = itemUninstall.Name;
                            }
                            else
                            {
                                InstalledListView.Items.Add(item);
                            }
                        }
                    }

                    appNameSelected = string.Empty;

                }, TaskScheduler.FromCurrentSynchronizationContext());

                task.Start();
            }
        }
    }
}
