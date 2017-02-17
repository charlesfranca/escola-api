using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscolaDeVoce.API.Controllers
{
    public class BaseController : Controller
    {
        protected string getClaimValue(string name){
            var identity = (ClaimsIdentity) User.Identity;

            var value = identity.Claims.Where(c => c.Type == name)
                    .Select(c => c.Value).SingleOrDefault();

            return value;
        }
    }
}
