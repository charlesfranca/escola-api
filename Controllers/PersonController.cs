using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EscolaDeVoce.API.Controllers
{
    [Route("api/[controller]")]
    public class PersonController : Controller
    {
        Services.Interfaces.IPersonService _personService;
        
        public PersonController(Services.Interfaces.IPersonService personService){
            _personService = personService;
        }

        [HttpGet]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.PersonViewModel>> Get()
        {
            try{
                var data = _personService.GetPeople(new Services.Message.GetPeopleRequest()).people;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.PersonViewModel>>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.PersonViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.PersonViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("embaixadoras")]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.EmbaixadoraViewModel>> GetEmbaixadoras()
        {
            try{
                var data = _personService.GetEmbaixadoras(new Services.Message.GetEmbaixadorasRequest()).person;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.EmbaixadoraViewModel>>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.EmbaixadoraViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.EmbaixadoraViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.PersonViewModel> Get(string id)
        {
            try{
                Guid personid = Guid.Empty;
                if(!Guid.TryParse(id, out personid))
                    return Infrastructure.ApiResponse<Services.ViewModel.PersonViewModel>.CreateResponse(false, "Curso não encontrado", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetPersonRequest req = new Services.Message.GetPersonRequest();
                req.personid = personid;
                var Person =  _personService.GetPerson(req).person; 
                return Infrastructure.ApiResponse<Services.ViewModel.PersonViewModel>.CreateResponse(true, "", Person);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<Services.ViewModel.PersonViewModel>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<Services.ViewModel.PersonViewModel>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }            
        }

        [HttpPost]
        public Infrastructure.ApiResponse<Services.ViewModel.PersonViewModel> Post([FromBody]Services.ViewModel.PersonViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.PersonViewModel>();
            try{
                var req = new Services.Message.AddPersonRequest();
                req.person = model;

                _personService.AddPerson(req);

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
        public Infrastructure.ApiResponse<Services.ViewModel.PersonViewModel> Put(string id, [FromBody]Services.ViewModel.PersonViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.PersonViewModel>();
            try{
                Guid personid = Guid.Empty;
                if(!Guid.TryParse(id, out personid))
                    return Infrastructure.ApiResponse<Services.ViewModel.PersonViewModel>.CreateResponse(false, "Curso não encontrado", null, System.Net.HttpStatusCode.NotFound);
                
                model.Id = personid;
                _personService.UpdatePerson(new Services.Message.UpdatePersonRequest(){ person = model });

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

        [HttpPut("makeembaixadora/{id}")]
        public Infrastructure.ApiResponse<bool> MakeEmbaixadora(string id)
        {
            var response = new Infrastructure.ApiResponse<bool>();
            try{
                Guid personid = Guid.Empty;
                if(!Guid.TryParse(id, out personid))
                    return Infrastructure.ApiResponse<bool>.CreateResponse(false, "Curso não encontrado", false, System.Net.HttpStatusCode.NotFound);
                
                bool std = _personService.MakeEmbaixadora(new Services.Message.MakeEmbaixadoraRequest(){ personId = personid }).status;

                response.status = true;
                response.data = std;
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
        public Infrastructure.ApiResponse<Services.ViewModel.PersonViewModel> Delete(string id)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.PersonViewModel>();
            try{
                Guid personid = Guid.Empty;
                if(!Guid.TryParse(id, out personid))
                    return Infrastructure.ApiResponse<Services.ViewModel.PersonViewModel>.CreateResponse(false, "Curso não encontrada", null, System.Net.HttpStatusCode.NotFound); 

                _personService.RemovePerson(new Services.Message.RemovePersonRequest(){personid = personid});
                
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
