using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using MBProgressHUD;
using System.Threading.Tasks;
using System.Threading;
using RestSharp.Portable;
using System.Net.Http;
using System.Collections.Generic;
using RestSharp.Portable.Deserializers;
using System.Linq;

namespace JsonTable
{
	partial class JsonDataTableViewController : UIViewController
	{
		public const string jsonApiUrl = "http://www.whatsnewbrowser.com/tl-sources.ashx";

		MTMBProgressHUD progressIndicator;

		public JsonDataTableViewController (IntPtr handle) : base (handle)
		{
		}

		public async override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Console.WriteLine("view controller loaded.");


			progressIndicator = new MTMBProgressHUD (View) {
				LabelText = "Loading data from json api...",
				RemoveFromSuperViewOnHide = true
			};

			View.AddSubview (progressIndicator);

			progressIndicator.Show (animated: true);

			var data = await loadData();

			progressIndicator.Hide (true);

			//sort data
			data = data.OrderBy( item => item.NAME).ToList();


			if (data != null) {
				JsonDataTable.Source = new JsonDataTableSource (data);
				JsonDataTable.ReloadData ();
			}
			// Perform any additional setup after loading the view, typically from a nib.
		}

		private async Task<List<Datum>> loadData(){
			
			try {
				RestClient client = new RestClient (jsonApiUrl);
				client.AddHandler ("text/plain", new RestSharpJsonNetDeserializer ());
				RestRequest request = new RestRequest (HttpMethod.Get);
				
				var result = await client.Execute<List<Datum>> (request).ConfigureAwait (false);
				
				return result.Data;
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);	
			}
			return null;
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
