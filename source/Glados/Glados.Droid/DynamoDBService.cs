using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;

namespace Glados.Droid
{
	public class DynamoDBService
	{
		

		public async Task ScanAsync(CognitoAWSCredentials credentials)
		{
			var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USWest2);
			// Pass the client to the DynamoDBContex
			DynamoDBContext context = new DynamoDBContext(client);

			var search = context.FromScanAsync<usersDDB>(new Amazon.DynamoDBv2.DocumentModel.ScanOperationConfig()
			{
				ConsistentRead = true
			});
			var searchResponse = await search.GetRemainingAsync();
			searchResponse.ForEach(users.addUserToList);
		}
	}
}
