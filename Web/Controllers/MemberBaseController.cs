using Common;
using System;

namespace Web.Controllers
{
    [Web.Controllers.Permissions]
    public class MemberBaseController : BaseController
    {
        // GET: MemberBase
        public override Enums.LoginType getLoginType()
        {
            return Enums.LoginType.member;
        }
    }
}