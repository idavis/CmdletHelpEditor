﻿#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Xml;
using CmdletHelpEditor.DataModel;
using MessageBox = System.Windows.Forms.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

#endregion

//using System.Windows.Forms;
//using System.Windows;

namespace CmdletHelpEditor
{
    /// <summary>
    ///   Interaction logic for VavigationControl.xaml
    /// </summary>
    public partial class NavigationControl : UserControl
    {
        public NavigationControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   Event handler for tree view selection change event.
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="args"> </param>
        public void TreeViewSlectionChange_Click( object sender, RoutedEventArgs args )
        {
            try
            {
                //Validate that the tree is not null and has items.
                if ( MainWindow.NavControl.CmdletTreeView.Items.Count > 0 )
                {
                    // Switch actions based on the selection context.
                    // We show and hide the necessary grids and populate the appropriate data.
                    var SelectedNode = (TreeViewItem) MainWindow.NavControl.CmdletTreeView.SelectedItem;
                    if ( SelectedNode != null )
                    {
                        var SelectedValue = (String) SelectedNode.Header;
                        switch ( SelectedValue )
                        {
                            case "Examples":
                                MainWindow.ParametersControl1.Visibility = Visibility.Collapsed;
                                MainWindow.DescriptionControl1.Visibility = Visibility.Collapsed;
                                MainWindow.ExamplesControl1.Visibility = Visibility.Visible;
                                MainWindow.RelatedLinks1.Visibility = Visibility.Collapsed;
                                MainWindow.EmptyParameterControl1.Visibility = Visibility.Collapsed;
                                // MainWindow.NavigationSplitter.Visibility = Visibility.Visible;
                                MainWindow.ResetExamplesPage();
                                // We always show the spec help even for new and empty example help records.
                                if ( MainWindow.ProjectName != null &&
                                     MainWindow.ProjectName != "" )
                                {
                                    MainWindow.ExamplesControl1.OldExampleDescTextBox.Text =
                                            (String) SelectedNode.Resources["SpecExample"];
                                }
                                break;
                            case "Parameters":
                                MainWindow.ParametersControl1.Visibility = Visibility.Collapsed;
                                MainWindow.DescriptionControl1.Visibility = Visibility.Collapsed;
                                MainWindow.ExamplesControl1.Visibility = Visibility.Collapsed;
                                MainWindow.RelatedLinks1.Visibility = Visibility.Collapsed;
                                MainWindow.EmptyParameterControl1.Visibility = Visibility.Visible;
                                //MainWindow.NavigationSplitter.Visibility = Visibility.Visible;
                                break;
                            case "Related Links":
                                MainWindow.ParametersControl1.Visibility = Visibility.Collapsed;
                                MainWindow.DescriptionControl1.Visibility = Visibility.Collapsed;
                                MainWindow.ExamplesControl1.Visibility = Visibility.Collapsed;
                                MainWindow.RelatedLinks1.Visibility = Visibility.Visible;
                                MainWindow.EmptyParameterControl1.Visibility = Visibility.Collapsed;
                                // MainWindow.NavigationSplitter.Visibility = Visibility.Visible;
                                MainWindow.ResetLinksPage();
                                // Same as we did with the examples above.
                                if ( MainWindow.ProjectName != null &&
                                     MainWindow.ProjectName != "" )
                                {
                                    MainWindow.RelatedLinks1.OldRelatedLinkTextBox.Text =
                                            (String) SelectedNode.Resources["SpecLinks"];
                                }
                                break;
                                // in this case if an item is selected then we call the code doDefaultTreeViewSelectionChange below.
                            default:
                                MainWindow.ParametersControl1.Visibility = Visibility.Collapsed;
                                MainWindow.DescriptionControl1.Visibility = Visibility.Collapsed;
                                MainWindow.ExamplesControl1.Visibility = Visibility.Collapsed;
                                MainWindow.RelatedLinks1.Visibility = Visibility.Collapsed;
                                MainWindow.EmptyParameterControl1.Visibility = Visibility.Collapsed;
                                //MainWindow.NavigationSplitter.Visibility = Visibility.Visible;
                                doDefaultTreeViewSelectionChange();
                                break;
                        }
                    }
                    else
                    {
                        MainWindow.ParametersControl1.Visibility = Visibility.Collapsed;
                        MainWindow.DescriptionControl1.Visibility = Visibility.Collapsed;
                    }
                }
            }

            catch ( Exception ex )
            {
                MessageBox.Show( ex.Message, "Application error.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
            }
        }

        /// <summary>
        ///   This routine handles the copy right click functionality
        ///   on the context menu on the tree.
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        public void OnCopySelection( object sender, RoutedEventArgs e )
        {
            var SelectedItem = (TreeViewItem) NavigationControl1.CmdletTreeView.SelectedItem;
            if ( SelectedItem.DataContext != null )
            {
                if ( SelectedItem.DataContext.GetType().Name == "cmdletDescription" )
                {
                    MainWindow.ClipBoardRecord.ClipBoardItem = SelectedItem.DataContext;
                    MainWindow.ClipBoardRecord.TypeName = "cmdlet description info";
                }
                else if ( SelectedItem.DataContext.GetType().Name == "parameterDecription" )
                {
                    MainWindow.ClipBoardRecord.ClipBoardItem = SelectedItem.DataContext;
                    MainWindow.ClipBoardRecord.TypeName = "parameter decription info";
                }
                else if ( SelectedItem.DataContext.GetType().Name == "example" )
                {
                    MainWindow.ClipBoardRecord.ClipBoardItem = SelectedItem.DataContext;
                    MainWindow.ClipBoardRecord.TypeName = "example info";
                }
                else if ( SelectedItem.DataContext.GetType().Name == "relatedlink" )
                {
                    MainWindow.ClipBoardRecord.ClipBoardItem = SelectedItem.DataContext;
                    MainWindow.ClipBoardRecord.TypeName = "related link info";
                }
                else if ( (String) SelectedItem.Header == "Parameters" )
                {
                    MainWindow.ClipBoardRecord.ClipBoardItem = SelectedItem.DataContext;
                    MainWindow.ClipBoardRecord.TypeName = "parameters description info";
                }
                else if ( (String) SelectedItem.Header == "Examples" )
                {
                    MainWindow.ClipBoardRecord.ClipBoardItem = SelectedItem.DataContext;
                    MainWindow.ClipBoardRecord.TypeName = "examples description info";
                }
                else if ( (String) SelectedItem.Header == "Related Links" )
                {
                    MainWindow.ClipBoardRecord.ClipBoardItem = SelectedItem.DataContext;
                    MainWindow.ClipBoardRecord.TypeName = "related links info";
                }
            }
            else
            {
                MessageBox.Show( "This action is not allowed on the snapin level." );
            }
        }

        /// <summary>
        ///   This is the OnPaste routine. If the action is paste,
        ///   it calls the correct paste function above.
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        public void OnPaste( object sender, RoutedEventArgs e )
        {
            var SelectedItem = (TreeViewItem) NavigationControl1.CmdletTreeView.SelectedItem;

            switch ( MainWindow.ClipBoardRecord.TypeName )
            {
                case "cmdlet description info":
                    if ( SelectedItem.DataContext is cmdletDescription )
                    {
                        var copiedinfo = (cmdletDescription) MainWindow.ClipBoardRecord.ClipBoardItem;
                        var selectedCmdlet = (cmdletDescription) SelectedItem.DataContext;
                        selectedCmdlet.ShortDescription = copiedinfo.ShortDescription;
                        selectedCmdlet.LongDescription = copiedinfo.LongDescription;
                        selectedCmdlet.InputType = copiedinfo.InputType;
                        selectedCmdlet.InputDesc = copiedinfo.InputDesc;
                        selectedCmdlet.OutputType = copiedinfo.OutputType;
                        selectedCmdlet.OutputDesc = copiedinfo.OutputDesc;
                        selectedCmdlet.Note = copiedinfo.Note;
                        SelectedItem.DataContext = selectedCmdlet;
                        doDefaultTreeViewSelectionChange();
                    }
                    else
                    {
                        MessageBox.Show(
                                "You have placed " + MainWindow.ClipBoardRecord.TypeName +
                                " type on the tools clip board.\nMake sure to paste it on a matching node type.",
                                "Error pasting copied record",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning );
                    }
                    break;
                case "parameters description info":
                    if ( SelectedItem.DataContext is Collection<parameterDecription> )
                    {
                        var copiedParameters =
                                (Collection<parameterDecription>) MainWindow.ClipBoardRecord.ClipBoardItem;
                        var selectedParameters = (Collection<parameterDecription>) SelectedItem.DataContext;
                        foreach ( parameterDecription param in selectedParameters )
                        {
                            foreach ( parameterDecription copiedparam in copiedParameters )
                            {
                                if ( param.Name ==
                                     copiedparam.Name )
                                {
                                    copyParameterInfo( param, copiedparam );
                                    break;
                                }
                            }
                        }
                        SelectedItem.DataContext = selectedParameters;
                        doDefaultTreeViewSelectionChange();
                    }
                    else
                    {
                        MessageBox.Show(
                                "You have placed " + MainWindow.ClipBoardRecord.TypeName +
                                " type on the tools clip board.\nMake sure to paste it on a matching node type.",
                                "Error pasting copied record",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning );
                    }

                    break;
                case "parameter decription info":
                    if ( SelectedItem.DataContext is parameterDecription )
                    {
                        var copiedParameter = (parameterDecription) MainWindow.ClipBoardRecord.ClipBoardItem;
                        var selectedPrameter = (parameterDecription) SelectedItem.DataContext;
                        copyParameterInfo( selectedPrameter, copiedParameter );
                        SelectedItem.DataContext = selectedPrameter;
                        doDefaultTreeViewSelectionChange();
                    }
                    else
                    {
                        MessageBox.Show(
                                "You have placed " + MainWindow.ClipBoardRecord.TypeName +
                                " type on the tools clip board.\nMake sure to paste it on a matching node type.",
                                "Error pasting copied record",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning );
                    }
                    break;
                case "example info":
                    if ( SelectedItem.DataContext is Collection<example> ||
                         SelectedItem.DataContext is example )
                    {
                        if ( SelectedItem.DataContext is Collection<example> )
                        {
                            var copiedExample = (example) MainWindow.ClipBoardRecord.ClipBoardItem;
                            var selectedExamples = (Collection<example>) SelectedItem.DataContext;
                            var newExample = new example();
                            copyExample( newExample, copiedExample, selectedExamples.Count );
                            selectedExamples.Add( newExample );
                            SelectedItem.DataContext = selectedExamples;
                        }
                        else
                        {
                            var parentNode = (TreeViewItem) SelectedItem.Parent;
                            var ParentExamples = (Collection<example>) parentNode.DataContext;
                            var copiedExample = (example) MainWindow.ClipBoardRecord.ClipBoardItem;
                            var selectedExample = (example) SelectedItem.DataContext;
                            copyExample( selectedExample, copiedExample, ParentExamples.Count );
                            SelectedItem.DataContext = selectedExample;
                        }
                        doDefaultTreeViewSelectionChange();
                    }
                    else
                    {
                        MessageBox.Show(
                                "You have placed " + MainWindow.ClipBoardRecord.TypeName +
                                " type on the tools clip board.\nMake sure to paste it on a matching node type.",
                                "Error pasting copied record",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning );
                    }
                    break;

                case "examples description info":
                    if ( SelectedItem.DataContext is Collection<example> )
                    {
                        var copiedExamples = (Collection<example>) MainWindow.ClipBoardRecord.ClipBoardItem;
                        var selectedExamples = (Collection<example>) SelectedItem.DataContext;
                        foreach ( example copiedExample in copiedExamples )
                        {
                            var newExample = new example();
                            copyExample( newExample, copiedExample, selectedExamples.Count );
                            selectedExamples.Add( newExample );
                        }
                        SelectedItem.DataContext = selectedExamples;

                        doDefaultTreeViewSelectionChange();
                    }
                    else
                    {
                        MessageBox.Show(
                                "You have placed " + MainWindow.ClipBoardRecord.TypeName +
                                " type on the tools clip board.\nMake sure to paste it on a matching node type.",
                                "Error pasting copied record",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning );
                    }
                    break;
                case "related link info":
                    if ( SelectedItem.DataContext is relatedlink ||
                         SelectedItem.DataContext is Collection<relatedlink> )
                    {
                        if ( SelectedItem.DataContext is Collection<relatedlink> )
                        {
                            var copiedLink = (relatedlink) MainWindow.ClipBoardRecord.ClipBoardItem;
                            var selectedLinks = (Collection<relatedlink>) SelectedItem.DataContext;
                            var newrelatedlink = new relatedlink();
                            copyLink( newrelatedlink, copiedLink, selectedLinks.Count );
                            selectedLinks.Add( newrelatedlink );
                            SelectedItem.DataContext = selectedLinks;
                        }
                        else
                        {
                            var parentNode = (TreeViewItem) SelectedItem.Parent;
                            var ParentLinks = (Collection<relatedlink>) parentNode.DataContext;
                            var copiedLink = (relatedlink) MainWindow.ClipBoardRecord.ClipBoardItem;
                            var selectedLink = (relatedlink) SelectedItem.DataContext;
                            copyLink( selectedLink, copiedLink, ParentLinks.Count );
                            SelectedItem.DataContext = selectedLink;
                        }
                        doDefaultTreeViewSelectionChange();
                    }
                    else
                    {
                        MessageBox.Show(
                                "You have placed " + MainWindow.ClipBoardRecord.TypeName +
                                " type on the tools clip board.\nMake sure to paste it on a matching node type.",
                                "Error pasting copied record",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning );
                    }
                    break;

                case "related links info":
                    if ( SelectedItem.DataContext is Collection<relatedlink> )
                    {
                        var copiedLinks = (Collection<relatedlink>) MainWindow.ClipBoardRecord.ClipBoardItem;
                        var selectedLinks = (Collection<relatedlink>) SelectedItem.DataContext;
                        foreach ( relatedlink copiedLink in copiedLinks )
                        {
                            var newLink = new relatedlink();
                            copyLink( newLink, copiedLink, selectedLinks.Count );
                            selectedLinks.Add( newLink );
                        }
                        SelectedItem.DataContext = selectedLinks;
                        doDefaultTreeViewSelectionChange();
                    }
                    else
                    {
                        MessageBox.Show(
                                "You have placed " + MainWindow.ClipBoardRecord.TypeName +
                                " type on the tools clip board.\nMake sure to paste it on a matching node type.",
                                "Error pasting copied record",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning );
                    }
                    break;
            }
        }

        /// <summary>
        ///   This one pastes one example at a time
        /// </summary>
        /// <param name="selectedExample"> </param>
        /// <param name="copiedExample"> </param>
        /// <param name="count"> </param>
        public void copyExample( example selectedExample, example copiedExample, int count )
        {
            MainWindow.ExamplesControl1.ExampleCommandTextBox.Text = copiedExample.ExampleCmd;
            MainWindow.ExamplesControl1.ExampleDescriptionTextBox.Text = copiedExample.ExampleDescription;
            MainWindow.ExamplesControl1.ExampleOutputTextBox.Text = copiedExample.ExampleOutput;
            MainWindow.ExamplesControl1.ExampleNameTextBox.Text = copiedExample.ExampleName;
            AddExample_Click( null, null );
        }

        /// <summary>
        ///   Paste one related link at a time
        /// </summary>
        /// <param name="selectedLink"> </param>
        /// <param name="copiedLink"> </param>
        /// <param name="count"> </param>
        public void copyLink( relatedlink selectedLink, relatedlink copiedLink, int count )
        {
            MainWindow.RelatedLinks1.RelatedLinkTextBox.Text = copiedLink.LinkText;
            AddRelatedLink_Click( null, null );
        }

        /// <summary>
        ///   This routine handles the Save button on the Related Link page
        ///   It adds the new record to the tree. 
        ///   If this is already in the tree it updates it with the latest 
        ///   info in the page.
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="args"> </param>
        public void AddRelatedLink_Click( object sender, RoutedEventArgs args )
        {
            var SelectedNode = (TreeViewItem) NavigationControl1.CmdletTreeView.SelectedItem;
            if ( SelectedNode != null )
            {
                int count = SelectedNode.Items.Count;
                if ( MainWindow.RelatedLinks1.LinkIDTextBox.Text == "" )
                        //This is a new link
                {
                    //Add new related link info.
                    var ParentNode = (TreeViewItem) SelectedNode.Parent;
                    var Cmdletdesc = (cmdletDescription) ParentNode.DataContext;
                    int LinkCount = 0;
                    if ( Cmdletdesc.RelatedLinks != null )
                    {
                        LinkCount = Cmdletdesc.RelatedLinks.Count;
                    }

                    var linkDetails = new relatedlink();
                    linkDetails.LinkText = MainWindow.RelatedLinks1.RelatedLinkTextBox.Text;
                    linkDetails.LinkID = LinkCount;
                    var NewLinkNode = new TreeViewItem();
                    NewLinkNode.DataContext = linkDetails;
                    NewLinkNode.Header = linkDetails.LinkText;
                    if ( Cmdletdesc.RelatedLinks == null )
                    {
                        var Links = new Collection<relatedlink>();
                        Links.Add( linkDetails );
                        Cmdletdesc.RelatedLinks = Links;
                        SelectedNode.DataContext = Links;
                    }
                    else
                    {
                        Cmdletdesc.RelatedLinks.Add( linkDetails );
                    }
                    MainWindow.ResetLinksPage();

                    if ( (String) SelectedNode.Header == "Related Links" )
                    {
                        if ( sender != null )
                        {
                            SelectedNode.Items.Add( NewLinkNode );
                            SelectedNode.IsExpanded = true;
                            var ChildNode = (TreeViewItem) SelectedNode.Items[count];
                            ChildNode.IsSelected = true;
                        }
                        else
                        {
                            SelectedNode.Items.Add( NewLinkNode );
                            SelectedNode.IsExpanded = true;
                        }
                    }
                }

                else //this is an existing link
                {
                    if ( SelectedNode.Header.ToString() == "Related Links" )
                    {
                        //Update link info.
                        var linkDetails = (Collection<relatedlink>) SelectedNode.DataContext;
                        var linkDetail = new relatedlink();
                        linkDetail.LinkText = MainWindow.RelatedLinks1.RelatedLinkTextBox.Text;
                        linkDetails.Add( linkDetail );
                    }
                    else
                    {
                        var linkDetails = (relatedlink) SelectedNode.DataContext;
                        linkDetails.LinkText = MainWindow.RelatedLinks1.RelatedLinkTextBox.Text;
                        SelectedNode.Header = MainWindow.RelatedLinks1.RelatedLinkTextBox.Text;
                    }
                }
            }
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
            var SelectedNode = (TreeViewItem) NavigationControl1.CmdletTreeView.SelectedItem;
            if ( SelectedNode != null )
            {
                int count = SelectedNode.Items.Count;
                if ( MainWindow.ExamplesControl1.ExampleNameTextBox.Text == "" )
                {
                    MainWindow.ExamplesControl1.ExampleNameTextBox.Text = "Example " + ( count + 1 ).ToString();
                }

                if ( MainWindow.ExamplesControl1.ExampleID.Text == "" )
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
                    examDetails.ExampleCmd = MainWindow.ExamplesControl1.ExampleCommandTextBox.Text;
                    examDetails.ExampleDescription = MainWindow.ExamplesControl1.ExampleDescriptionTextBox.Text;
                    examDetails.ExampleName = MainWindow.ExamplesControl1.ExampleNameTextBox.Text;
                    examDetails.ExampleOutput = MainWindow.ExamplesControl1.ExampleOutputTextBox.Text;
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
                        examDetail.ExampleCmd = MainWindow.ExamplesControl1.ExampleCommandTextBox.Text;
                        examDetail.ExampleDescription = MainWindow.ExamplesControl1.ExampleDescriptionTextBox.Text;
                        examDetail.ExampleName = MainWindow.ExamplesControl1.ExampleNameTextBox.Text;
                        examDetail.ExampleOutput = MainWindow.ExamplesControl1.ExampleOutputTextBox.Text;
                        examDetails.Add( examDetail );
                        //SelectedNode.Header = this.ExampleNameTextBox.Text;
                    }
                    else
                    {
                        var examDetails = (example) SelectedNode.DataContext;
                        examDetails.ExampleCmd = MainWindow.ExamplesControl1.ExampleCommandTextBox.Text;
                        examDetails.ExampleDescription = MainWindow.ExamplesControl1.ExampleDescriptionTextBox.Text;
                        examDetails.ExampleName = MainWindow.ExamplesControl1.ExampleNameTextBox.Text;
                        examDetails.ExampleOutput = MainWindow.ExamplesControl1.ExampleOutputTextBox.Text;
                        SelectedNode.Header = MainWindow.ExamplesControl1.ExampleNameTextBox.Text;
                    }
                }
            }
        }


