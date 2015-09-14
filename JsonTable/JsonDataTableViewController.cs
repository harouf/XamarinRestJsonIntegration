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
using System.CodeDom.Compiler;
using System.Security.Cryptography;
using System.Text;
using System.Net;

namespace JsonTable
{
	partial class JsonDataTableViewController : UIViewController
	{
		public const string jsonApiUrl = "http://www.whatsnewbrowser.com/tl-sources.ashx?p=";
		public const string serverTimeApiUrl = "http://www.whatsnewbrowser.com/time.ashx";

		MTMBProgressHUD progressIndicator;

		#region generate passcode for server
		public static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1,
			1, 0, 0, 0, DateTimeKind.Utc);

		public static DateTime ServerTime()
		{
			// FETCH SERVER TIME FROM SERVER
			// THIS WILL BE DONE MANY TIMES WHILE APP IS OPEN
			// SET GLOBAL VAR THAT KEEPS TRACK OF TIME ONCE BEING SYNCED WITH SERVER TIME

			var textFromFile = (new WebClient()).DownloadString(serverTimeApiUrl);
			//Console.WriteLine (textFromFile);

			return DateTime.Parse(textFromFile);
		}

		public static string GetPassword()
		{
			var secret = "3ef37cdc-0640-4f85-9c51-becd176105c7";

			var theTime = ServerTime ();
			long counter = (long)(theTime - UNIX_EPOCH).TotalSeconds / 90;

			return GeneratePassword(secret, counter);
		}

		public static string GeneratePassword(string secret, long
			iterationNumber, int digits = 6)
		{
			byte[] counter = BitConverter.GetBytes(iterationNumber);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(counter);

			byte[] key = Encoding.ASCII.GetBytes(secret);

			HMACSHA1 hmac = new HMACSHA1(key, true);

			byte[] hash = hmac.ComputeHash(counter);

			int offset = hash[hash.Length - 1] & 0xf;

			int binary =
				((hash[offset] & 0x7f) << 24)
				| ((hash[offset + 1] & 0xff) << 16)
				| ((hash[offset + 2] & 0xff) << 8)
				| (hash[offset + 3] & 0xff);

			int password = binary % (int)Math.Pow(10, digits); // 6 digits

			return password.ToString(new string('0', digits));
		}
		#endregion

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

			if (data != null) {
				//sort data
				data = data.OrderBy (item => item.NAME).ToList ();
			} else {
				data = new List<Datum>{ new Datum (){ NAME = "No Results" } };
			}
			JsonDataTable.Source = new JsonDataTableSource (data);
			JsonDataTable.ReloadData ();

			// Perform any additional setup after loading the view, typically from a nib.
		}

		private async Task<List<Datum>> loadData(){
			
			try {
				var jsonApiUrlProtected = jsonApiUrl +
					GetPassword().ToString();
				Console.WriteLine("Protected Api Url: " + jsonApiUrlProtected);

				RestClient client = new RestClient (jsonApiUrlProtected);
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
