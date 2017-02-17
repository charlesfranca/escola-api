using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EscolaDeVoce.API.Controllers
{
    [Route("api/[controller]")]
    public class NewsController : Controller
    {
        Services.Interfaces.INewsService _newsService;
        
        public NewsController(Services.Interfaces.INewsService newsService){
            _newsService = newsService;
        }

        [HttpGet]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.NewsViewModel>> Get()
        {
            try{
                var data = _newsService.GetNewsList(new Services.Message.GetNewsListRequest()).news;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.NewsViewModel>>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.NewsViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.NewsViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.NewsViewModel> Get(string id)
        {
            try{
                Guid newsid = Guid.Empty;
                if(!Guid.TryParse(id, out newsid))
                    return Infrastructure.ApiResponse<Services.ViewModel.NewsViewModel>.CreateResponse(false, "Noticia não encontrada", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetNewsRequest req = new Services.Message.GetNewsRequest();
                req.newsid = newsid;
                var news =  _newsService.GetNews(req).news; 
                return Infrastructure.ApiResponse<Services.ViewModel.NewsViewModel>.CreateResponse(true, "", news);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<Services.ViewModel.NewsViewModel>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<Services.ViewModel.NewsViewModel>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }            
        }

        [HttpPost]
        public Infrastructure.ApiResponse<Services.ViewModel.NewsViewModel> Post([FromBody]Services.ViewModel.NewsViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.NewsViewModel>();
            try{
                var req = new Services.Message.AddNewsRequest();
                req.news = model;

                _newsService.AddNews(req);

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
        public Infrastructure.ApiResponse<Services.ViewModel.NewsViewModel> Put(string id, [FromBody]Services.ViewModel.NewsViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.NewsViewModel>();
            try{
                Guid newsid = Guid.Empty;
                if(!Guid.TryParse(id, out newsid))
                    return Infrastructure.ApiResponse<Services.ViewModel.NewsViewModel>.CreateResponse(false, "Noticia não encontrada", null, System.Net.HttpStatusCode.NotFound);
                
                model.Id = newsid;
                _newsService.UpdateNews(new Services.Message.UpdateNewsRequest(){ news = model });

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
        public Infrastructure.ApiResponse<Services.ViewModel.NewsViewModel> Delete(string id)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.NewsViewModel>();
            try{
                Guid newsid = Guid.Empty;
                if(!Guid.TryParse(id, out newsid))
                    return Infrastructure.ApiResponse<Services.ViewModel.NewsViewModel>.CreateResponse(false, "Noticia não encontrada", null, System.Net.HttpStatusCode.NotFound); 

                _newsService.RemoveNews(new Services.Message.RemoveNewsRequest(){newsid = newsid});
                
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
