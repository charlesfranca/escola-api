using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EscolaDeVoce.API
{
    public class AuthenticationModel
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}
