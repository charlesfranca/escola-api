using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EscolaDeVoce.API
{
    public class AutoMapperSetup
    {
        public static void Config(){
            AutoMapper.Mapper.Initialize(x=>{
                x.CreateMap<EscolaDeVoce.Model.Category, Services.CategoryService>();
            });
        }
    }
}
