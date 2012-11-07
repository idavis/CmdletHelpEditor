using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;

namespace HelpEditorOS
{
    /// <summary>
    /// Interaction logic for MainGrid.xaml
    /// </summary>
    /// 
    public partial class MainGrid : UserControl
    {
        
        public static ObservableCollection<ModuleObject> myModules = new ObservableCollection<ModuleObject>();
        
        public MainGrid()
        {
            InitializeComponent();
            LoadModules();         
        }

        public void LoadModules()
        {
            PowerShell ps = PowerShell.Create();
            ps.AddCommand("get-module").AddParameter("listavailable");
            Collection<PSObject> modules = ps.Invoke();

            // ObservableCollection<ModuleObject> availableModules = new ObservableCollection<ModuleObject>();

            foreach (PSObject PsSnapin in modules)
            {
                ModuleObject module = new ModuleObject();
                module.Name = (String)PsSnapin.Members["Name"].Value;
                module.Version = (String)PsSnapin.Members["version"].Value.ToString();
                module.Descrition = (String)PsSnapin.Members["Description"].Value;
                module.ModuleType = (String)PsSnapin.Members["ModuleType"].Value.ToString();
                myModules.Add(module);
                this.PsSnapinList.Items.Add(module);
            }

        }

        private void PsSnapinList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HelpEditorOS.MainWindow.selectedModule = (ModuleObject)this.PsSnapinList.SelectedItem;
        }
    }
}
