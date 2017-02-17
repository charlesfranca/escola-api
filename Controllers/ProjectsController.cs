using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EscolaDeVoce.API.Controllers
{
    [Route("api/[controller]")]
    public class ProjectsController : Controller
    {
        Services.Interfaces.IProjectService _projectService;
        
        public ProjectsController(Services.Interfaces.IProjectService projetoService){
            _projectService = new Services.ProjectService();
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
        public Infrastructure.ApiResponse<IList<Services.ViewModel.ProjectViewModel>> Get()
        {
            try{
                var data = _projectService.GetProjects(new Services.Message.GetProjectsRequest()).projects;
                return CreateResponse<IList<Services.ViewModel.ProjectViewModel>>(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return CreateResponse<IList<Services.ViewModel.ProjectViewModel>>(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return CreateResponse<IList<Services.ViewModel.ProjectViewModel>>(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.ProjectViewModel> Get(string id)
        {
            try{
                Guid projectid = Guid.Empty;
                if(!Guid.TryParse(id, out projectid))
                    return CreateResponse<Services.ViewModel.ProjectViewModel>(false, "Projeto não encontrado", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetProjectRequest req = new Services.Message.GetProjectRequest();
                req.projectId = projectid;
                var project =  _projectService.GetProject(req).project; 
                return CreateResponse<Services.ViewModel.ProjectViewModel>(true, "", project);
            }catch(Infrastructure.BusinessRuleException bex){
                return CreateResponse<Services.ViewModel.ProjectViewModel>(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return CreateResponse<Services.ViewModel.ProjectViewModel>(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }            
        }

        [HttpPost]
        public Infrastructure.ApiResponse<Services.ViewModel.ProjectViewModel> Post([FromBody]Services.ViewModel.ProjectViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.ProjectViewModel>();
            try{
                var req = new Services.Message.AddProjectRequest();
                req.project = model;

                _projectService.AddProject(req);

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
        public Infrastructure.ApiResponse<Services.ViewModel.ProjectViewModel> Put(string id, [FromBody]Services.ViewModel.ProjectViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.ProjectViewModel>();
            try{
                Guid projectid = Guid.Empty;
                if(!Guid.TryParse(id, out projectid))
                    return CreateResponse<Services.ViewModel.ProjectViewModel>(false, "projeto não encontrado", null, System.Net.HttpStatusCode.NotFound);
                
                model.Id = projectid;
                _projectService.UpdateProject(new Services.Message.UpdateProjectRequest(){ project = model });

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
        public Infrastructure.ApiResponse<Services.ViewModel.ProjectViewModel> Delete(string id)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.ProjectViewModel>();
            try{
                Guid projectid = Guid.Empty;
                if(!Guid.TryParse(id, out projectid))
                    return CreateResponse<Services.ViewModel.ProjectViewModel>(false, "Projeto não encontrado", null, System.Net.HttpStatusCode.NotFound); 

                _projectService.RemoveProject(new Services.Message.DeleteProjectRequest(){projectid = projectid});
                
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