        /// <summary>
        ///   This is the routine that pastes the Parameter info to the slected cell.
        /// </summary>
        /// <param name="param"> </param>
        /// <param name="copiedparam"> </param>
        public void copyParameterInfo( parameterDecription param, parameterDecription copiedparam )
        {
            param.DefaultValue = copiedparam.DefaultValue;
            param.NewDescription = copiedparam.NewDescription;
            param.Globbing = copiedparam.Globbing;
        }

        /// <summary>
        ///   This routine exports the XML file for one Cmdlet at a time.
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        public void ExportXml_Click( object sender, RoutedEventArgs e )
        {
            var CmdletHelp = new cmdletDescription();

            try
            {
                if ( NavigationControl1.CmdletTreeView.Items.Count > 0 )
                {
                    //Check where the selection is made and find the parent cmdlet name.
                    if ( NavigationControl1.CmdletTreeView.SelectedValue != null )
                    {
                        var SelectedNode = (TreeViewItem) NavigationControl1.CmdletTreeView.SelectedItem;
                        var SelectedValue = (String) SelectedNode.Header;
                        switch ( SelectedValue )
                        {
                            case "Examples":
                                SelectedNode = (TreeViewItem) SelectedNode.Parent;
                                CmdletHelp = (cmdletDescription) SelectedNode.DataContext;
                                break;
                            case "Parameters":
                                SelectedNode = (TreeViewItem) SelectedNode.Parent;
                                CmdletHelp = (cmdletDescription) SelectedNode.DataContext;

                                break;
                            case "Related Links":
                                SelectedNode = (TreeViewItem) SelectedNode.Parent;
                                CmdletHelp = (cmdletDescription) SelectedNode.DataContext;
                                break;
                            case "Decription":
                                SelectedNode = (TreeViewItem) SelectedNode.Parent;
                                CmdletHelp = (cmdletDescription) SelectedNode.DataContext;
                                ;
                                break;
                            default:
                                CmdletHelp = CheckTheType();
                                break;
                        }
                    }
                }
                if ( CmdletHelp.CmdletName != null )
                {
                    XmlWriter writer = null;
                    var settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.IndentChars = ( "    " );
                    settings.NewLineHandling = NewLineHandling.None;

                    settings.ConformanceLevel = ConformanceLevel.Document;


                    try
                    {
                        // Configure open file dialog box
                        var dlg = new SaveFileDialog();
                        //OpenFileDialog dlg = new OpenFileDialog();
                        var SelectedSnapin = new List<SnapinView>();
                        SelectedSnapin = (List<SnapinView>) MainWindow.MainGrid1.PsSnapinList.SelectedItem;

                        dlg.FileName = CmdletHelp.CmdletName + "-Help"; // Default file name
                        dlg.DefaultExt = ".xml"; // Default file extension
                        dlg.Filter = "PowerShell Help Xml files (.xml)|*.xml"; // Filter files by extension

                        // Show open file dialog box
                        DialogResult? result = dlg.ShowDialog();

                        // Process open file dialog box results
                        if ( result.Value ==
                             DialogResult.OK )
                        {
                            // Open document
                            MainWindow.HelpFilePath = dlg.FileName;
                            MainWindow.OldHelpFileExist = true;
                        }
                    }
                    catch ( Exception ex )
                    {
                        MessageBox.Show( ex.Message,
                                         "Error loading the help file.",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Warning );
                    }

                    if ( MainWindow.OldHelpFileExist )
                    {
                        writer = XmlWriter.Create( MainWindow.HelpFilePath, settings );
                        SaveHelpFileBody( writer, CmdletHelp );
                        writer.Flush();
                        writer.Close();
                        MessageBox.Show( MainWindow.HelpFilePath + "\n\nFile saved.",
                                         "File saved",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Information );
                    }
                }
                else
                {
                    MessageBox.Show( "Cannot save contents of the selected node.\nNo file has been saved.",
                                     "No file has been saved.",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Warning );
                }
            }
            catch ( Exception ex )
            {
                MessageBox.Show( ex.Message, "Application error.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
            }
        }

        /// <summary>
        ///   Check the type of the selected item on the tree view.
        ///   This is needed to determine different functionality such
        ///   as copy and paste. It lets me know which type of node on the tree
        ///   is slected.
        /// </summary>
        /// <returns> </returns>
        public cmdletDescription CheckTheType()
        {
            var CmdletHelp = new cmdletDescription();
            var SelectedNode = (TreeViewItem) NavigationControl1.CmdletTreeView.SelectedItem;

            if ( SelectedNode != null )
            {
                if ( SelectedNode.DataContext != null )
                {
                    if ( SelectedNode.DataContext is parameterDecription )
                    {
                        SelectedNode = (TreeViewItem) SelectedNode.Parent;
                        SelectedNode = (TreeViewItem) SelectedNode.Parent;
                        CmdletHelp = (cmdletDescription) SelectedNode.DataContext;
                    }
                    else if ( SelectedNode.DataContext is example )
                    {
                        SelectedNode = (TreeViewItem) SelectedNode.Parent;
                        SelectedNode = (TreeViewItem) SelectedNode.Parent;
                        CmdletHelp = (cmdletDescription) SelectedNode.DataContext;
                    }
                    else if ( SelectedNode.DataContext is relatedlink )
                    {
                        SelectedNode = (TreeViewItem) SelectedNode.Parent;
                        SelectedNode = (TreeViewItem) SelectedNode.Parent;
                        CmdletHelp = (cmdletDescription) SelectedNode.DataContext;
                    }
                    else if ( SelectedNode.DataContext is cmdletDescription )
                    {
                        CmdletHelp = (cmdletDescription) SelectedNode.DataContext;
                    }
                }
            }
            return CmdletHelp;
        }

        /// <summary>
        ///   This routine is used to create the one Cmdlet help MAML
        ///   at a time.
        /// </summary>
        /// <param name="writer"> </param>
        /// <param name="CmdletHelp"> </param>
        public void SaveHelpFileBody( XmlWriter writer, cmdletDescription CmdletHelp )
        {
            try
            {
                writer.WriteRaw(
                        "<command:command xmlns:maml=\"http://schemas.microsoft.com/maml/2004/10\" xmlns:command=\"http://schemas.microsoft.com/maml/dev/command/2004/10\" xmlns:dev=\"http://schemas.microsoft.com/maml/dev/2004/10\">\r\n" );
                writer = MainWindow.helpWriter.writeCmdletDetails( writer, CmdletHelp );

                //Add Syntax section <command:syntax>
                writer.WriteRaw( "	<command:syntax>" );

                Collection<parameterDecription> ParamDescription = CmdletHelp.ParameterDecription;
                Collection<parameterSet> HelpParameterSets = CmdletHelp.ParameterSets;
                //Iterate through the Syntax section and add Syntax items.
                //foreach (parameterset in paramDescription.ParameterSets[0].Parameters[0].Name
                if ( HelpParameterSets != null )
                {
                    foreach ( parameterSet HelpParameterSet in HelpParameterSets )
                    {
                        writer.WriteRaw( "		<command:syntaxItem>\r\n" );
                        //Cdmlet Name.
                        writer.WriteRaw( "			<maml:name>" );
                        writer.WriteRaw( CmdletHelp.CmdletName );
                        writer.WriteRaw( "</maml:name>\r\n" );
                        // writer.WriteRaw("		<!--New Syntax Item-->\r\n");
                        if ( HelpParameterSet.Parameters != null )
                        {
                            foreach ( parametersetParameter HelpParameter in HelpParameterSet.Parameters )
                            {
                                foreach ( parameterDecription param in ParamDescription )
                                {
                                    if ( param.Name.ToLower() ==
                                         HelpParameter.Name.ToLower() )
                                    {
                                        writer = MainWindow.helpWriter.createSyntaxItem( writer, param, CmdletHelp );
                                        break;
                                    }
                                }
                            }
                        }
                        //End <command:syntaxItem>
                        writer.WriteRaw( "		</command:syntaxItem>\r\n" );
                    }
                }
                else
                {
                    writer.WriteRaw( "		<command:syntaxItem>\r\n" );
                    writer.WriteRaw( "		</command:syntaxItem>\r\n" );
                }


                writer.WriteRaw( "	</command:syntax>\r\n" );
                //writer.WriteRaw("	</command:syntax>\r\n");

                //Add Parameters section <command:parameters>
                writer.WriteRaw( "	<command:parameters>\r\n" );
                //writer.WriteComment("Parameters section");

                //Iterate through the parameters section and add parameters Items.


                foreach ( parameterDecription parameter in ParamDescription )
                {
                    writer = MainWindow.helpWriter.createParameters( writer, parameter );
                }


                writer.WriteRaw( "	</command:parameters>\r\n" );


                //Input Object Section
                writer = MainWindow.helpWriter.createInputSection( writer, CmdletHelp );

                //Output Object Section
                writer = MainWindow.helpWriter.createOutputSection( writer, CmdletHelp );


                // Error Section (Static section not used)
                //<command:terminatingErrors />
                //<command:nonTerminatingErrors />
                writer.WriteRaw( "	<command:terminatingErrors>\r\n" );
                //writer.WriteComment("Terminating errors section");
                writer.WriteRaw( "	</command:terminatingErrors>\r\n" );
                writer.WriteRaw( "	<command:nonTerminatingErrors>\r\n" );
                //writer.WriteComment("Non terminating errors section");
                writer.WriteRaw( "	</command:nonTerminatingErrors>\r\n" );


                //AlertSet  <!-- Notes section  -->
                writer = MainWindow.helpWriter.createAlertSetSection( writer, CmdletHelp );

                //Examples section.
                //Examples header goes here <command:examples>
                writer.WriteRaw( "	<command:examples>\r\n" );

                //Iterate through all the examples here
                if ( CmdletHelp.Examples != null )
                {
                    foreach ( example ExampleRecord in CmdletHelp.Examples )
                    {
                        writer = MainWindow.helpWriter.createExampleItemSection( writer, ExampleRecord );
                        //End examples section
                    }
                }
                writer.WriteRaw( "	</command:examples>\r\n" );


                //Links section
                // <maml:relatedLinks>
                writer.WriteRaw( "	<maml:relatedLinks>\r\n" );
                //Iterate through the links

                if ( CmdletHelp.RelatedLinks != null )
                {
                    foreach ( relatedlink RelatedLinkRecord in CmdletHelp.RelatedLinks )
                    {
                        writer = MainWindow.helpWriter.createLinksSection( writer, RelatedLinkRecord );
                    }
                }
                writer.WriteRaw( "	</maml:relatedLinks>\r\n" );

                //Write the end node for the starting <command:command> node.
                //writer.WriteRaw();
                writer.WriteRaw( "</command:command>\r\n" );
                //  }

                //Write the help file.

                //  }
            }
            catch ( Exception ex )
            {
                MessageBox.Show( ex.Message, "Error writing the XML file", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                //FailedToWrite = true;
                if ( writer != null )
                {
                    writer.Close();
                }
            }
        }


        /// <summary>
        ///   Write the parameter in the text viewer.
        /// </summary>
        /// <param name="param"> </param>
        /// <returns> </returns>
        public String writeParameter( parameterDecription param )
        {
            String textStream = "";
            textStream = "    -" + param.Name + " <" + param.ParameterType + ">\r\n";
            textStream += "        " + param.NewDescription + "\r\n\r\n";
            textStream += "        Required?                    ";
            if ( param.isMandatory )
            {
                textStream += "true\r\n";
            }
            else
            {
                textStream += "false\r\n";
            }
            textStream += "        Position?                    " + param.Position + "\r\n";
            textStream += "        Default value                " + param.DefaultValue + "\r\n";
            String pipelineInput;
            if ( param.VFP ||
                 param.VFPBPN )
            {
                pipelineInput = "true (";
                if ( param.VFP )
                {
                    pipelineInput += "ByValue";
                }
                if ( param.VFPBPN )
                {
                    if ( pipelineInput.Length > 6 )
                    {
                        pipelineInput += ", ByPropertyName)";
                    }
                    else
                    {
                        pipelineInput += "ByPropertyName)";
                    }
                }
                else
                {
                    pipelineInput += ")";
                }
            }
            else
            {
                pipelineInput = "false";
            }
            textStream += "        Accept pipeline input?       " + pipelineInput + "\r\n";
            if ( param.Globbing )
            {
                textStream += "        Accept wildcard characters?  true\r\n\r\n";
            }
            else
            {
                textStream += "        Accept wildcard characters?  false\r\n\r\n";
            }

            return textStream;
        }

        /// <summary>
        ///   Create the text viewer of the selected Cmdlet.
        ///   This is a simple function to help me create a txt view
        ///   of the get-help functionality on the cmdlet.
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        public void ExportTextFile_Click( object sender, RoutedEventArgs e )
        {
            var CmdletHelp = new cmdletDescription();

            try
            {
                if ( NavigationControl1.CmdletTreeView.Items.Count > 0 )
                {
                    if ( NavigationControl1.CmdletTreeView.SelectedValue != null )
                    {
                        var SelectedNode = (TreeViewItem) NavigationControl1.CmdletTreeView.SelectedItem;
                        var SelectedValue = (String) SelectedNode.Header;
                        switch ( SelectedValue )
                        {
                            case "Examples":
                                SelectedNode = (TreeViewItem) SelectedNode.Parent;
                                CmdletHelp = (cmdletDescription) SelectedNode.DataContext;
                                break;
                            case "Parameters":
                                SelectedNode = (TreeViewItem) SelectedNode.Parent;
                                CmdletHelp = (cmdletDescription) SelectedNode.DataContext;
                                break;
                            case "Related Links":
                                SelectedNode = (TreeViewItem) SelectedNode.Parent;
                                CmdletHelp = (cmdletDescription) SelectedNode.DataContext;
                                break;
                            case "Decription":
                                SelectedNode = (TreeViewItem) SelectedNode.Parent;
                                CmdletHelp = (cmdletDescription) SelectedNode.DataContext;
                                ;
                                break;
                            default:
                                CmdletHelp = CheckTheType();
                                break;
                        }
                    }
                }
                if ( CmdletHelp.CmdletName != null )
                {
                    String textStream = "";

                    try
                    {
                        textStream += "NAME\r\n" + CmdletHelp.CmdletName + "\r\n\r\n";
                        textStream += "SYNOPSIS\r\n" + CmdletHelp.ShortDescription + "\r\n\r\n";
                        textStream += "SYNTAX\r\n";
                        textStream += writeSyntax( CmdletHelp ) + "\r\n";
                        textStream += "DETAILED DESCRIPTION\r\n" + CmdletHelp.LongDescription + "\r\n\r\n";
                        textStream += "PARAMETERS\r\n";
                        String paramStream = "";
                        foreach ( parameterDecription param in CmdletHelp.ParameterDecription )
                        {
                            paramStream = writeParameter( param );
                            textStream += paramStream;
                        }
                        textStream += "INPUT TYPE\r\n" + CmdletHelp.InputType + "\r\n\r\n";
                        textStream += "INPUT TYPE DESCRIPTION\r\n" + CmdletHelp.InputDesc + "\r\n\r\n";
                        textStream += "RETURN TYPE\r\n" + CmdletHelp.OutputType + "\r\n\r\n";
                        textStream += "RETURN TYPE DESCRIPTION\r\n" + CmdletHelp.OutputDesc + "\r\n\r\n";
                        textStream += "NOTES\r\n" + CmdletHelp.Note + "\r\n\r\n";
                        textStream += "EXAMPLES\r\n\r\n";
                        if ( CmdletHelp.Examples != null )
                        {
                            foreach ( example examp in CmdletHelp.Examples )
                            {
                                String exampleStream = writeExample( examp );
                                textStream += exampleStream;
                            }
                        }
                        textStream += "RELATED LINKS\r\n";
                        if ( CmdletHelp.RelatedLinks != null )
                        {
                            foreach ( relatedlink link in CmdletHelp.RelatedLinks )
                            {
                                textStream += "    " + link.LinkText + "\r\n";
                            }
                        }

                        var SaveDialog = new SaveFileDialog();
                        SaveDialog.DefaultExt = "txt";
                        SaveDialog.FileName = CmdletHelp.CmdletName;
                        SaveDialog.Filter = "Text Files (*.txt)|*.txt";
                        SaveDialog.Title = CmdletHelp.CmdletName;
                        DialogResult result = SaveDialog.ShowDialog();

                        if ( result == DialogResult.OK )
                        {
                            string FileName = SaveDialog.FileName;


                            using ( var sw = new StreamWriter( FileName ) )
                            {
                                // Add some text to the file.
                                //sw.AutoFlush;
                                sw.Write( textStream );
                                sw.Close();

                                // Loading the CS fileName Created in Notepad.
                                var newProcess = new Process();
                                newProcess.StartInfo.FileName = "notepad.exe";
                                newProcess.StartInfo.Arguments = FileName;
                                newProcess.Start();
                            }
                        }
                    }
                    catch ( Exception ex )
                    {
                        MessageBox.Show( ex.Message,
                                         "Error loading the help file.",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Warning );
                    }
                }
                else
                {
                    MessageBox.Show( "No valid cmdlet name has been selected.\nPlease select the correct Cmdlet name.",
                                     "Not a valid selection.",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Warning );
                }
            }
            catch ( Exception ex )
            {
                MessageBox.Show( ex.Message, "Application error.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
            }
        }


        /// <summary>
        ///   Write the Cmdlet Parameter Sets syntax.
        /// </summary>
        /// <param name="CmdletHelp"> </param>
        /// <returns> </returns>
        public String writeSyntax( cmdletDescription CmdletHelp )
        {
            String Syntax = "";
            if ( CmdletHelp.ParameterSets != null )
            {
                foreach ( parameterSet parSet in CmdletHelp.ParameterSets )
                {
                    Syntax += "    " + CmdletHelp.CmdletName + " ";
                    foreach ( parametersetParameter ParameterName in parSet.Parameters )
                    {
                        foreach ( parameterDecription param in CmdletHelp.ParameterDecription )
                        {
                            if ( param.Name ==
                                 ParameterName.Name )
                            {
                                if ( param.isMandatory &&
                                     param.Position.ToLower() != "named" )
                                {
                                    Syntax += "[-" + param.Name + "] <" + param.ParameterType + "> ";
                                }
                                else if ( param.isMandatory &&
                                          param.Position.ToLower() == "named" )
                                {
                                    Syntax += "-" + param.Name + " <" + param.ParameterType + "> ";
                                }
                                else if ( !param.isMandatory &&
                                          param.Position.ToLower() != "named" )
                                {
                                    Syntax += "[[-" + param.Name + "] <" + param.ParameterType + ">] ";
                                }
                                else if ( !param.isMandatory &&
                                          param.Position == "named" )
                                {
                                    Syntax += "[-" + param.Name + " <" + param.ParameterType + ">] ";
                                }
                            }
                        }
                    }
                    Syntax += "\r\n";
                }
            }
            else
            {
                foreach ( parameterDecription param in CmdletHelp.ParameterDecription )
                {
                    if ( param.isMandatory &&
                         param.Position.ToLower() != "named" )
                    {
                        Syntax += "[-" + param.Name + "] <" + param.ParameterType + "> ";
                    }
                    else if ( param.isMandatory &&
                              param.Position.ToLower() == "named" )
                    {
                        Syntax += "-" + param.Name + " <" + param.ParameterType + "> ";
                    }
                    else if ( !param.isMandatory &&
                              param.Position.ToLower() != "named" )
                    {
                        Syntax += "[[-" + param.Name + "] <" + param.ParameterType + ">] ";
                    }
                    else if ( !param.isMandatory &&
                              param.Position == "named" )
                    {
                        Syntax += "[-" + param.Name + " <" + param.ParameterType + ">] ";
                    }
                }
            }


            return Syntax;
        }


        /// <summary>
        ///   Write the Example section in the text viewer.
        /// </summary>
        /// <param name="examp"> </param>
        /// <returns> </returns>
        public String writeExample( example examp )
        {
            String ExampleStream;
            ExampleStream = "    -------------------------- " + examp.ExampleName +
                            " --------------------------\r\n\r\n";
            ExampleStream += "    C:\\PS>" + examp.ExampleCmd + "\r\n\r\n";
            ExampleStream += "    " + examp.ExampleDescription + "\r\n\r\n";
            ExampleStream += "    " + examp.ExampleOutput + "\r\n\r\n";

            return ExampleStream;
        }


        /// <summary>
        ///   We call this routine everytime a selection is handled in the tree.
        ///   Load the correct text in the UI from the Data context on the selected Tree item
        ///   We show and hide the corresponding grids as well.
        /// </summary>
        public void doDefaultTreeViewSelectionChange()
        {
            // Do Tree View selection code here.            
            var SelectedNode = (TreeViewItem) MainWindow.NavControl.CmdletTreeView.SelectedItem;

            if ( SelectedNode != null )
            {
                if ( SelectedNode.DataContext != null )
                {
                    // This means we have selected a Parameter record
                    if ( SelectedNode.DataContext is parameterDecription )
                    {
                        // Clear the parameters page and show it.
                        MainWindow.resetParameterPage();
                        MainWindow.ParametersControl1.Visibility = Visibility.Visible;
                        MainWindow.DescriptionControl1.Visibility = Visibility.Collapsed;
                        MainWindow.EmptyParameterControl1.Visibility = Visibility.Collapsed;
                        //This is a parameter mother node
                        var param = SelectedNode.DataContext as parameterDecription;
                        MainWindow.ParametersControl1.ParameterNameTextBox.Text = param.Name;
                        MainWindow.ParametersControl1.DynamicCheckBox.IsChecked = false;
                        // Populate the data appropriatly. 
                        MainWindow.ParametersControl1.ParameterNameTextBox.Text = param.Name;
                        if ( param.VFRA )
                        {
                            MainWindow.ParametersControl1.ValueFromRemainingCheckBox.IsChecked = true;
                        }

                        // if we are in the online mode, we do set the lables to bold red in the parameter
                        // description page.
                        if ( MainWindow.ProjectName != null &&
                             MainWindow.ProjectName != "" )
                        {
                            if ( param.SpecVFRA !=
                                 param.VFRA )
                            {
                                MainWindow.ParametersControl1.ValueFromRemainingCheckBox.Foreground = Brushes.Red;
                                MainWindow.ParametersControl1.ValueFromRemainingCheckBox.FontWeight = FontWeights.Bold;
                            }
                            else
                            {
                                MainWindow.ParametersControl1.ValueFromRemainingCheckBox.Foreground = Brushes.White;
                                MainWindow.ParametersControl1.ValueFromRemainingCheckBox.FontWeight = FontWeights.Normal;
                            }
                        }
                        if ( param.VFPBPN )
                        {
                            MainWindow.ParametersControl1.VFPBPN_CheckBox.IsChecked = true;
                        }
                        if ( MainWindow.ProjectName != null &&
                             MainWindow.ProjectName != "" )
                        {
                            if ( param.SpecVFPBPN !=
                                 param.VFPBPN )
                            {
                                MainWindow.ParametersControl1.VFPBPN_CheckBox.Foreground = Brushes.Red;
                                MainWindow.ParametersControl1.VFPBPN_CheckBox.FontWeight = FontWeights.Bold;
                            }
                            else
                            {
                                MainWindow.ParametersControl1.VFPBPN_CheckBox.Foreground = Brushes.White;
                                MainWindow.ParametersControl1.VFPBPN_CheckBox.FontWeight = FontWeights.Normal;
                            }
                        }
                        if ( param.VFP )
                        {
                            MainWindow.ParametersControl1.VFP_CheckBox.IsChecked = true;
                        }
                        if ( MainWindow.ProjectName != null &&
                             MainWindow.ProjectName != "" )
                        {
                            if ( param.SpecVFP !=
                                 param.VFP )
                            {
                                MainWindow.ParametersControl1.VFP_CheckBox.Foreground = Brushes.Red;
                                MainWindow.ParametersControl1.VFP_CheckBox.FontWeight = FontWeights.Bold;
                            }
                            else
                            {
                                MainWindow.ParametersControl1.VFP_CheckBox.Foreground = Brushes.White;
                                MainWindow.ParametersControl1.VFP_CheckBox.FontWeight = FontWeights.Normal;
                            }
                        }
                        if ( param.isMandatory )
                        {
                            MainWindow.ParametersControl1.MandatoryCheckBox.IsChecked = true;
                        }
                        if ( MainWindow.ProjectName != null &&
                             MainWindow.ProjectName != "" )
                        {
                            if ( param.SpecisMandatory !=
                                 param.isMandatory )
                            {
                                MainWindow.ParametersControl1.MandatoryCheckBox.Foreground = Brushes.Red;
                                MainWindow.ParametersControl1.MandatoryCheckBox.FontWeight = FontWeights.Bold;
                            }
                            else
                            {
                                MainWindow.ParametersControl1.MandatoryCheckBox.Foreground = Brushes.White;
                                MainWindow.ParametersControl1.MandatoryCheckBox.FontWeight = FontWeights.Normal;
                            }
                        }

                        if ( param.isDynamic )
                        {
                            MainWindow.ParametersControl1.DynamicCheckBox.IsChecked = true;
                        }
                        if ( MainWindow.ProjectName != null &&
                             MainWindow.ProjectName != "" )
                        {
                            if ( param.SpecisDynamic !=
                                 param.isDynamic )
                            {
                                MainWindow.ParametersControl1.DynamicCheckBox.Foreground = Brushes.Red;
                                MainWindow.ParametersControl1.DynamicCheckBox.FontWeight = FontWeights.Bold;
                            }
                            else
                            {
                                MainWindow.ParametersControl1.DynamicCheckBox.Foreground = Brushes.White;
                                MainWindow.ParametersControl1.DynamicCheckBox.FontWeight = FontWeights.Normal;
                            }
                        }
                        if ( param.Globbing )
                        {
                            MainWindow.ParametersControl1.GlobbingCheckBox.IsChecked = true;
                        }

                        if ( param.OldGlobbing )
                        {
                            MainWindow.ParametersControl1.OldGlobbingCheckBox.IsChecked = true;
                        }

                        if ( param.Globbing !=
                             param.OldGlobbing )
                        {
                            MainWindow.ParametersControl1.OldGlobbingCheckBox.Foreground = Brushes.Red;
                            MainWindow.ParametersControl1.OldGlobbingCheckBox.FontWeight = FontWeights.Bold;
                        }
                        else
                        {
                            MainWindow.ParametersControl1.OldGlobbingCheckBox.Foreground = Brushes.White;
                            MainWindow.ParametersControl1.OldGlobbingCheckBox.FontWeight = FontWeights.Normal;
                        }
                        MainWindow.ParametersControl1.PositionTextBox.Text = param.Position;

                        MainWindow.ParametersControl1.ParameterDescTextBox.Text = param.NewDescription;
                        MainWindow.ParametersControl1.OldParameterDescTextBox.Text = param.OldDescription;
                        MainWindow.ParametersControl1.DefaultValueTextBox.Text = param.DefaultValue;
                        MainWindow.ParametersControl1.OldParamDefaultValueTextBox.Text = param.OldDefaultValue;
                        MainWindow.ParametersControl1.ParameterTypeTextBox.Text = param.ParameterType;
                        if ( MainWindow.ProjectName != null &&
                             MainWindow.ProjectName != "" )
                        {
                            if ( param.ParameterType !=
                                 param.SpecParameterType )
                            {
                                MainWindow.ParametersControl1.ParameterTypeLabel.Foreground = Brushes.Red;
                                MainWindow.ParametersControl1.ParameterTypeLabel.FontWeight = FontWeights.Bold;
                            }
                            else
                            {
                                MainWindow.ParametersControl1.ParameterTypeLabel.Foreground = Brushes.White;
                                MainWindow.ParametersControl1.ParameterTypeLabel.FontWeight = FontWeights.Normal;
                            }
                        }


                        if ( param.Attributes != null )
                        {
                            foreach ( parameterAttribute ParameterAttribute in param.Attributes )
                            {
                                MainWindow.ParametersControl1.AttributesList.Items.Add( ParameterAttribute.Attribute );
                            }
                        }

                        if ( param.Aliases != null )
                        {
                            foreach ( parameterAlias ParameterAlias in param.Aliases )
                            {
                                MainWindow.ParametersControl1.AliasesList.Items.Add( ParameterAlias.Alias );
                            }
                        }
                    }
                            // We populate the examples record in this section
                    else if ( SelectedNode.DataContext is example )
                    {
                        var exampledetails = SelectedNode.DataContext as example;
                        MainWindow.ExamplesControl1.ExampleNameTextBox.Text = exampledetails.ExampleName;
                        MainWindow.ExamplesControl1.OldExampleNameTextBox.Text = exampledetails.OldExampleName;
                        MainWindow.ExamplesControl1.OldExampleCommandTextBox.Text = exampledetails.OldExampleCmd;
                        MainWindow.ExamplesControl1.ExampleOutputTextBox.Text = exampledetails.ExampleOutput;
                        MainWindow.ExamplesControl1.OldExampleOutputTextBox.Text = exampledetails.OldExampleOutput;
                        MainWindow.ExamplesControl1.ExampleDescriptionTextBox.Text = exampledetails.ExampleDescription;
                        MainWindow.ExamplesControl1.OldExampleDescTextBox.Text = exampledetails.OldExampleDescription;
                        MainWindow.ExamplesControl1.ExampleCommandTextBox.Text = exampledetails.ExampleCmd;
                        MainWindow.ExamplesControl1.ExampleID.Text = exampledetails.ExampleID.ToString();
                        MainWindow.ParametersControl1.Visibility = Visibility.Collapsed;
                        MainWindow.DescriptionControl1.Visibility = Visibility.Collapsed;
                        MainWindow.ExamplesControl1.Visibility = Visibility.Visible;
                        MainWindow.RelatedLinks1.Visibility = Visibility.Collapsed;
                        MainWindow.EmptyParameterControl1.Visibility = Visibility.Collapsed;
                        MainWindow.ExamplesControl1.OldExampleDescTextBox.Text = exampledetails.OldExampleDescription;
                    }
                            // We populate the Links page with the link info.
                    else if ( SelectedNode.DataContext is relatedlink )
                    {
                        var linkdetails = SelectedNode.DataContext as relatedlink;
                        MainWindow.RelatedLinks1.RelatedLinkTextBox.Text = linkdetails.LinkText;
                        MainWindow.RelatedLinks1.OldRelatedLinkTextBox.Text = linkdetails.OldLinkText;
                        MainWindow.RelatedLinks1.LinkIDTextBox.Text = linkdetails.LinkID.ToString();
                        MainWindow.ParametersControl1.Visibility = Visibility.Collapsed;
                        MainWindow.DescriptionControl1.Visibility = Visibility.Collapsed;
                        MainWindow.ExamplesControl1.Visibility = Visibility.Collapsed;
                        MainWindow.RelatedLinks1.Visibility = Visibility.Visible;
                        MainWindow.EmptyParameterControl1.Visibility = Visibility.Collapsed;
                    }
                            // Populate the Cmdlet main description page.
                    else if ( SelectedNode.DataContext is cmdletDescription )
                    {
                        MainWindow.resetDescriptionPage();
                        var desc = SelectedNode.DataContext as cmdletDescription;
                        MainWindow.ParametersControl1.Visibility = Visibility.Collapsed;
                        MainWindow.DescriptionControl1.Visibility = Visibility.Visible;
                        MainWindow.EmptyParameterControl1.Visibility = Visibility.Collapsed;
                        MainWindow.DescriptionControl1.ShortDescriptionTextBox.Text = desc.ShortDescription;
                        MainWindow.DescriptionControl1.OldShortDescTextBox.Text = desc.OldShortDescription;
                        MainWindow.DescriptionControl1.DetailedDescriptionTextBox.Text = desc.LongDescription;
                        MainWindow.DescriptionControl1.OldDetailedDescriptionTextBox.Text = desc.OldLongDescription;
                        MainWindow.DescriptionControl1.InputTypeDescTextBox.Text = desc.InputDesc;
                        MainWindow.DescriptionControl1.OldInputTypeDescTextBox.Text = desc.OldInputDesc;
                        MainWindow.DescriptionControl1.InputTypeTextBox.Text = desc.InputType;
                        MainWindow.DescriptionControl1.OldInputTypeTextBox.Text = desc.OldInputType;
                        MainWindow.DescriptionControl1.OutputTypeDescTextBox.Text = desc.OutputDesc;
                        MainWindow.DescriptionControl1.OldOutputTypeDescTextBox.Text = desc.OldOutputDesc;
                        MainWindow.DescriptionControl1.OldTypeTextBox.Text = desc.OldOutputType;
                        MainWindow.DescriptionControl1.OutpuTypeTextBox.Text = desc.OutputType;
                        MainWindow.DescriptionControl1.NotesDescriptionTextBox.Text = desc.Note;
                        MainWindow.DescriptionControl1.OldNotesTextBox.Text = desc.OldNote;
                    }
                }
                else
                {
                    MainWindow.DescriptionControl1.Visibility = Visibility.Collapsed;
                    MainWindow.EmptyParameterControl1.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                MainWindow.DescriptionControl1.Visibility = Visibility.Collapsed;
                MainWindow.EmptyParameterControl1.Visibility = Visibility.Collapsed;
            }
        }
    }
}
