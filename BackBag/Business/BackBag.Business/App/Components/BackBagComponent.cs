
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using BackBag.Business.App.Entities;
using BackBag.Common.Collection;
using BackBag.Common.Common;
using BackBag.Common.Log;

namespace BackBag.Business.App.Components
{
    public sealed class BackBagComponent
    {
        private const string BACKBAG_PATH = "\\Data";

        private const string BACKBAG_PATH_FILE = "\\backbag.json";

        public const string APPHOST_LOCAL = "\\Data\\AppHost";

        public const string ICONS_LOCAL = "\\Data\\Icons";

        private const string APPHOST_LOCAL_FILE = "\\apphost.json";

        public BackBagModel BackBag { set; get; }

        public AppHostModel AppHost { set; get; }

        private BackBagComponent()
        {

        }

        public void Init()
        {
            try
            {
                var apphostLocalFileRaw = File.ReadAllText(CommonEnvironment.BaseDirectory + APPHOST_LOCAL + APPHOST_LOCAL_FILE);

                AppHost = NewtonJsonSerializer.Instance.Deserialize<AppHostModel>(apphostLocalFileRaw);
            }
            catch (Exception ex)
            {
                FileLogger.Instance.Log(ex);
            }

            try
            {
                var apphostRemoteFileRaw = File.ReadAllText(AppHost.AppHost + AppHost.AppHostFile);

                AppHost = NewtonJsonSerializer.Instance.Deserialize<AppHostModel>(apphostRemoteFileRaw);

                File.WriteAllText(CommonEnvironment.BaseDirectory + APPHOST_LOCAL + APPHOST_LOCAL_FILE, apphostRemoteFileRaw);
            }
            catch (Exception ex)
            {
                FileLogger.Instance.Log(ex);
            }

            try
            {
                var backbagRaw = File.ReadAllText(CommonEnvironment.BaseDirectory + BACKBAG_PATH + BACKBAG_PATH_FILE);

                BackBag = NewtonJsonSerializer.Instance.Deserialize<BackBagModel>(backbagRaw);
            }
            catch (Exception ex)
            {
                FileLogger.Instance.Log(ex);
            }

            InitApp();

            FlushBackBag();
        }

        public Task<AppInstallationModel> GetInstallAppTask(string name)
        {
            var appRaw = AppHost.Apps.Find(d => d.Name.AreEqual(name));

            var task = new Task<AppInstallationModel>(state =>
            {
                var installationModel = new AppInstallationModel();

                if (BackBag != null && BackBag.InstalledApps != null && BackBag.InstalledApps.Count > 0 && appRaw != null)
                {
                    var isUpToDate = BackBag.InstalledApps.Exists(d => d.Name.AreEqual(appRaw.Name) && d.Version >= appRaw.Version);

                    if (isUpToDate)
                    {
                        installationModel.Result = InstallAppResult.IsUpToDate;

                        return installationModel;
                    }
                }

                try
                {
                    var app = state as AppModel;

                    if (BackBag != null && app != null)
                    {
                        CopyTo(AppHost.AppHost + app.Path, CommonEnvironment.BaseDirectory + APPHOST_LOCAL + app.Path);

                        var installedApp = new AppModel();

                        installedApp.Name = app.Name;

                        installedApp.Version = app.Version;

                        installedApp.Path = app.Path;

                        installedApp.StartApp = app.StartApp;

                        installedApp.Icon = app.Icon;

                        if (BackBag.InstalledApps == null)
                        {
                            BackBag.InstalledApps = new List<AppModel>();
                        }

                        if (BackBag.InstalledApps != null)
                        {
                            var installedAppCurrent = BackBag.InstalledApps.FirstOrDefault(d => d.Name.AreEqual(installedApp.Name));

                            if (installedAppCurrent != null)
                            {
                                installedAppCurrent.Version = installedApp.Version;
                            }
                            else
                            {
                                BackBag.InstalledApps.Add(installedApp);
                            }

                            FlushBackBag();
                        }

                        installationModel.IsSuccess = true;

                        installationModel.Name = app.Name;
                    }
                }
                catch (Exception ex)
                {
                    FileLogger.Instance.Log(ex);
                }

                return installationModel;

            }, appRaw);

            return task;
        }

