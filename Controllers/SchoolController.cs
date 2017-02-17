using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscolaDeVoce.API.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    public class SchoolController : Controller
    {
        Services.Interfaces.ISchoolService _schoolService;
        
        public SchoolController(Services.Interfaces.ISchoolService schoolService){
            _schoolService = schoolService;
        }

        [HttpGet]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.SchoolViewModel>> Get()
        {
            Console.Write(User.Identity.Name);
            try{
                var data = _schoolService.GetSchools(new Services.Message.GetSchoolsRequest()).schools;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.SchoolViewModel>>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.SchoolViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.SchoolViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.SchoolViewModel> Get(string id)
        {
            try{
                Guid schoolid = Guid.Empty;
                if(!Guid.TryParse(id, out schoolid))
                    return Infrastructure.ApiResponse<Services.ViewModel.SchoolViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetSchoolRequest req = new Services.Message.GetSchoolRequest();
                req.schoolId = schoolid;
                var category =  _schoolService.GetSchool(req).school; 
                return Infrastructure.ApiResponse<Services.ViewModel.SchoolViewModel>.CreateResponse(true, "", category);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<Services.ViewModel.SchoolViewModel>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<Services.ViewModel.SchoolViewModel>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }            
        }

        [HttpPost]
        public Infrastructure.ApiResponse<Services.ViewModel.SchoolViewModel> Post([FromBody]Services.ViewModel.SchoolViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.SchoolViewModel>();
            try{
                var req = new Services.Message.AddSchoolRequest();
                req.school = model;

                _schoolService.AddSchool(req);

                response.status = true;
                response.data = model;
                response.code = System.Net.HttpStatusCode.Created;
            }catch(Infrastructure.BusinessRuleException bex){
                response.status = true;
                response.code = System.Net.HttpStatusCode.BadRequest;
                response.brokenRules = bex.BrokenRules;
                response.error_message = bex.Message;
            }catch(Exception ex){
                response.status = true;
                response.code = System.Net.HttpStatusCode.InternalServerError;
                response.error_message = "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.";
            }
            return response;
        }

        [HttpPut("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.SchoolViewModel> Put(string id, [FromBody]Services.ViewModel.SchoolViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.SchoolViewModel>();
            try{
                Guid schoolid = Guid.Empty;
                if(!Guid.TryParse(id, out schoolid))
                    return Infrastructure.ApiResponse<Services.ViewModel.SchoolViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound);
                
                model.Id = schoolid;
                _schoolService.UpdateSchool(new Services.Message.UpdateSchoolRequest(){ school = model });

                response.status = true;
                response.data = model;
                response.code = System.Net.HttpStatusCode.Created;
            }catch(Infrastructure.BusinessRuleException bex){
                response.status = true;
                response.code = System.Net.HttpStatusCode.BadRequest;
                response.brokenRules = bex.BrokenRules;
                response.error_message = bex.Message;
            }catch(Exception ex){
                response.status = true;
                response.code = System.Net.HttpStatusCode.InternalServerError;
                response.error_message = "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.";
            }

            return response;
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.SchoolViewModel> Delete(string id)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.SchoolViewModel>();
            try{
                Guid schoolid = Guid.Empty;
                if(!Guid.TryParse(id, out schoolid))
                    return Infrastructure.ApiResponse<Services.ViewModel.SchoolViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound); 

                _schoolService.RemoveSchool(new Services.Message.RemoveSchoolRequest(){schoolId = schoolid});
                
                response.status = true;
                response.data = null;
                response.code = System.Net.HttpStatusCode.Created;
            }catch(Infrastructure.BusinessRuleException bex){
                response.status = true;
                response.code = System.Net.HttpStatusCode.BadRequest;
                response.brokenRules = bex.BrokenRules;
                response.error_message = bex.Message;
            }catch(Exception ex){
                response.status = true;
                response.code = System.Net.HttpStatusCode.InternalServerError;
                response.error_message = "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.";
            }

            return response;
        }
    }
}
