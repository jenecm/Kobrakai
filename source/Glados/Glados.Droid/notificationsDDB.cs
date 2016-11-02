using System;
using Amazon.DynamoDBv2.DataModel;

namespace Glados.Droid
{
		[DynamoDBTable("kobrakaiNotifications")]
		public class notificationsDDB
		{
			[DynamoDBHashKey]
			public string id { get; set; }
			public string lost { get; set; }
			public string searcher { get; set; }
			public string active { get; set; }
		}

}
