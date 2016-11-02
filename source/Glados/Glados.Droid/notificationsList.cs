using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace Glados.Droid
{
	public static class notificationsList
	{
		private static List<notificationsDDB> notifications_list = new List<notificationsDDB>();

		public static List<notificationsDDB> getNotificationsList()
		{
			return notifications_list;
		}

		public static void updateTheNotifications(notificationsDDB notification)
		{
			bool containsItem = notifications_list.Any(usersDDB => usersDDB.id == notification.id);
			if (containsItem)
			{
				deleteNotificationFromList(notification);
				addNotificationToList(notification);
			}
		}

		public static void addNotificationToList(notificationsDDB notification)
		{
			bool containsItem = notifications_list.Any(notificationsDDB => notificationsDDB.id == notification.id);
			if (containsItem)
			{
				updateTheNotifications(notification);
			}
			else {
				notifications_list.Add(notification);
			}

		}

		public static void deleteNotificationFromList(notificationsDDB notification)
		{
			bool containsItem = notifications_list.Any(usersDDB => usersDDB.id == notification.id);
			if (containsItem)
			{
				foreach (notificationsDDB a_notification in notifications_list)
				{
					if (a_notification.id.Equals(notification.id))
					{
						notifications_list.Remove(a_notification);
					}
				}
			}
		}

		public static notificationsDDB getNotification(string id)
		{
			notificationsDDB theNotification = new notificationsDDB();
			bool containsItem = notifications_list.Any(notificationsDDB => notificationsDDB.id == id);
			if (containsItem)
			{
				foreach (notificationsDDB a_notification in notifications_list)
				{
					if (id.Equals(a_notification.id))
					{
						return a_notification;
					}
				}
			}

			return theNotification;

		}

		public static void updateNotification(string id)
		{
			foreach (notificationsDDB a_notification in notifications_list)
			{
				if (id.Equals(a_notification.id))
				{
					a_notification.active = "false";
				}
			}
		}


		public static async Task setNotificationsList()
		{

			// access the database and populate the list with items from the DDB
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
					TableName = "kobrakaiNotifications"
				});

				queryResponse.Items.ForEach((i) =>
				{
					if (i["lost"].S.Equals(User.GetId()))
					{
						notificationsDDB theNotification = new notificationsDDB();
						theNotification.id = (i["id"].S);

						theNotification.lost = (i["lost"].S);

						theNotification.searcher = (i["searcher"].S);

						theNotification.active = (i["active"].S);

						addNotificationToList(theNotification);
					}
				});

			}
		}



		public static async Task addNotificagtionToDDB(string id, string lost, string searcher, string active)
		{
			//Contract.Ensures(Contract.Result<Task>() != null);
			CognitoAWSCredentials credentials = new CognitoAWSCredentials(
							   "us-west-2:d17455cb-c093-403a-a797-d8b01906f7b2", // Identity Pool 

							   RegionEndpoint.USWest2 // Regio

						   );
			var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USWest2);

			var theNotification = new Document();

			theNotification["id"] = id;
			theNotification["lost"] = lost;
			theNotification["searcher"] = searcher;
			theNotification["active"] = active;

			Table notifications = Table.LoadTable(client, "kobrakaiNotifications");

			Document updatedNotification = await notifications.UpdateItemAsync(theNotification);


		}
	}

}
