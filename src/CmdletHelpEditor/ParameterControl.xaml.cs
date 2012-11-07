#region Using Directives

using System;
using System.Windows;
using System.Windows.Controls;
using CmdletHelpEditor.DataModel;

#endregion

namespace CmdletHelpEditor
{
    /// <summary>
    ///   Interaction logic for ParameterControl.xaml
    /// </summary>
    public partial class ParametersControl : UserControl
    {
        public ParametersControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   This routine is an event handler. 
        ///   I call the save Parameter Description routine everytime a 
        ///   LostFocus happened on an editable field in the Cmdlet description page.
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="args"> </param>
        private void saveParameterDescription( object sender, RoutedEventArgs args )
        {
            saveParameterDescription1();
        }

        /// <summary>
        ///   Save parameter data when the Globiing parameter is checked.
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void GlobbingParam_Checked( object sender, RoutedEventArgs e )
        {
            saveParameterDescription1();
        }

        /// <summary>
        ///   Called when parameters info gets lost focus
        /// </summary>
        public void saveParameterDescription1()
        {
            var Node = (TreeViewItem) MainWindow.NavControl.CmdletTreeView.SelectedItem;
            if ( Node != null )
            {
                var param = (parameterDecription) Node.DataContext;
                param.NewDescription = ParametersControl1.ParameterDescTextBox.Text;
                param.DefaultValue = ParametersControl1.DefaultValueTextBox.Text;
                if ( (Boolean) ParametersControl1.GlobbingCheckBox.IsChecked )
                {
                    param.Globbing = true;
                }
                else
                {
                    param.Globbing = false;
                }
            }
        }
    }
}
