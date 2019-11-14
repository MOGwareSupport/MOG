using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MOG_ControlsLibrary.Controls
{
	/// <summary>
	/// Summary description for MogUtil_ClassificationTrees.
	/// </summary>
	public class MogUtil_ClassificationTrees
	{
		private const string BlankNode = "BLANK";
		static private string mLastNodePath = "";

		/// <summary>
		/// Creates OR finds a node, given it's full path.  Not for use with any Tree that needs Mog_BaseTags for 
		/// ContextMenu use.
		/// 
		/// NOTE:  Pass in the Tree xOR the TreeNode to expand.  If one is passed in, the other should be null.
		/// </summary>
		/// <param name="tree">Tree to expand</param>
		/// <param name="parent">TreeNode to expand</param>
		/// <param name="delimiter">Delimiter of tree we are expanding</param>
		/// <param name="fullPath">Fullpath to node</param>
		/// <returns>TreeNode found xOR created by this process</returns>
		static public TreeNode CreateAndExpandTreeNodeFullPath( TreeView tree, TreeNode parent, string delimiter, string fullPath)
		{
			// Store the values of our TreeNodeCollection
			TreeNodeCollection topNodes = null;
			// Decide which set of Nodes to user for our TreeNodeCollection...
			if (parent != null)
			{
				topNodes = parent.Nodes;
			}
			else
			{
				topNodes = tree.Nodes;
			}

			// Split the full path by the delimiter passed in
			string[] lastNodeParts = fullPath.Split( delimiter.ToCharArray() );
			
			// Make sure the split gave us a valid top node
			if (lastNodeParts[0].Length == 0 &&
				lastNodeParts.Length > 1)
			{
				lastNodeParts[0] = lastNodeParts[1];
			}

			// find the first name
			TreeNode alreadyExistNode = FindTreeNode(topNodes, lastNodeParts[0]);

			// if exist find then next from the children of the first
			if (alreadyExistNode != null)
			{
				// Update our path to be less than what it was
				string LastNodePath = fullPath.Replace(lastNodeParts[0], "");
				LastNodePath = LastNodePath.TrimStart(delimiter.ToCharArray());

				if (LastNodePath.Length > 0)
				{
					// recurse into this function for the next node
					parent = CreateAndExpandTreeNodeFullPath(tree, alreadyExistNode, delimiter, LastNodePath);
					return parent;
				}
			}
				// if not, create it and all its children right here
			else
			{
				foreach (string nodeLeafName in lastNodeParts)
				{
					if (nodeLeafName.Length > 0)
					{
						// Create a node
						TreeNode newChild = new TreeNode(nodeLeafName);
						newChild.Checked = true;
						
						if (parent != null)
						{
							parent.Nodes.Add(newChild);
							parent = newChild;
						}
						else
						{
							int index = tree.Nodes.Add(newChild);
							parent = tree.Nodes[index];
						}
					}
				}

				return parent;
			}

			return null;
		}

		/// <summary>
		/// Creates OR finds a node, given it's full path.  Not for use with any Tree that needs Mog_BaseTags for 
		/// ContextMenu use.
		/// 
		/// NOTE:  Pass in the Tree xOR the TreeNode to expand.  If one is passed in, the other should be null.
		/// </summary>
		/// <param name="tree">Tree to expand</param>
		/// <param name="parent">TreeNode to expand</param>
		/// <param name="delimiter">Delimiter of tree we are expanding</param>
		/// <param name="fullPath">Fullpath to node</param>
		/// <returns>TreeNode found xOR created by this process</returns>
		static public TreeNode CreateTreeNodeFullPath( TreeView tree, TreeNode parent, string delimiter, string fullPath)
		{
			// Store the values of our TreeNodeCollection
			TreeNodeCollection topNodes = null;
			// Decide which set of Nodes to user for our TreeNodeCollection...
			if (parent != null)
			{
				topNodes = parent.Nodes;
			}
			else
			{
				topNodes = tree.Nodes;
			}

			// Split the full path by the delimiter passed in
			string[] lastNodeParts = fullPath.Split( delimiter.ToCharArray() );
			
			// Make sure the split gave us a valid top node
			if (lastNodeParts[0].Length == 0)
			{
				lastNodeParts[0] = lastNodeParts[1];
			}

			// find the first name
			TreeNode alreadyExistNode = FindTreeNode(topNodes, lastNodeParts[0]);

			// if exist find then next from the children of the first
			if (alreadyExistNode != null)
			{
				// Update our path to be less than what it was
				string LastNodePath = fullPath.Replace(lastNodeParts[0], "");
				LastNodePath = LastNodePath.TrimStart(delimiter.ToCharArray());

				if (LastNodePath.Length > 0)
				{
					if (alreadyExistNode.Nodes.Count == 1 && alreadyExistNode.Nodes[0].Text == BlankNode)
					{
						return parent;
					}
						// Create a child place holder
						alreadyExistNode.Nodes.Add(BlankNode);

						// recurse into this function for the next node
						//parent = CreateAndExpandTreeNodeFullPath(tree, alreadyExistNode, delimiter, LastNodePath);					
				}
			}
				// if not, create it and all its children right here
			else
			{
				foreach (string nodeLeafName in lastNodeParts)
				{
					if (nodeLeafName.Length > 0)
					{
						// Create a node
						TreeNode newChild = new TreeNode(nodeLeafName);
						
						if (parent != null)
						{
							parent.Nodes.Add(newChild);
							parent = newChild;
						}
						else
						{
							int index = tree.Nodes.Add(newChild);
							parent = tree.Nodes[index];
						}
					}
				}

				return parent;
			}

			return null;
		}

		static public TreeNode CreateTreeNodeFullPath( TreeView tree, TreeNode parent, string delimiter, string fullPath, string verifyFilename,
			int VirtualImageIndex, int NonVirtualImageIndex, int VirtualFileImageIndex, int NonVirtualFileImageIndex)
		{
			TreeNodeCollection topNodes = null;
			
			if (parent != null)
			{
				topNodes = parent.Nodes;
			}
			else
			{
				topNodes = tree.Nodes;
			}

			// Split the full path by the delimiter passed in
			string[] lastNodeParts = fullPath.Split( delimiter.ToCharArray() );
			
			// find the first name
			TreeNode alreadyExistNode = null;
			if (string.Compare(parent.Text, lastNodeParts[0], true) == 0)
			{
				alreadyExistNode = parent;
			}
			else
			{
				alreadyExistNode = FindTreeNode(topNodes, lastNodeParts[0]);
			}

			// if exist find then next from the children of the first
			if (alreadyExistNode != null)
			{
				// Is this a file or directory
				if (Path.GetDirectoryName(verifyFilename).IndexOf(lastNodeParts[0]) != -1)
				{
					// Must be a directory
					if (Directory.Exists(Path.GetDirectoryName(verifyFilename)))
					{
						alreadyExistNode.ImageIndex = NonVirtualImageIndex;
						alreadyExistNode.SelectedImageIndex = NonVirtualImageIndex;
						alreadyExistNode.ForeColor = SystemColors.ControlText;
					}
					else
					{
						alreadyExistNode.ImageIndex = VirtualImageIndex;
						alreadyExistNode.SelectedImageIndex = VirtualImageIndex;
						alreadyExistNode.ForeColor = SystemColors.GrayText;
					}
				} 
				else
				{
					// Must be a file
					if (File.Exists(verifyFilename))
					{
						alreadyExistNode.ImageIndex = NonVirtualFileImageIndex;
						alreadyExistNode.SelectedImageIndex = NonVirtualFileImageIndex;
						alreadyExistNode.ForeColor = SystemColors.ControlText;
					}
					else
					{
						alreadyExistNode.ImageIndex = VirtualFileImageIndex;
						alreadyExistNode.SelectedImageIndex = VirtualFileImageIndex;
						alreadyExistNode.ForeColor = SystemColors.GrayText;
					}
				}

				string LastNodePath = "";
				// Update our path to be less than what it was
				for (int i = 1; i < lastNodeParts.Length; i++)
				{
					if (LastNodePath.Length == 0)
					{
						LastNodePath = lastNodeParts[i];
					}
					else
					{
						LastNodePath = LastNodePath + "\\" + lastNodeParts[i];
					}
				}
				
				if (LastNodePath.Length > 0)
				{
					if (alreadyExistNode.Nodes.Count == 1 && alreadyExistNode.Nodes[0].Text == BlankNode)
					{
						return parent;
					}
					else
					{
						// Create a child place holder
						alreadyExistNode.Nodes.Add(BlankNode);
					}
				}
			}
				// if not, create it and all its children right here
			else
			{
				foreach (string nodeLeafName in lastNodeParts)
				{
					if (nodeLeafName.Length > 0)
					{
						// Create a node
						TreeNode newChild = new TreeNode(nodeLeafName);

						// Is this a file or directory
						if (Path.GetDirectoryName(verifyFilename).IndexOf(nodeLeafName) != -1)
						{
							// Must be a directory
							if (Directory.Exists(Path.GetDirectoryName(verifyFilename)))
							{
								newChild.ImageIndex = NonVirtualImageIndex;
								newChild.SelectedImageIndex = NonVirtualImageIndex;
								newChild.ForeColor = SystemColors.ControlText;
							}
							else
							{
								newChild.ImageIndex = VirtualImageIndex;
								newChild.SelectedImageIndex = VirtualImageIndex;
								newChild.ForeColor = SystemColors.GrayText;
							}
						} 
						else
						{
							// Must be a file
							if (File.Exists(verifyFilename))
							{
								newChild.ImageIndex = NonVirtualFileImageIndex;
								newChild.SelectedImageIndex = NonVirtualFileImageIndex;
								newChild.ForeColor = SystemColors.ControlText;
							}
							else
							{
								newChild.ImageIndex = VirtualFileImageIndex;
								newChild.SelectedImageIndex = VirtualFileImageIndex;
								newChild.ForeColor = SystemColors.GrayText;
							}
						}
						
						if (parent != null)
						{
							parent.Nodes.Add(newChild);
							parent = newChild;
						}
						else
						{
							int index = tree.Nodes.Add(newChild);
							parent = tree.Nodes[index];
						}
					}
				}

				return parent;
			}

			return null;
		}
		

		static public TreeNode CreateAndVerifyTreeNodeFullPath( TreeView tree, TreeNode parent, string delimiter, string fullPath, string verifyFilename,
			int VirtualImageIndex, int NonVirtualImageIndex, int VirtualFileImageIndex, int NonVirtualFileImageIndex)
		{
			TreeNodeCollection topNodes = null;
			
			if (parent != null)
			{
				topNodes = parent.Nodes;
			}
			else
			{
				topNodes = tree.Nodes;
			}

			// Split the full path by the delimiter passed in
			string[] lastNodeParts = fullPath.Split( delimiter.ToCharArray() );
			
			// find the first name
			TreeNode alreadyExistNode = null;
			if (string.Compare(parent.Text, lastNodeParts[0], true) == 0)
			{
				alreadyExistNode = parent;
			}
			else
			{
				alreadyExistNode = FindTreeNode(topNodes, lastNodeParts[0]);
			}

			// if exist find then next from the children of the first
			if (alreadyExistNode != null)
			{
				// Is this a file or directory
				if (Path.GetDirectoryName(verifyFilename).IndexOf(lastNodeParts[0]) != -1)
				{
					// Must be a directory
					if (Directory.Exists(Path.GetDirectoryName(verifyFilename)))
					{
						alreadyExistNode.ImageIndex = NonVirtualImageIndex;
						alreadyExistNode.SelectedImageIndex = NonVirtualImageIndex;
						alreadyExistNode.ForeColor = SystemColors.ControlText;
					}
					else
					{
						alreadyExistNode.ImageIndex = VirtualImageIndex;
						alreadyExistNode.SelectedImageIndex = VirtualImageIndex;
						alreadyExistNode.ForeColor = SystemColors.GrayText;
					}
				} 
				else
				{
					// Must be a file
					if (File.Exists(verifyFilename))
					{
						alreadyExistNode.ImageIndex = NonVirtualFileImageIndex;
						alreadyExistNode.SelectedImageIndex = NonVirtualFileImageIndex;
						alreadyExistNode.ForeColor = SystemColors.ControlText;
					}
					else
					{
						alreadyExistNode.ImageIndex = VirtualFileImageIndex;
						alreadyExistNode.SelectedImageIndex = VirtualFileImageIndex;
						alreadyExistNode.ForeColor = SystemColors.GrayText;
					}
				}

				string LastNodePath = "";
				// Update our path to be less than what it was
				for (int i = 1; i < lastNodeParts.Length; i++)
				{
					if (LastNodePath.Length == 0)
					{
						LastNodePath = lastNodeParts[i];
					}
					else
					{
						LastNodePath = LastNodePath + "\\" + lastNodeParts[i];
					}
				}
				
				if (LastNodePath.Length > 0)
				{
					// recurse into this function for the next node
					parent = CreateAndVerifyTreeNodeFullPath(tree, alreadyExistNode, delimiter, LastNodePath, verifyFilename, VirtualImageIndex, NonVirtualImageIndex, VirtualFileImageIndex, NonVirtualFileImageIndex);
				}
			}
				// if not, create it and all its children right here
			else
			{
				foreach (string nodeLeafName in lastNodeParts)
				{
					if (nodeLeafName.Length > 0)
					{
						// Create a node
						TreeNode newChild = new TreeNode(nodeLeafName);

						// Is this a file or directory
						if (Path.GetDirectoryName(verifyFilename).IndexOf(nodeLeafName) != -1)
						{
							// Must be a directory
							if (Directory.Exists(Path.GetDirectoryName(verifyFilename)))
							{
								newChild.ImageIndex = NonVirtualImageIndex;
								newChild.SelectedImageIndex = NonVirtualImageIndex;
								newChild.ForeColor = SystemColors.ControlText;
							}
							else
							{
								newChild.ImageIndex = VirtualImageIndex;
								newChild.SelectedImageIndex = VirtualImageIndex;
								newChild.ForeColor = SystemColors.GrayText;
							}
						} 
						else
						{
							// Must be a file
							if (File.Exists(verifyFilename))
							{
								newChild.ImageIndex = NonVirtualFileImageIndex;
								newChild.SelectedImageIndex = NonVirtualFileImageIndex;
								newChild.ForeColor = SystemColors.ControlText;
							}
							else
							{
								newChild.ImageIndex = VirtualFileImageIndex;
								newChild.SelectedImageIndex = VirtualFileImageIndex;
								newChild.ForeColor = SystemColors.GrayText;
							}
						}
						
						if (parent != null)
						{
							parent.Nodes.Add(newChild);
							parent = newChild;
						}
						else
						{
							int index = tree.Nodes.Add(newChild);
							parent = tree.Nodes[index];
						}
					}
				}

				return parent;
			}

			return null;
		}

		/// <summary>
		/// Given a Collection of TreeNodes, find the node with Text == name (non-recursive)
		/// </summary>
		/// <param name="Nodes">The collection to look in</param>
		/// <param name="name">The Text value to search for</param>
		/// <returns>Null, if no node found, else returns TreeNode with Text matching `name`</returns>
		static public TreeNode FindTreeNode(TreeNodeCollection Nodes, string name)
		{
			// Make sure we have valid nodes (which we should always have valid input, but better to check)
			if( Nodes != null && Nodes.Count > 0 )
			{				
				foreach( TreeNode node in Nodes )
				{
					// Does this node match the fist part of our split path?
					if( string.Compare(node.Text, name, true) == 0 )
					{
						return node;
					} 
				}
			}

			return null;
		}

		/// <summary>
		/// Given a Collection of TreeNodes, a delimiter, and the fullPath of the desired node, drill into
		/// the given Collection to find TreeNode with FullPath of `fullPath`
		/// </summary>
		/// <param name="Nodes">Collection of TreeNodes</param>
		/// <param name="delimiter">Delimiter used by TreeView being searched</param>
		/// <param name="fullPath">Path of node to be found</param>
		/// <returns>Null if node not found, otherwise, returns the TreeNode being sought</returns>
		static public TreeNode FindAndExpandTreeNodeFromFullPath( TreeNodeCollection Nodes, string delimiter, string fullPath )
		{
			// Make sure we have valid nodes (which we should always have valid input, but better to check)
			if( Nodes != null && Nodes.Count > 0 )
			{
				// Split the full path by the delimiter passed in
				string[] lastNodeParts = fullPath.Split( delimiter.ToCharArray() );
				if( lastNodeParts.Length > 0 )
				{
					foreach( TreeNode node in Nodes )
					{
						// Does this node match the fist part of our split path?
						if( string.Compare(node.Text, lastNodeParts[0], true) == 0 )
						{
							// Update our path to be less than what it was
							mLastNodePath = fullPath.Replace(lastNodeParts[0], "");
							mLastNodePath = mLastNodePath.TrimStart(delimiter.ToCharArray());

							// Expand the node
							node.Expand();

							// Should we continue to drill?
							if (mLastNodePath.Length != 0 && node.Nodes.Count > 0)
							{
								return FindAndExpandTreeNodeFromFullPath(node.Nodes, delimiter, mLastNodePath);
							}
							else
							{
								return node;
							}
						} 
					} // end foreach
				} // end if
			} // end if

			return null;
		} // end ()

		/// <summary>
		/// Given a Collection of TreeNodes, a delimiter, and the fullPath of the desired node, drill into
		/// the given Collection to find TreeNode with FullPath of `fullPath`
		/// </summary>
		/// <param name="Nodes">Collection of TreeNodes</param>
		/// <param name="delimiter">Delimiter used by TreeView being searched</param>
		/// <param name="fullPath">Path of node to be found</param>
		/// <returns>Null if node not found, otherwise, returns the TreeNode being sought</returns>
		static public TreeNode FindVisibleTreeNodeFromFullPath( TreeNodeCollection Nodes, string delimiter, string fullPath )
		{
			// Make sure we have valid nodes (which we should always have valid input, but better to check)
			if( Nodes != null && Nodes.Count > 0 )
			{
				// Split the full path by the delimiter passed in
				string[] lastNodeParts = fullPath.Split( delimiter.ToCharArray() );
				if( lastNodeParts.Length > 0 )
				{
					foreach( TreeNode node in Nodes )
					{
						// Does this node match the fist part of our split path?
						if( string.Compare(node.Text, lastNodeParts[0], true) == 0 )
						{
							// Update our path to be less than what it was
							mLastNodePath = fullPath.Replace(lastNodeParts[0], "");
							mLastNodePath = mLastNodePath.TrimStart(delimiter.ToCharArray());

							// Should we continue to drill?
							if (mLastNodePath.Length != 0 && node.Nodes.Count > 0)
							{
								return FindVisibleTreeNodeFromFullPath(node.Nodes, delimiter, mLastNodePath);
							}
							else
							{
								return node;
							}
						} 
					} // end foreach
				} // end if
			} // end if

			return null;
		} // end ()
	}
}
