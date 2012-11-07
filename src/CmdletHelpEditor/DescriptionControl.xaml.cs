#region Using Directives

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CmdletHelpEditor.DataModel;

#endregion

namespace CmdletHelpEditor
{
    /// <summary>
    ///   Interaction logic for DescriptionControl.xaml
    /// </summary>
    public partial class DescriptionControl : UserControl
    {
        public DescriptionControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   Routed events when a Cmdet description item has lost focus.
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="args"> </param>
        private void saveCmdletDescription( object sender, RoutedEventArgs args )
        {
            saveCmdletDescription1();
        }

        /// <summary>
        ///   The routine called when a Cmdlet description item has lost focus.
        /// </summary>
        public void saveCmdletDescription1()
        {
            var Node = (TreeViewItem) MainWindow.NavControl.CmdletTreeView.SelectedItem;
            var desc = (cmdletDescription) Node.DataContext;
            desc.LongDescription = DescriptionControl1.DetailedDescriptionTextBox.Text;
            desc.ShortDescription = DescriptionControl1.ShortDescriptionTextBox.Text;
            desc.InputType = DescriptionControl1.InputTypeTextBox.Text;
            desc.InputDesc = DescriptionControl1.InputTypeDescTextBox.Text;
            desc.OutputType = DescriptionControl1.OutpuTypeTextBox.Text;
            desc.OutputDesc = DescriptionControl1.OutputTypeDescTextBox.Text;
            desc.Note = DescriptionControl1.NotesDescriptionTextBox.Text;
            if ( desc.LongDescription != null && desc.LongDescription != "" &&
                 desc.ShortDescription != null &&
                 desc.ShortDescription != "" )
            {
                Node.Foreground = Brushes.Green;
            }
        }
    }
}
