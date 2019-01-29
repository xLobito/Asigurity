using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsigurityLightweight.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content.PM;
using Android.App;

namespace AsigurityLightweight.Implementations.Tests
{
    [TestClass()]
    public class OpenApplicationTests
    {
        [TestMethod()]
        public void OpenSpecificApplicationOnIntentTest()
        {
            PackageInfoFlags PkgInfoFlags = PackageInfoFlags.MetaData | PackageInfoFlags.SharedLibraryFiles | PackageInfoFlags.UninstalledPackages;
            List<ApplicationInfo> AllApplications = Application.Context.PackageManager.GetInstalledApplications(PkgInfoFlags).ToList();
            List<ApplicationInfo> ApplicationsList = AllApplications.Where(AppInfo => bool.Parse(AppInfo.Flags.ToString()) & 1 != 0).ToList();
        }
    }
}