using System;
using UIKit;
using System.Collections.Generic;
using Foundation;

namespace JsonTable
{
	public class JsonDataTableSource : UITableViewSource {

		List<Datum> TableItems;

		string CellIdentifier = "JsonDataTableCell";

		public JsonDataTableSource (List<Datum> items)
		{
			TableItems = items;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return TableItems.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (CellIdentifier);
			Datum item = TableItems[indexPath.Row];

			//---- if there are no cells to reuse, create a new one
			if (cell == null)
			{ 
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, CellIdentifier); 
			}

			cell.TextLabel.Text = item.NAME + " - " + item.TWITTER;
			cell.DetailTextLabel.Text = item.WWW;

			return cell;
		}
	}
}

