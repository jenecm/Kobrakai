using System;
using Amazon.DynamoDBv2.DataModel;

namespace Glados.Droid
{
	[DynamoDBTable("kobrakaiLocations")]
	public class locationsDDB
	{
		[DynamoDBHashKey]
		public string id { get; set; }
		public string beacon { get; set; }
		public string qr { get; set; }
	}
}
