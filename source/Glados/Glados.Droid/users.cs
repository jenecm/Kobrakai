using System;
using System.Collections.Generic;
using System.Linq;

namespace Glados.Droid
{
	public static class users
	{
		static List<usersDDB> userList = new List<usersDDB>();

		public static List<usersDDB> getUserList()
		{
			return userList;
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

		public static void setUsersList(List<usersDDB> usersList)
		{
			foreach (usersDDB a_user in usersList)
			{
				bool containsItem = userList.Any(usersDDB => usersDDB.id == a_user.id);
				if (containsItem)
				{
					return;
				}
				userList.Add(a_user);
			}
		}
	}
}
