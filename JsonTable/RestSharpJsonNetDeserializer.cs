using System;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace RestSharp.Portable.Deserializers
{
	public class RestSharpJsonNetDeserializer : IDeserializer
	{
		public static string DateTimeFormatString = "dd/MM/yyyy h:mm:ss tt zzz";

		private readonly Newtonsoft.Json.JsonSerializer _deserializer;

		/// <summary>
		/// Default serializer
		/// </summary>
		public RestSharpJsonNetDeserializer()
		{
			_deserializer = new Newtonsoft.Json.JsonSerializer
			{
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Include,
				DefaultValueHandling = DefaultValueHandling.Include,
				DateParseHandling = DateParseHandling.DateTimeOffset,
				DateFormatString = DateTimeFormatString,
				Culture = new System.Globalization.CultureInfo("en-US")
			};
		}

		/// <summary>
		/// Default deserializer with overload for allowing custom Json.NET settings
		/// </summary>
		public RestSharpJsonNetDeserializer(Newtonsoft.Json.JsonSerializer serializer)
		{
			_deserializer = serializer;
		}


		#region IDeserializer implementation
		public T Deserialize<T> (IRestResponse response)
		{
			Encoding encoding = Encoding.UTF8;
			string content = encoding.GetString (response.RawBytes, 0, response.RawBytes.Length);
			using (var stringReader = new StringReader(content))
			{
				using (var jsonTextReader = new JsonTextReader(stringReader))
				{
					var result = _deserializer.Deserialize<T> (jsonTextReader);
					return result;
				}
			}
		}
		#endregion
	}
}

