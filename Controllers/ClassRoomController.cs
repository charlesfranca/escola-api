using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EscolaDeVoce.API.Controllers
{
    [Route("api/[controller]")]
    public class ClassRoomController : Controller
    {
        Services.Interfaces.IClassRoomService _classRoomService;
        
        public ClassRoomController(Services.Interfaces.IClassRoomService classRoomService){
            _classRoomService = classRoomService;
        }

        [HttpGet]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.ClassRoomViewModel>> Get()
        {
            try{
                var data = _classRoomService.GetClassRooms(new Services.Message.GetClassRoomsRequest()).classRooms;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.ClassRoomViewModel>>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.ClassRoomViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.ClassRoomViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public Infrastructure.ApiResponse<Services.ViewModel.ClassRoomViewModel> Get(string id)
        {
            try{
                Guid classRoomid = Guid.Empty;
                if(!Guid.TryParse(id, out classRoomid))
                    return Infrastructure.ApiResponse<Services.ViewModel.ClassRoomViewModel>.CreateResponse(false, "Curso não encontrado", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetClassRoomRequest req = new Services.Message.GetClassRoomRequest();
                req.classRoomid = classRoomid;
                var classRoom =  _classRoomService.GetClassRoom(req).classRoom; 
                return Infrastructure.ApiResponse<Services.ViewModel.ClassRoomViewModel>.CreateResponse(true, "", classRoom);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<Services.ViewModel.ClassRoomViewModel>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception){
                return Infrastructure.ApiResponse<Services.ViewModel.ClassRoomViewModel>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }            
        }

        [HttpPost]
        public Infrastructure.ApiResponse<Services.ViewModel.ClassRoomViewModel> Post([FromBody]Services.ViewModel.ClassRoomViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.ClassRoomViewModel>();
            try{
                var req = new Services.Message.AddClassRoomRequest();
                req.classRoom = model;

                _classRoomService.AddClassRoom(req);

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
        public Infrastructure.ApiResponse<Services.ViewModel.ClassRoomViewModel> Put(string id, [FromBody]Services.ViewModel.ClassRoomViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.ClassRoomViewModel>();
            try{
                Guid classRoomid = Guid.Empty;
                if(!Guid.TryParse(id, out classRoomid))
                    return Infrastructure.ApiResponse<Services.ViewModel.ClassRoomViewModel>.CreateResponse(false, "Curso não encontrado", null, System.Net.HttpStatusCode.NotFound);
                
                model.Id = classRoomid;
                _classRoomService.UpdateClassRoom(new Services.Message.UpdateClassRoomRequest(){ classRoom = model });

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
        public Infrastructure.ApiResponse<Services.ViewModel.ClassRoomViewModel> Delete(string id)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.ClassRoomViewModel>();
            try{
                Guid classRoomid = Guid.Empty;
                if(!Guid.TryParse(id, out classRoomid))
                    return Infrastructure.ApiResponse<Services.ViewModel.ClassRoomViewModel>.CreateResponse(false, "Curso não encontrada", null, System.Net.HttpStatusCode.NotFound); 

                _classRoomService.RemoveClassRoom(new Services.Message.RemoveClassRoomRequest(){classRoomid = classRoomid});
                
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
