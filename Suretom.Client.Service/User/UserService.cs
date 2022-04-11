using Newtonsoft.Json;
using Suretom.Client.Common;
using Suretom.Client.Entity;
using Suretom.Client.IService;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suretom.Client.Service
{
    public class UserService : ServiceBase, IUserService
    {
        public UserService()
        {
            Urls.Add("Info", "User/Info");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public Userinfo GetUserInfo()
        {
            var param = new { GlobalContext.Token, };

            var result = Get(JsonConvert.SerializeObject(param), Urls["Info"], this.GetType());

            if (result.Success)
            {
                return JsonConvert.DeserializeObject<Userinfo>(result.Data.ToString());
            }

            return null;
        }
    }
}