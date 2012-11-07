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
    ///   Interaction logic for RelatedLinks.xaml
    /// </summary>
    public partial class RelatedLinks : UserControl
    {
        public RelatedLinks()
        {
            InitializeComponent();
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
            var SelectedNode = (TreeViewItem) MainWindow.NavControl.CmdletTreeView.SelectedItem;
            if ( SelectedNode != null )
            {
                int count = SelectedNode.Items.Count;
                if ( RelatedLinks1.LinkIDTextBox.Text == "" )
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
                    linkDetails.LinkText = RelatedLinks1.RelatedLinkTextBox.Text;
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
                        linkDetail.LinkText = RelatedLinks1.RelatedLinkTextBox.Text;
                        linkDetails.Add( linkDetail );
                    }
                    else
                    {
                        var linkDetails = (relatedlink) SelectedNode.DataContext;
                        linkDetails.LinkText = RelatedLinks1.RelatedLinkTextBox.Text;
                        SelectedNode.Header = RelatedLinks1.RelatedLinkTextBox.Text;
                    }
                }
            }
        }

        public void NewRelatedLink_Click( object sender, RoutedEventArgs e )
        {
            var ChildNode = (TreeViewItem) MainWindow.NavControl.CmdletTreeView.SelectedItem;
            var Header = (String) ChildNode.Header;
            if ( Header.ToLower() != "related links" )
            {
                ChildNode = (TreeViewItem) ChildNode.Parent;
                ChildNode.IsSelected = true;
            }
            MainWindow.ResetLinksPage();
        }

        /// <summary>
        ///   This routine removes a related link from the tree.
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="args"> </param>
        public void DeleteLink_Click( object sender, RoutedEventArgs args )
        {
            if ( RelatedLinks1.LinkIDTextBox.Text == "" )
            {
                //Safe to simply reset the Examples page.
                MainWindow.ResetLinksPage();
            }
            else //We must remove the current record from the Examples list
            {
                var selectedNode = (TreeViewItem) MainWindow.NavControl.CmdletTreeView.SelectedItem;
                var ParentNode = (TreeViewItem) selectedNode.Parent;
                var CmdletNode = (TreeViewItem) ParentNode.Parent;
                ParentNode.IsSelected = true;
                ParentNode.Items.Remove( selectedNode );

                var selectedCmdlet = (cmdletDescription) CmdletNode.DataContext;
                var selectedLink = (relatedlink) selectedNode.DataContext;

                foreach ( cmdletDescription mycmdlet in MainWindow.CmdletsHelps )
                {
                    if ( mycmdlet.CmdletName ==
                         selectedCmdlet.CmdletName )
                    {
                        Collection<relatedlink> myLinks = mycmdlet.RelatedLinks;
                        var linktoRemove = new relatedlink();
                        foreach ( relatedlink mylink in myLinks )
                        {
                            if ( mylink.LinkID ==
                                 selectedLink.LinkID )
                            {
                                linktoRemove = mylink;
                            }
                        }
                        if ( linktoRemove.LinkID.ToString() != null )
                        {
                            mycmdlet.RelatedLinks.Remove( linktoRemove );
                            ParentNode.Items.Remove( selectedNode );
                        }
                    }
                }
            }
        }
    }
}
