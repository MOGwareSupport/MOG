using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MOG_EventViewer
{
	internal class ColumnSorter : IComparer<ListViewItem>
	{
		private int m_column;
		private bool m_ascending;

		public ColumnSorter(int column, bool ascending)
		{
			m_column = column;
			m_ascending = ascending;
		}

		public int Compare(ListViewItem item0, ListViewItem item1)
		{
			int result = 0;

			if (item0.SubItems.Count > m_column &&
				item1.SubItems.Count > m_column)
			{
				string text0 = item0.SubItems[m_column].Text;
				string text1 = item1.SubItems[m_column].Text;

				result = String.Compare(text0, text1);
			}

			return m_ascending ? result : -result;
		}
	}
}
