using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using System.Linq;

namespace JsonTable
{
	public class JsonDataTableSource : UITableViewSource {

		const string CellIdentifier = "JsonDataTableCell";

		List<Datum> TableItems;
		Dictionary<string, List<Datum>> indexedTableItems;
		string[] keys;

		public JsonDataTableSource (List<Datum> items)
		{
			TableItems = items;

			indexedTableItems = new Dictionary<string, List<Datum>>();
			foreach (var t in items) {
				if (indexedTableItems.ContainsKey (t.NAME[0].ToString ())) {
					indexedTableItems[t.NAME[0].ToString ()].Add(t);
				} else {
					indexedTableItems.Add (t.NAME[0].ToString (), new List<Datum>() {t});
				}
			}
			keys = indexedTableItems.Keys.ToArray ();
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return keys.Length;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return indexedTableItems[keys[section]].Count;
		}

		public override string[] SectionIndexTitles (UITableView tableView)
		{
			return keys;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (CellIdentifier);

			Datum item = indexedTableItems[keys[indexPath.Section]][indexPath.Row];

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

