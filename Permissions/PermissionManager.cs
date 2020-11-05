using System;
using TCAdmin.SDK;
using TCAdmin.SDK.Objects;
using TCAdmin.SDK.Security;

namespace Alexr03.Common.TCAdmin.Permissions
{
    public class PermissionManager<T> where T : Enum
    {
        private readonly string _moduleId;

        public PermissionManager(string moduleId)
        {
            _moduleId = moduleId ?? throw new ArgumentNullException(nameof(moduleId));
        }

        public bool CurrentUserHasPermission(T permission)
        {
            var user = Session.GetCurrentUser();
            return UserHasPermission(user, permission);
        }

        public bool UserHasPermission(User user, T permission)
        {
            return user.UserType == UserType.Admin ||
                   SecurityManager.UserHasPermission(user, _moduleId, Convert.ToInt32(permission));
        }

        public void ThrowIfCurrentUserLackPermission(T permission)
        {
            var user = Session.GetCurrentUser();
            ThrowIfUserLackPermission(user, permission);
        }

        public void ThrowIfUserLackPermission(User user, T permission)
        {
            if (!UserHasPermission(user, permission)) throw new SecurityException("Access is denied");
        }
    }
}