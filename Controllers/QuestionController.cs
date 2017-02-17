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
    public class QuestionController : Controller
    {
        Services.Interfaces.IPersonalityQuestionService _personalityQuestionService;
        
        public QuestionController(Services.Interfaces.IPersonalityQuestionService personalityQuestionService){
            _personalityQuestionService = personalityQuestionService;
        }

        [HttpGet]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.PersonalityQuestionViewModel>> Get()
        {
            Console.Write(User.Identity.Name);
            try{
                var data = _personalityQuestionService.GetPersonalityQuestionList(new Services.Message.GetPersonalityQuestionListRequest()).personalityQuestions;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.PersonalityQuestionViewModel>>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.PersonalityQuestionViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception ex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.PersonalityQuestionViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("notAnsweredQuestions/{id}")]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.PersonalityQuestionViewModel>> GetUserNotAnsweredQuestion(string id)
        {
            Console.Write(User.Identity.Name);
            try{
                var request = new Services.Message.GetNotAnswearedQuestionsRequest();
                request.model = new Services.ViewModel.NotAnsweredQuestionViewModel();
                request.model.userId = Guid.Parse(id);

                var data = _personalityQuestionService.GetUserNotAnswerdQuestions(request).questions;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.PersonalityQuestionViewModel>>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.PersonalityQuestionViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception ex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.PersonalityQuestionViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("nextQuestion/{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel> GetNextQuestion(string id)
        {
            Console.Write(User.Identity.Name);
            try{
                var request = new Services.Message.GetNotAnswearedQuestionsRequest();
                request.model = new Services.ViewModel.NotAnsweredQuestionViewModel();
                request.model.userId = Guid.Parse(id);

                var data = _personalityQuestionService.GetNextUserNotAnswerdQuestions(request).questions;
                return Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception ex){
                return Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel> Get(string id)
        {
            try{
                Guid questionId = Guid.Empty;
                if(!Guid.TryParse(id, out questionId))
                    return Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetPersonalityQuestionRequest req = new Services.Message.GetPersonalityQuestionRequest();
                req.personalityQuestionid = questionId;
                var personalityQuestion =  _personalityQuestionService.GetPersonalityQuestion(req).personalityQuestion; 
                return Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel>.CreateResponse(true, "", personalityQuestion);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }            
        }

        [HttpPost]
        public Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel> Post([FromBody]Services.ViewModel.PersonalityQuestionViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel>();
            try{
                var req = new Services.Message.AddPersonalityQuestionRequest();
                req.personalityQuestion = model;

                _personalityQuestionService.AddPersonalityQuestion(req);

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
        public Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel> Put(string id, [FromBody]Services.ViewModel.PersonalityQuestionViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel>();
            try{
                Guid questionId = Guid.Empty;
                if(!Guid.TryParse(id, out questionId))
                    return Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound);
                
                model.Id = questionId;
                _personalityQuestionService.UpdatePersonalityQuestion(new Services.Message.UpdatePersonalityQuestionRequest(){ personalityQuestion = model });

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
        public Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel> Delete(string id)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel>();
            try{
                Guid questionId = Guid.Empty;
                if(!Guid.TryParse(id, out questionId))
                    return Infrastructure.ApiResponse<Services.ViewModel.PersonalityQuestionViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound); 

                _personalityQuestionService.RemovePersonalityQuestion(new Services.Message.RemovePersonalityQuestionRequest(){personalityQuestionid = questionId});
                
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
