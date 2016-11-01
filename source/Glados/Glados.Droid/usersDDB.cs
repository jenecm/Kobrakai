using System;
using Amazon.DynamoDBv2.DataModel;
namespace Glados.Droid
{
	[DynamoDBTable("kobrakaiUsers")]
	public class usersDDB
	{
		[DynamoDBHashKey]
		public string id { get; set; }
		public string location { get; set; }
		public string name { get; set; }
		public string position { get; set; }
	}
}
