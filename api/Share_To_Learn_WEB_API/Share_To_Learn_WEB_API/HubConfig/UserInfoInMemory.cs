using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Share_To_Learn_WEB_API.HubConfig
{
    public class UserInfoInMemory
    {
        private ConcurrentDictionary<string, UserInfo> _onlineUser { get; set; } = new ConcurrentDictionary<string, UserInfo>();

        public bool AddUpdate(string email, string connectionId)
        {
            var userAlreadyExists = _onlineUser.ContainsKey(email);

            var userInfo = new UserInfo
            {
                Email = email,
                ConnectionId = connectionId
            };

            _onlineUser.AddOrUpdate(email, userInfo, (key, value) => userInfo);

            return userAlreadyExists;
        }

        public void Remove(string email)
        {
            UserInfo userInfo;
            _onlineUser.TryRemove(email, out userInfo);
        }

        public IEnumerable<UserInfo> GetAllUsersExceptThis(string email)
        {
            return _onlineUser.Values.Where(item => item.Email != email);
        }

        public UserInfo GetUserInfo(string email)
        {
            UserInfo user;
            _onlineUser.TryGetValue(email, out user);
            return user;
        }
    }
}
