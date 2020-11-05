using System.Web.Mvc;
using TCAdmin.SDK.Objects;
using TCAdmin.SDK.Security;

namespace Alexr03.Common.TCAdmin.Web.Attributes.ActionFilters
{
    public class RequiresPermissionAttribute : ActionFilterAttribute
    {
        private readonly string _moduleId;
        private readonly int _permissionId;
        private readonly StandardPermission _standardPermission;
        private readonly bool _standardPermissionSet;

        public RequiresPermissionAttribute(string moduleId, int permissionId)
        {
            _moduleId = moduleId;
            _permissionId = permissionId;
        }

        public RequiresPermissionAttribute(StandardPermission standardPermission)
        {
            _standardPermission = standardPermission;
            _standardPermissionSet = true;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = global::TCAdmin.SDK.Session.GetCurrentUser();

            if (user.UserType != UserType.Admin && _standardPermissionSet &&
                !SecurityManager.CurrentUserHasPermission(_standardPermission) ||
                !SecurityManager.CurrentUserHasPermission(_moduleId, _permissionId))
                throw new SecurityException("Access is denied");
        }
    }
}