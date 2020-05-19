using Microsoft.Win32;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace DBSysReport
{
    [RunInstaller(true)]
    public partial class InstallerClass : System.Configuration.Install.Installer
    {
        private const string environmentKey = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";

        public InstallerClass()
        {
            InitializeComponent();
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            // string environmentVar = Environment.GetEnvironmentVariable("PATH");
            string dbSysPath = Context.Parameters["targetdir"];
            while (dbSysPath[dbSysPath.Length - 1] == '\\')
                dbSysPath = dbSysPath.Substring(0, dbSysPath.Length - 1);

            //get non-expanded PATH environment variable
            RegistryKey key = Registry.LocalMachine.CreateSubKey(environmentKey);
            string path = (string)key.GetValue("Path", "", RegistryValueOptions.DoNotExpandEnvironmentNames);

            if (path.IndexOf(dbSysPath) < 0)
            {
                //set the path as an an expandable string
                key.SetValue("Path", path + ";" + dbSysPath, RegistryValueKind.ExpandString);
            }
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            //get non-expanded PATH environment variable
            RegistryKey key = Registry.LocalMachine.CreateSubKey(environmentKey);
            string path = (string)key.GetValue("Path", "", RegistryValueOptions.DoNotExpandEnvironmentNames);

            // string removeString: ";DBSys";
            foreach (string dbSysPath in path.Split(';'))
            {
                Regex regex = new Regex("DBSysReport");
                Match match = regex.Match(dbSysPath);

                if (match.Success)
                {
                    //set the path as an an expandable string
                    path = path.Replace(dbSysPath + ";", "");
                    key.SetValue("Path", path, RegistryValueKind.ExpandString);
                    break;
                }
            }

            base.Uninstall(savedState);
        }
    }
}
