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
    public class AnswersController : Controller
    {
        Services.Interfaces.IAnswearService _answerService;
        
        public AnswersController(Services.Interfaces.IAnswearService answerService){
            _answerService = answerService;
        }

        [HttpGet]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.AnswerViewModel>> Get()
        {
            Console.Write(User.Identity.Name);
            try{
                var data = _answerService.GetAnswearList(new Services.Message.GetAnswearListRequest()).answears;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.AnswerViewModel>>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.AnswerViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.AnswerViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.AnswerViewModel> Get(string id)
        {
            try{
                Guid answerid = Guid.Empty;
                if(!Guid.TryParse(id, out answerid))
                    return Infrastructure.ApiResponse<Services.ViewModel.AnswerViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetAnswearRequest req = new Services.Message.GetAnswearRequest();
                req.answearId = answerid;
                var Answer =  _answerService.GetAnswear(req).answear; 
                return Infrastructure.ApiResponse<Services.ViewModel.AnswerViewModel>.CreateResponse(true, "", Answer);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<Services.ViewModel.AnswerViewModel>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<Services.ViewModel.AnswerViewModel>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }            
        }

        [HttpGet("useranswers/{id}")]
        public Infrastructure.ApiResponse<List<Services.ViewModel.UserAnswerViewModel>> GetUserAnswers(string id)
        {
            try{
                Guid userId = Guid.Empty;
                if(!Guid.TryParse(id, out userId))
                    return Infrastructure.ApiResponse<List<Services.ViewModel.UserAnswerViewModel>>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetUserAnswersRequest req = new Services.Message.GetUserAnswersRequest();
                req.userId = userId;
                var useranswers =  _answerService.GetUserAnswers(req).useranswers.ToList();
                return Infrastructure.ApiResponse<List<Services.ViewModel.UserAnswerViewModel>>.CreateResponse(true, "", useranswers);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<List<Services.ViewModel.UserAnswerViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<List<Services.ViewModel.UserAnswerViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }            
        }

        [HttpPost("userAnswer")]
        public Infrastructure.ApiResponse<bool> Post([FromBody]Services.ViewModel.UserAnswerQuestionViewModel model)
        {
            var response = new Infrastructure.ApiResponse<bool>();
            try{
                var req = new Services.Message.UserAnswerRequest();
                req.model = model;

                _answerService.UserAnswer(req);

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

        [HttpPost]
        public Infrastructure.ApiResponse<Services.ViewModel.AnswerViewModel> Post([FromBody]Services.ViewModel.AnswerViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.AnswerViewModel>();
            try{
                var req = new Services.Message.AddAnswearRequest();
                req.answear = model;

                _answerService.AddAnswear(req);

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
        public Infrastructure.ApiResponse<Services.ViewModel.AnswerViewModel> Put(string id, [FromBody]Services.ViewModel.AnswerViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.AnswerViewModel>();
            try{
                Guid answerid = Guid.Empty;
                if(!Guid.TryParse(id, out answerid))
                    return Infrastructure.ApiResponse<Services.ViewModel.AnswerViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound);
                
                model.Id = answerid;
                _answerService.UpdateAnswear(new Services.Message.UpdateAnswearRequest(){ answear = model });

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
        public Infrastructure.ApiResponse<Services.ViewModel.AnswerViewModel> Delete(string id)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.AnswerViewModel>();
            try{
                Guid answerid = Guid.Empty;
                if(!Guid.TryParse(id, out answerid))
                    return Infrastructure.ApiResponse<Services.ViewModel.AnswerViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound); 

                _answerService.RemoveAnswear(new Services.Message.RemoveAnswearRequest(){answearId = answerid});
                
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