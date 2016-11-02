using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;

namespace Glados.Droid
{
	public static class locationsID
	{
		static List<string> locationsList = new List<string>();

		public static List<string> getLocationsList()
		{
			return locationsList;
		}

		public static void addLocationToList(string location)
		{
			if (locationsList.Contains(location))
			{
				return;
			}
			locationsList.Add(location);
		}

		public static void setLocationsList(List<string> locations)
		{
			foreach (string a_location in locations)
			{
				if (locationsList.Contains(a_location))
				{
					return;
				}
				locationsList.Add(a_location);
			}
		}

		public static async Task getListFromDDB()
		{
			CognitoAWSCredentials credentials = new CognitoAWSCredentials(
							   "us-west-2:d17455cb-c093-403a-a797-d8b01906f7b2", // Identity Pool 

							   RegionEndpoint.USWest2 // Regio

						   );
			var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USWest2);
			// Pass the client to the DynamoDBConte
			//DynamoDBContext context = new DynamoDBContext(client);

			using (client)
			{
				var queryResponse = await client.ScanAsync(new ScanRequest()
				{
					TableName = "kobrakaiLocations"
				});
				queryResponse.Items.ForEach((i) =>
				{
					addLocationToList(i["id"].S);
				});
			}

		}
	}
}

