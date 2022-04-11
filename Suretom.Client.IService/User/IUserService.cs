using Suretom.Client.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suretom.Client.IService
{
    public interface IUserService
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Userinfo GetUserInfo();
    }
}