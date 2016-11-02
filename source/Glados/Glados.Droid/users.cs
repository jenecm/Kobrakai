using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Glados.Droid
{
	public static class users
	{
        static List<usersDDB> userList = new List<usersDDB>();

        public static List<usersDDB> getUserList()
        {
            return userList;
        }

        public static List<string> getNames()
        {
            List<string> names = new List<string>();
            foreach (usersDDB the_user in userList)
            {
                names.Add(the_user.name);
            }
            return names;
        }

        public static void updateUser(usersDDB user)
        {
            bool containsItem = userList.Any(usersDDB => usersDDB.id == user.id);
            if (containsItem)
            {
                deleteUserFromList(user);
                addUserToList(user);
            }
        }

        public static void addUserToList(usersDDB user)
        {
            bool containsItem = userList.Any(usersDDB => usersDDB.id == user.id);
            if (containsItem)
            {
                return;
            }
            userList.Add(user);
        }

        public static usersDDB getUser(string id)
        {
            usersDDB theUser = new usersDDB();
            bool containsItem = userList.Any(usersDDB => usersDDB.id == id);
            if (containsItem)
            {
                foreach (usersDDB a_user in userList)
                {
                    if (id.Equals(a_user.id))
                    {
                        theUser.id = a_user.id;
                        theUser.name = a_user.name;
                        theUser.position = a_user.position;
                        theUser.location = a_user.location;
                    }
                }
            }
            else
            {
                theUser.name = "name not retrieved";
            }
            return theUser;
        }

        public static List<usersDDB> getUsersByName(string name)
        {
            List<usersDDB> theUser = new List<usersDDB>();
            bool containsItem = userList.Any(usersDDB => usersDDB.name == name);
            if (containsItem)
            {
                foreach (usersDDB a_user in userList)
                {
                    if (name.Contains(a_user.name))
                    {
                        theUser.Add(a_user);
                    }
                }
            }
            
            return theUser;
        }

        public static void deleteUserFromList(usersDDB user)
        {
            bool containsItem = userList.Any(usersDDB => usersDDB.id == user.id);
            if (containsItem)
            {
                foreach (usersDDB a_user in userList)
                {
                    if (a_user.id.Equals(user.id))
                    {
                        userList.Remove(a_user);
                    }
                }
            }
        }

        public static async Task setUsersList()
        {
            // access the database and populate the list with items from the DDB
            CognitoAWSCredentials credentials = new CognitoAWSCredentials(
                           "us-west-2:d17455cb-c093-403a-a797-d8b01906f7b2", // Identity Pool 

                           RegionEndpoint.USWest2 // Regio

                       );
            var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USWest2);

            using (client)
            {

                var queryResponse = await client.ScanAsync(new ScanRequest()
                {
                    TableName = "kobrakaiUsers"
                });

                queryResponse.Items.ForEach((i) =>
                {
                    usersDDB theUser = new usersDDB();
                    theUser.id = (i["id"].S);

                    theUser.name = (i["name"].S);

                    theUser.position = (i["position"].S);

                    theUser.location = (i["location"].S);

                    addUserToList(theUser);
                });

            }
        }
    }
}