        private void InitApp()
        {
            if (BackBag != null && AppHost != null && AppHost != null && AppHost.Apps != null)
            {
                if (BackBag.InstalledApps == null)
                {
                    BackBag.InstalledApps = new List<AppModel>();
                }

                if (BackBag.UninstalledApps == null)
                {
                    BackBag.UninstalledApps = new List<AppModel>();
                }

                if (BackBag.InstalledApps.Count > 0)
                {
                    foreach (var app in BackBag.InstalledApps)
                    {
                        app.HasNew = AppHost.Apps.Exists(d => d.Name.AreEqual(app.Name) && app.Version < d.Version);
                    }
                }

                if (AppHost.Apps.Count > 0)
                {
                    foreach (var app in AppHost.Apps)
                    {
                        var installedApp = BackBag.InstalledApps.FirstOrDefault(d => d.Name.AreEqual(app.Name));

                        if (installedApp != null && installedApp.Version < app.Version)
                        {
                            installedApp.HasNew = true;
                        }

                        var uninstalledApp = BackBag.UninstalledApps.FirstOrDefault(d => d.Name.AreEqual(app.Name));

                        if (uninstalledApp != null)
                        {
                            uninstalledApp.Version = app.Version;
                        }
                        else
                        {
                            BackBag.UninstalledApps.Add(app);

                            try
                            {
                                var iconPath = CommonEnvironment.BaseDirectory + ICONS_LOCAL + app.Path;

                                var iconFilePath = CommonEnvironment.BaseDirectory + ICONS_LOCAL + app.Path + app.Icon;

                                if (!Directory.Exists(iconPath))
                                {
                                    Directory.CreateDirectory(iconPath);
                                }

                                if (!File.Exists(iconFilePath))
                                {
                                    File.Copy(AppHost.AppHost + app.Path + app.Icon, iconFilePath);
                                }
                            }
                            catch (Exception ex)
                            {
                                FileLogger.Instance.Log(ex);
                            }
                        }
                    }
                }
            }
        }

        private void FlushBackBag()
        {
            var backBagRaw = NewtonJsonSerializer.Instance.Serialize(BackBag, true);

            File.WriteAllText(CommonEnvironment.BaseDirectory + BACKBAG_PATH + BACKBAG_PATH_FILE, backBagRaw);
        }

        private void CopyTo(string source, string target)
        {
            if (!Directory.Exists(source))
            {
                return;
            }

            if (Directory.Exists(target))
            {
                try
                {
                    Directory.Delete(target, true);
                }
                catch (Exception ex)
                {
                    FileLogger.Instance.Log(ex);
                }
            }

            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }

            if (Directory.Exists(source) && Directory.Exists(target))
            {
                var files = Directory.GetFiles(source);

                var dirs = Directory.GetDirectories(source);

                if (files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileName(file);

                        if (!string.IsNullOrWhiteSpace(fileName))
                        {
                            var targetFile = target + "\\" + fileName;

                            if (File.Exists(targetFile))
                            {
                                try
                                {
                                    File.Copy(source + "\\" + fileName, targetFile, true);
                                }
                                catch (Exception ex)
                                {
                                    FileLogger.Instance.Log(ex);
                                }
                            }
                            else
                            {
                                File.Copy(source + "\\" + fileName, targetFile);
                            }
                        }
                    }
                }

                if (dirs.Length > 0)
                {
                    foreach (var dir in dirs)
                    {
                        var dirName = Path.GetFileName(dir);

                        if (!string.IsNullOrWhiteSpace(dirName))
                        {
                            var targetDir = target + "\\" + dirName;

                            if (!Directory.Exists(targetDir))
                            {
                                Directory.CreateDirectory(targetDir);
                            }

                            CopyTo(source + "\\" + dirName, targetDir);
                        }
                    }
                }
            }
        }

        #region Singleton

        public static BackBagComponent Instance { get { return InternalBackBagComponent.Instance; } }

        private class InternalBackBagComponent
        {
            // Tell C# compiler not to mark type as beforefieldinit
            static InternalBackBagComponent()
            {
            }

            internal static readonly BackBagComponent Instance = new BackBagComponent();
        }

        #endregion
    }

    public enum InstallAppResult
    {
        Invalid,

        IsUpToDate,

        IsInvoked
    }
}
