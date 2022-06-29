using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BK_Restore
{
    public partial class FormLogin : XtraForm
    {
        private bool isLoadServers = false;

        public FormLogin()
        {
            InitializeComponent();
            txtPassword.Properties.PasswordChar = '•';
            //cboServers.SelectedIndex = 0;
        }

        public async Task GetListServer()
        {
            cboServers.Items.Clear();
            String serverName = Environment.MachineName;

            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey registry = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey intansKey = registry.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                if (intansKey != null)
                {
                    foreach (var instanceName in intansKey.GetValueNames())
                    {
                        cboServers.Items.Add(serverName + "\\" + instanceName);
                    }
                }
            }
            cboServers.SelectedIndex = 0;
        }

        private async void cboServers_DropDown(object sender, EventArgs e)
        {
            if (isLoadServers == false)
            {
                SplashScreenManager.ShowForm(this, typeof(WaitFormCustom), false, false, false, ParentFormState.Locked);
                await GetListServer();
                SplashScreenManager.CloseForm(false);
                isLoadServers = true;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Program.ServerName = cboServers.Text;
            Program.UserName = txtLogin.Text;
            Program.Password = txtPassword.Text;

            if (Program.Connect() == true)
            {
                FormMain formMain = new FormMain();
                this.Hide();
                formMain.Closed += (s, args) =>
                {
                    this.Show();
                };
                formMain.Show();
            }
            else
            {
                XtraMessageBox.Show("Đăng nhập thất bại", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
