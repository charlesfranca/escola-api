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
    public class UserController : BaseController
    {
        Services.Interfaces.IUserService _userService;
        
        public UserController(Services.Interfaces.IUserService userService){
            _userService = userService;
        }

        [HttpGet]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.UserViewModel>> Get()
        {
            try{
                var data = _userService.GetUsers(new Services.Message.GetUsersRequest()).list;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.UserViewModel>>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.UserViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.UserViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.UserViewModel> Get(string id)
        {
            try{
                Guid userid = Guid.Empty;
                if(!Guid.TryParse(id, out userid))
                    return Infrastructure.ApiResponse<Services.ViewModel.UserViewModel>.CreateResponse(false, "Usuário não encontrado", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetUserRequest req = new Services.Message.GetUserRequest();
                req.userid = userid;
                var user =  _userService.GetUser(req).user; 
                return Infrastructure.ApiResponse<Services.ViewModel.UserViewModel>.CreateResponse(true, "", user);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<Services.ViewModel.UserViewModel>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<Services.ViewModel.UserViewModel>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }            
        }

        [HttpGet("info")]
        public Infrastructure.ApiResponse<Services.ViewModel.UserViewModel> GetInfo()
        {
            var id = getClaimValue("id");
            try{
                Guid userid = Guid.Empty;
                if(!Guid.TryParse(id, out userid))
                    return Infrastructure.ApiResponse<Services.ViewModel.UserViewModel>.CreateResponse(false, "Usuário não encontrado", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetUserRequest req = new Services.Message.GetUserRequest();
                req.userid = userid;
                var user =  _userService.GetUser(req).user; 
                return Infrastructure.ApiResponse<Services.ViewModel.UserViewModel>.CreateResponse(true, "", user);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<Services.ViewModel.UserViewModel>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<Services.ViewModel.UserViewModel>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }            
        }

        [AllowAnonymous]
        [HttpPost]
        public Infrastructure.ApiResponse<Services.ViewModel.UserViewModel> Post([FromBody]Services.ViewModel.CreateUserViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.UserViewModel>();
            try{
                var req = new Services.Message.AddUserRequest();
                req.model = model;

                _userService.AddUser(req);

                response.status = true;
                
                Services.ViewModel.UserViewModel u = new Services.ViewModel.UserViewModel();
                u.username = model.username;

                response.data = u;
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

        [HttpPost("saveProfileImage")]
        public Infrastructure.ApiResponse<bool> saveProfileImage([FromBody]Services.ViewModel.UpdateUserImageViewModel model)
        {
            var response = new Infrastructure.ApiResponse<bool>();
            try{
                var req = new Services.Message.ChangeUserProfileImageRequest();
                req.user = model;

                _userService.ChangeUserProfileImage(req);

                response.status = true;
                
                response.data = response.status;
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

        [HttpPost("saveProfileCover")]
        public Infrastructure.ApiResponse<bool> saveProfileCover([FromBody]Services.ViewModel.UpdateUserCoverViewModel model)
        {
            var response = new Infrastructure.ApiResponse<bool>();
            try{
                var req = new Services.Message.ChangeUserProfileCoverRequest();
                req.user = model;
                _userService.ChangeUserProfileCover(req);
                
                response.status = true;                
                response.data = response.status;
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
        public Infrastructure.ApiResponse<Services.ViewModel.UserViewModel> Put(string id, [FromBody]Services.ViewModel.UserViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.UserViewModel>();
            try{
                Guid userid = Guid.Empty;
                if(!Guid.TryParse(id, out userid))
                    return Infrastructure.ApiResponse<Services.ViewModel.UserViewModel>.CreateResponse(false, "Usuário não encontrado", null, System.Net.HttpStatusCode.NotFound);
                
                model.Id = userid;
                _userService.UpdateUser(new Services.Message.UpdateUserRequest(){ user = model });

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
        public Infrastructure.ApiResponse<Services.ViewModel.UserViewModel> Delete(string id)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.UserViewModel>();
            try{
                Guid userid = Guid.Empty;
                if(!Guid.TryParse(id, out userid))
                    return Infrastructure.ApiResponse<Services.ViewModel.UserViewModel>.CreateResponse(false, "Usuário não encontrada", null, System.Net.HttpStatusCode.NotFound); 

                _userService.RemoveUser(new Services.Message.RemoveUserRequest(){userid = userid});
                
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
