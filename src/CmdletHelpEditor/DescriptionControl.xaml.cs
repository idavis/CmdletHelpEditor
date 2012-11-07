using System;
using System.Collections.Generic;
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

namespace HelpEditorOS
{
	/// <summary>
	/// Interaction logic for DescriptionControl.xaml
	/// </summary>
	public partial class DescriptionControl : UserControl
	{
		public DescriptionControl()
		{
			this.InitializeComponent();
		}
        /// <summary>
        /// Routed events when a Cmdet description item has lost focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void saveCmdletDescription(object sender, RoutedEventArgs args)
        {
            saveCmdletDescription1();
        }

        /// <summary>
        /// The routine called when a Cmdlet description item has lost focus.
        /// </summary>
        public void saveCmdletDescription1()
        {
            TreeViewItem Node = (TreeViewItem)MainWindow.NavControl.CmdletTreeView.SelectedItem;
            cmdletDescription desc = (cmdletDescription)Node.DataContext;
            desc.LongDescription = this.DescriptionControl1.DetailedDescriptionTextBox.Text;
            desc.ShortDescription = this.DescriptionControl1.ShortDescriptionTextBox.Text;
            desc.InputType = this.DescriptionControl1.InputTypeTextBox.Text;
            desc.InputDesc = this.DescriptionControl1.InputTypeDescTextBox.Text;
            desc.OutputType = this.DescriptionControl1.OutpuTypeTextBox.Text;
            desc.OutputDesc = this.DescriptionControl1.OutputTypeDescTextBox.Text;
            desc.Note = this.DescriptionControl1.NotesDescriptionTextBox.Text;
            if (desc.LongDescription != null && desc.LongDescription != "" &&
                desc.ShortDescription != null && desc.ShortDescription != "")
            {
                Node.Foreground = Brushes.Green;
            }

        }

	}
}