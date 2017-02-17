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
    public class CategoriasController : Controller
    {
        Services.Interfaces.ICategoryService _categoriaService;
        
        public CategoriasController(Services.Interfaces.ICategoryService categoriaService){
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.CategoryViewModel>> Get()
        {
            Console.Write(User.Identity.Name);
            try{
                var data = _categoriaService.GetCategories(new Services.Message.GetCategoriesRequest()).categories;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.CategoryViewModel>>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.CategoryViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception ex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.CategoryViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.CategoryViewModel> Get(string id)
        {
            try{
                Guid catid = Guid.Empty;
                if(!Guid.TryParse(id, out catid))
                    return Infrastructure.ApiResponse<Services.ViewModel.CategoryViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetCategoryRequest req = new Services.Message.GetCategoryRequest();
                req.CategoryId = catid;
                var category =  _categoriaService.GetCategory(req).category; 
                return Infrastructure.ApiResponse<Services.ViewModel.CategoryViewModel>.CreateResponse(true, "", category);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<Services.ViewModel.CategoryViewModel>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<Services.ViewModel.CategoryViewModel>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }            
        }

        [HttpPost]
        public Infrastructure.ApiResponse<Services.ViewModel.CategoryViewModel> Post([FromBody]Services.ViewModel.CategoryViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.CategoryViewModel>();
            try{
                var req = new Services.Message.AddCategoryRequest();
                req.category = model;

                _categoriaService.AddCategory(req);

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
        public Infrastructure.ApiResponse<Services.ViewModel.CategoryViewModel> Put(string id, [FromBody]Services.ViewModel.CategoryViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.CategoryViewModel>();
            try{
                Guid catid = Guid.Empty;
                if(!Guid.TryParse(id, out catid))
                    return Infrastructure.ApiResponse<Services.ViewModel.CategoryViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound);
                
                model.Id = catid;
                _categoriaService.UpdateCategory(new Services.Message.UpdateCategoryRequest(){ category = model });

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
        public Infrastructure.ApiResponse<Services.ViewModel.CategoryViewModel> Delete(string id)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.CategoryViewModel>();
            try{
                Guid catid = Guid.Empty;
                if(!Guid.TryParse(id, out catid))
                    return Infrastructure.ApiResponse<Services.ViewModel.CategoryViewModel>.CreateResponse(false, "Categoria não encontrada", null, System.Net.HttpStatusCode.NotFound); 

                _categoriaService.RemoveCategory(new Services.Message.RemoveCategoryRequest(){catid = catid});
                
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
