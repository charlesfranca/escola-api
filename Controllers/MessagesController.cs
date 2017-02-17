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
    public class MessagesController : Controller
    {
        Services.Interfaces.IMessageService _messageService;
        
        public MessagesController(Services.Interfaces.IMessageService messageService){
            _messageService = messageService;
        }

        [HttpGet("userMessages/{fromId}/{toId}")]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.MessageViewModel>> GetUserToUser(string fromId, string toId)
        {
            Console.Write(User.Identity.Name);
            var request = new Services.Message.GetUserToUserMessageListRequest();
            request.Message = new Services.ViewModel.MessageUserToUserViewModel();
            request.Message.fromId = Guid.Parse(fromId);
            request.Message.toId = Guid.Parse(toId);
            try{
                var data = _messageService.GetMessagesUserToUser(request).messages;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.MessageViewModel>>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.MessageViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.MessageViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.MessageViewModel>> Get()
        {
            Console.Write(User.Identity.Name);
            try{
                var data = _messageService.GetMessages(new Services.Message.GetMessageListResponse()).messages;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.MessageViewModel>>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.MessageViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.MessageViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.MessageViewModel> Get(string id)
        {
            try{
                Guid messageid = Guid.Empty;
                if(!Guid.TryParse(id, out messageid))
                    return Infrastructure.ApiResponse<Services.ViewModel.MessageViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetMessageRequest req = new Services.Message.GetMessageRequest();
                req.messageid = messageid;
                var message =  _messageService.GetMessage(req).message; 
                return Infrastructure.ApiResponse<Services.ViewModel.MessageViewModel>.CreateResponse(true, "", message);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<Services.ViewModel.MessageViewModel>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<Services.ViewModel.MessageViewModel>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }            
        }


        [HttpPost]
        public Infrastructure.ApiResponse<Services.ViewModel.MessageViewModel> Post([FromBody]Services.ViewModel.MessageViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.MessageViewModel>();
            try{
                var req = new Services.Message.AddMessageRequest();
                req.message = model;

                _messageService.AddMessage(req);

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
        public Infrastructure.ApiResponse<Services.ViewModel.MessageViewModel> Put(string id, [FromBody]Services.ViewModel.MessageViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.MessageViewModel>();
            try{
                Guid messageid = Guid.Empty;
                if(!Guid.TryParse(id, out messageid))
                    return Infrastructure.ApiResponse<Services.ViewModel.MessageViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound);
                
                model.Id = messageid;
                _messageService.UpdateMessage(new Services.Message.UpdateMessageRequest(){ message = model });

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
        public Infrastructure.ApiResponse<Services.ViewModel.MessageViewModel> Delete(string id)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.MessageViewModel>();
            try{
                Guid messageid = Guid.Empty;
                if(!Guid.TryParse(id, out messageid))
                    return Infrastructure.ApiResponse<Services.ViewModel.MessageViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound); 

                _messageService.RemoveMessage(new Services.Message.RemoveMessageRequest(){messageId = messageid});
                
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