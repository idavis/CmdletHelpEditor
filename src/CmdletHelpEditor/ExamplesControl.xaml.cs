#region Using Directives

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CmdletHelpEditor.DataModel;

#endregion

namespace CmdletHelpEditor
{
    /// <summary>
    ///   Interaction logic for ExamplesControl.xaml
    /// </summary>
    public partial class ExamplesControl : UserControl
    {
        public ExamplesControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   This routine handles the Save button on the Examples page
        ///   It adds the new record to the tree. 
        ///   If this is already in the tree it updates it with the latest 
        ///   info in the page.
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="args"> </param>
        public void AddExample_Click( object sender, RoutedEventArgs args )
        {
            var SelectedNode = (TreeViewItem) MainWindow.NavControl.CmdletTreeView.SelectedItem;
            if ( SelectedNode != null )
            {
                int count = SelectedNode.Items.Count;
                if ( ExamplesControl1.ExampleNameTextBox.Text == "" )
                {
                    ExamplesControl1.ExampleNameTextBox.Text = "Example " + ( count + 1 ).ToString();
                }

                if ( ExamplesControl1.ExampleID.Text == "" )
                        //This is a new example
                {
                    //Add new exampe info.
                    var ParentNode = new TreeViewItem();
                    if ( (String) SelectedNode.Header == "Examples" )
                    {
                        ParentNode = (TreeViewItem) SelectedNode.Parent;
                    }
                    else
                    {
                        ParentNode = (TreeViewItem) SelectedNode.Parent;
                        ParentNode = (TreeViewItem) ParentNode.Parent;
                    }

                    var Cmdletdesc = (cmdletDescription) ParentNode.DataContext;
                    var examDetails = new example();
                    examDetails.ExampleCmd = ExamplesControl1.ExampleCommandTextBox.Text;
                    examDetails.ExampleDescription = ExamplesControl1.ExampleDescriptionTextBox.Text;
                    examDetails.ExampleName = ExamplesControl1.ExampleNameTextBox.Text;
                    examDetails.ExampleOutput = ExamplesControl1.ExampleOutputTextBox.Text;
                    examDetails.ExampleID = count;
                    var NewExampleNode = new TreeViewItem();
                    NewExampleNode.DataContext = examDetails;
                    NewExampleNode.Header = examDetails.ExampleName;
                    if ( Cmdletdesc.Examples == null )
                    {
                        var Examples = new Collection<example>();
                        Examples.Add( examDetails );
                        Cmdletdesc.Examples = Examples;
                        SelectedNode.DataContext = Examples;
                    }
                    else
                    {
                        Cmdletdesc.Examples.Add( examDetails );
                    }
                    MainWindow.ResetExamplesPage();
                    if ( (String) SelectedNode.Header == "Examples" )
                    {
                        if ( sender != null )
                        {
                            SelectedNode.Items.Add( NewExampleNode );
                            SelectedNode.IsExpanded = true;
                            //int count = SelectedNode.Items.Count;
                            var ChildNode = (TreeViewItem) SelectedNode.Items[count];
                            ChildNode.IsSelected = true;
                        }
                        else
                        {
                            SelectedNode.Items.Add( NewExampleNode );
                            SelectedNode.IsExpanded = true;
                        }
                    }
                }


                else //this is an existing example
                {
                    //Update Example info.
                    if ( SelectedNode.Header.ToString() == "Examples" )
                    {
                        var examDetails = (Collection<example>) SelectedNode.DataContext;
                        var examDetail = new example();
                        examDetail.ExampleCmd = ExamplesControl1.ExampleCommandTextBox.Text;
                        examDetail.ExampleDescription = ExamplesControl1.ExampleDescriptionTextBox.Text;
                        examDetail.ExampleName = ExamplesControl1.ExampleNameTextBox.Text;
                        examDetail.ExampleOutput = ExamplesControl1.ExampleOutputTextBox.Text;
                        examDetails.Add( examDetail );
                        //SelectedNode.Header = this.ExampleNameTextBox.Text;
                    }
                    else
                    {
                        var examDetails = (example) SelectedNode.DataContext;
                        examDetails.ExampleCmd = ExamplesControl1.ExampleCommandTextBox.Text;
                        examDetails.ExampleDescription = ExamplesControl1.ExampleDescriptionTextBox.Text;
                        examDetails.ExampleName = ExamplesControl1.ExampleNameTextBox.Text;
                        examDetails.ExampleOutput = ExamplesControl1.ExampleOutputTextBox.Text;
                        SelectedNode.Header = ExamplesControl1.ExampleNameTextBox.Text;
                    }
                }
            }
        }

        /// <summary>
        ///   Create a new Example record in the tree
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        public void NewExample_Click( object sender, RoutedEventArgs e )
        {
            var ChildNode = (TreeViewItem) MainWindow.NavControl.CmdletTreeView.SelectedItem;
            var Header = (String) ChildNode.Header;
            if ( Header.ToLower() != "examples" )
            {
                ChildNode = (TreeViewItem) ChildNode.Parent;
                ChildNode.IsSelected = true;
            }
            MainWindow.ResetExamplesPage();
        }

        /// <summary>
        ///   This routine removes examples from the tree.
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="args"> </param>
        public void DeleteExample_Click( object sender, RoutedEventArgs args )
        {
            if ( ExamplesControl1.ExampleID.Text == "" )
            {
                //Safe to simply reset the Examples page.
                MainWindow.ResetExamplesPage();
            }
            else //We must remove the current record from the Examples list
            {
                var selectedNode = (TreeViewItem) MainWindow.NavControl.CmdletTreeView.SelectedItem;
                var ParentNode = (TreeViewItem) selectedNode.Parent;
                var CmdletNode = (TreeViewItem) ParentNode.Parent;
                ParentNode.IsSelected = true;
                //int ItemToRemove = selectedNode.Items.IndexOf();
                //
                var selectedCmdlet = (cmdletDescription) CmdletNode.DataContext;
                var selectedExample = (example) selectedNode.DataContext;

                foreach ( cmdletDescription mycmdlet in MainWindow.CmdletsHelps )
                {
                    if ( mycmdlet.CmdletName ==
                         selectedCmdlet.CmdletName )
                    {
                        Collection<example> myExamples = mycmdlet.Examples;
                        var exampletoRemove = new example();
                        foreach ( example myexample in myExamples )
                        {
                            if ( myexample.ExampleID ==
                                 selectedExample.ExampleID )
                            {
                                exampletoRemove = myexample;
                            }
                        }
                        if ( exampletoRemove.ExampleID.ToString() != null )
                        {
                            mycmdlet.Examples.Remove( exampletoRemove );
                            ParentNode.Items.Remove( selectedNode );
                        }
                    }
                }
            }
        }
    }
}
