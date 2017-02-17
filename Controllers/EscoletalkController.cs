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
    public class EscoletalkController : Controller
    {
        Services.Interfaces.IEscoleteTalkService _escoleteTalkService;
        
        public EscoletalkController(Services.Interfaces.IEscoleteTalkService escoleteTalkService){
            _escoleteTalkService = escoleteTalkService;
        }

        [HttpGet]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.EscoleteTalkViewModel>> Get()
        {
            Console.Write(User.Identity.Name);
            try{
                var data = _escoleteTalkService.GetEscoleteTalks(new Services.Message.GetEscoleteTalksRequest()).escoleteTalks;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.EscoleteTalkViewModel>>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.EscoleteTalkViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception ex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.EscoleteTalkViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.EscoleteTalkViewModel> Get(string id)
        {
            try{
                Guid escoleteTalkId = Guid.Empty;
                if(!Guid.TryParse(id, out escoleteTalkId))
                    return Infrastructure.ApiResponse<Services.ViewModel.EscoleteTalkViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetEscoleteTalkRequest req = new Services.Message.GetEscoleteTalkRequest();
                req.escoleteTalkId = escoleteTalkId;
                var escoleteTalk =  _escoleteTalkService.GetEscoleteTalk(req).escoleteTalk; 
                return Infrastructure.ApiResponse<Services.ViewModel.EscoleteTalkViewModel>.CreateResponse(true, "", escoleteTalk);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<Services.ViewModel.EscoleteTalkViewModel>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<Services.ViewModel.EscoleteTalkViewModel>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }            
        }

        [HttpPost]
        public Infrastructure.ApiResponse<Services.ViewModel.EscoleteTalkViewModel> Post([FromBody]Services.ViewModel.EscoleteTalkViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.EscoleteTalkViewModel>();
            try{
                var req = new Services.Message.AddEscoleteTalkRequest();
                req.escoleteTalk = model;

                _escoleteTalkService.AddEscoleteTalk(req);

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

        [HttpPost("comment")]
        public Infrastructure.ApiResponse<bool> PostComment([FromBody]Services.ViewModel.CommentEscoleteTalkViewModel model)
        {
            var response = new Infrastructure.ApiResponse<bool>();
            try{
                var req = new Services.Message.CommentEscoleteTalkRequest();
                req.escoleteTalk = model;

                _escoleteTalkService.CommentEscoleteTalk(req);

                response.status = true;
                //response.data = model;
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
        public Infrastructure.ApiResponse<Services.ViewModel.EscoleteTalkViewModel> Put(string id, [FromBody]Services.ViewModel.EscoleteTalkViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.EscoleteTalkViewModel>();
            try{
                Guid escoleteTalk = Guid.Empty;
                if(!Guid.TryParse(id, out escoleteTalk))
                    return Infrastructure.ApiResponse<Services.ViewModel.EscoleteTalkViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound);
                
                model.Id = escoleteTalk;
                _escoleteTalkService.UpdateEscoleteTalk(new Services.Message.UpdateEscoleteTalkRequest(){ escoleteTalk = model });

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
        public Infrastructure.ApiResponse<Services.ViewModel.EscoleteTalkViewModel> Delete(string id)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.EscoleteTalkViewModel>();
            try{
                Guid escoleteTalkId = Guid.Empty;
                if(!Guid.TryParse(id, out escoleteTalkId))
                    return Infrastructure.ApiResponse<Services.ViewModel.EscoleteTalkViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound); 

                _escoleteTalkService.RemoveEscoleteTalk(new Services.Message.DeleteEscoleteTalkRequest(){ escoleteTalkId = escoleteTalkId});
                
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
