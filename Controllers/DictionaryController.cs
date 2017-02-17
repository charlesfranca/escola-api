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
    public class DictionaryController : Controller
    {
        Services.Interfaces.IDictionaryService _dictionaryService;
        
        public DictionaryController(Services.Interfaces.IDictionaryService dictionaryService){
            _dictionaryService = dictionaryService;
        }

        [HttpGet]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.DictionaryViewModel>> Get()
        {
            Console.Write(User.Identity.Name);
            try{
                var data = _dictionaryService.GetDictionaries(new Services.Message.GetDictionariesRequest()).dictionaries;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.DictionaryViewModel>>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.DictionaryViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception ex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.DictionaryViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.DictionaryViewModel> Get(string id)
        {
            try{
                Guid dictionaryId = Guid.Empty;
                if(!Guid.TryParse(id, out dictionaryId))
                    return Infrastructure.ApiResponse<Services.ViewModel.DictionaryViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetDictionaryRequest req = new Services.Message.GetDictionaryRequest();
                req.dictionaryId = dictionaryId;
                var dictionary =  _dictionaryService.GetDictionary(req).dictionary; 
                return Infrastructure.ApiResponse<Services.ViewModel.DictionaryViewModel>.CreateResponse(true, "", dictionary);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<Services.ViewModel.DictionaryViewModel>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<Services.ViewModel.DictionaryViewModel>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }            
        }

        [HttpPost]
        public Infrastructure.ApiResponse<Services.ViewModel.DictionaryViewModel> Post([FromBody]Services.ViewModel.DictionaryViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.DictionaryViewModel>();
            try{
                var req = new Services.Message.AddDictionaryRequest();
                req.dictionary = model;

                _dictionaryService.AddDictionary(req);

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
        public Infrastructure.ApiResponse<Services.ViewModel.DictionaryViewModel> Put(string id, [FromBody]Services.ViewModel.DictionaryViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.DictionaryViewModel>();
            try{
                Guid dictionaryId = Guid.Empty;
                if(!Guid.TryParse(id, out dictionaryId))
                    return Infrastructure.ApiResponse<Services.ViewModel.DictionaryViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound);
                
                model.Id = dictionaryId;
                _dictionaryService.UpdateDictionary(new Services.Message.UpdateDictionaryRequest(){ dictionary = model });

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
        public Infrastructure.ApiResponse<Services.ViewModel.DictionaryViewModel> Delete(string id)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.DictionaryViewModel>();
            try{
                Guid dictionaryId = Guid.Empty;
                if(!Guid.TryParse(id, out dictionaryId))
                    return Infrastructure.ApiResponse<Services.ViewModel.DictionaryViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound); 

                _dictionaryService.RemoveDictionary(new Services.Message.RemoveDictionaryRequest(){dictionaryId = dictionaryId});
                
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
