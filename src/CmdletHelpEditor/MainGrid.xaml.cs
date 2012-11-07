#region Using Directives

using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Windows.Controls;
using CmdletHelpEditor.DataModel;

#endregion

namespace CmdletHelpEditor
{
    /// <summary>
    ///   Interaction logic for MainGrid.xaml
    /// </summary>
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
            ps.AddCommand( "get-module" ).AddParameter( "listavailable" );
            Collection<PSObject> modules = ps.Invoke();

            // ObservableCollection<ModuleObject> availableModules = new ObservableCollection<ModuleObject>();

            foreach ( PSObject PsSnapin in modules )
            {
                var module = new ModuleObject();
                module.Name = (String) PsSnapin.Members["Name"].Value;
                module.Version = PsSnapin.Members["version"].Value.ToString();
                module.Descrition = (String) PsSnapin.Members["Description"].Value;
                module.ModuleType = PsSnapin.Members["ModuleType"].Value.ToString();
                myModules.Add( module );
                PsSnapinList.Items.Add( module );
            }
        }

        private void PsSnapinList_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            MainWindow.selectedModule = (ModuleObject) PsSnapinList.SelectedItem;
        }
    }
}
