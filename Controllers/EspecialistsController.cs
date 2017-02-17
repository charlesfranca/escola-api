using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// using NLog;

namespace EscolaDeVoce.API.Controllers
{
    [Route("api/[controller]")]
    public class EspecialistsController : Controller
    {
        // private static Logger _logger = LogManager.GetCurrentClassLogger();

        Services.Interfaces.IEspecialistService _especialistService;
        
        public EspecialistsController(Services.Interfaces.IEspecialistService especialistService){
            _especialistService = especialistService;
            // _logger.Info("App initialized");
        }

        private Infrastructure.ApiResponse<T> CreateResponse<T>(bool status, string message, T data, System.Net.HttpStatusCode code = System.Net.HttpStatusCode.Created, List<Infrastructure.BusinessRule> rules = null){
            var response = new Infrastructure.ApiResponse<T>();
            response.status = status;
            response.error_message = message;
            response.data = data;
            response.brokenRules = rules;
            response.code = code;
            return response;
        }

        [HttpGet]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.EspecialistViewModel>> Get()
        {
            try{
                var data = _especialistService.GetEspecialists(new Services.Message.GetEspecialistsRequest()).especialists;
                return CreateResponse<IList<Services.ViewModel.EspecialistViewModel>>(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return CreateResponse<IList<Services.ViewModel.EspecialistViewModel>>(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception ex){
                return CreateResponse<IList<Services.ViewModel.EspecialistViewModel>>(false, ex.Message, null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.EspecialistViewModel> Get(string id)
        {
            try{
                Guid especialistid = Guid.Empty;
                if(!Guid.TryParse(id, out especialistid))
                    return CreateResponse<Services.ViewModel.EspecialistViewModel>(false, "Projeto não encontrado", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetEspecialistRequest req = new Services.Message.GetEspecialistRequest();
                req.especialistId = especialistid;
                var especialist =  _especialistService.GetEspecialist(req).especialist; 
                return CreateResponse<Services.ViewModel.EspecialistViewModel>(true, "", especialist);
            }catch(Infrastructure.BusinessRuleException bex){
                return CreateResponse<Services.ViewModel.EspecialistViewModel>(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return CreateResponse<Services.ViewModel.EspecialistViewModel>(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public Infrastructure.ApiResponse<Services.ViewModel.EspecialistViewModel> Post([FromBody]Services.ViewModel.EspecialistViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.EspecialistViewModel>();
            try{
                var req = new Services.Message.AddEspecialistRequest();
                req.especialist = model;
                _especialistService.AddEspecialist(req);
                
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
        public Infrastructure.ApiResponse<Services.ViewModel.EspecialistViewModel> Put(string id, [FromBody]Services.ViewModel.EspecialistViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.EspecialistViewModel>();
            try{
                Guid especialistid = Guid.Empty;
                if(!Guid.TryParse(id, out especialistid))
                    return CreateResponse<Services.ViewModel.EspecialistViewModel>(false, "projeto não encontrado", null, System.Net.HttpStatusCode.NotFound);
                
                model.Id = especialistid;
                _especialistService.UpdateEspecialist(new Services.Message.UpdateEspecialistRequest(){ especialist = model });

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
        public Infrastructure.ApiResponse<Services.ViewModel.EspecialistViewModel> Delete(string id)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.EspecialistViewModel>();
            try{
                Guid especialistid = Guid.Empty;
                if(!Guid.TryParse(id, out especialistid))
                    return CreateResponse<Services.ViewModel.EspecialistViewModel>(false, "Projeto não encontrado", null, System.Net.HttpStatusCode.NotFound); 

                _especialistService.RemoveEspecialist(new Services.Message.DeleteEspecialistRequest(){especialistId = especialistid});
                
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
