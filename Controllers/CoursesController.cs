using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EscolaDeVoce.API.Controllers
{
    [Route("api/[controller]")]
    public class CoursesController : Controller
    {
        Services.Interfaces.ICourseService _courseService;
        
        public CoursesController(Services.Interfaces.ICourseService courseService){
            _courseService = courseService;
        }

        [HttpGet("{id?}")]
        public Infrastructure.ApiResponse<IList<Services.ViewModel.CourseViewModel>> Get(string id)
        {
            try{
                Guid userid = Guid.Empty;
                Guid.TryParse(id, out userid);

                var request = new Services.Message.GetCoursesRequest();
                request.userId = userid;
                var data = _courseService.GetCourses(request).courses;
                return Infrastructure.ApiResponse<IList<Services.ViewModel.CourseViewModel>>.CreateResponse(true, "", data);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.CourseViewModel>>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception ex){
                return Infrastructure.ApiResponse<IList<Services.ViewModel.CourseViewModel>>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("details/{id}/{userid?}")]
        public Infrastructure.ApiResponse<Services.ViewModel.CourseViewModel> Get(string id, string userid)
        {
            try{
                Guid userexternalid = Guid.Empty;
                Guid.TryParse(userid, out userexternalid);

                Guid courseid = Guid.Empty;
                if(!Guid.TryParse(id, out courseid))
                    return Infrastructure.ApiResponse<Services.ViewModel.CourseViewModel>.CreateResponse(false, "Curso não encontrado", null, System.Net.HttpStatusCode.NotFound);

                Services.Message.GetCourseRequest req = new Services.Message.GetCourseRequest();
                req.courseid = courseid;
                req.userId = userexternalid;
                var course =  _courseService.GetCourse(req).course; 
                return Infrastructure.ApiResponse<Services.ViewModel.CourseViewModel>.CreateResponse(true, "", course);
            }catch(Infrastructure.BusinessRuleException bex){
                return Infrastructure.ApiResponse<Services.ViewModel.CourseViewModel>.CreateResponse(false, bex.Message, null, System.Net.HttpStatusCode.BadRequest, bex.BrokenRules);
            }catch(Exception ex){
                return Infrastructure.ApiResponse<Services.ViewModel.CourseViewModel>.CreateResponse(false, "Ocorreu um erro inesperado. Entre em contato com o nosso time de desenvolvimento.", null, System.Net.HttpStatusCode.InternalServerError);
            }            
        }

        [HttpPost]
        public Infrastructure.ApiResponse<Services.ViewModel.CourseViewModel> Post([FromBody]Services.ViewModel.CourseViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.CourseViewModel>();
            try{
                var req = new Services.Message.AddCourseRequest();
                req.course = model;

                _courseService.AddCourse(req);

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
        public Infrastructure.ApiResponse<Services.ViewModel.CourseViewModel> Put(string id, [FromBody]Services.ViewModel.CourseViewModel model)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.CourseViewModel>();
            try{
                Guid courseid = Guid.Empty;
                if(!Guid.TryParse(id, out courseid))
                    return Infrastructure.ApiResponse<Services.ViewModel.CourseViewModel>.CreateResponse(false, "Curso não encontrado", null, System.Net.HttpStatusCode.NotFound);
                
                model.Id = courseid;
                _courseService.UpdateCourse(new Services.Message.UpdateCourseRequest(){ course = model });

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
        public Infrastructure.ApiResponse<Services.ViewModel.CourseViewModel> Delete(string id)
        {
            var response = new Infrastructure.ApiResponse<Services.ViewModel.CourseViewModel>();
            try{
                Guid courseid = Guid.Empty;
                if(!Guid.TryParse(id, out courseid))
                    return Infrastructure.ApiResponse<Services.ViewModel.CourseViewModel>.CreateResponse(false, "Curso não encontrada", null, System.Net.HttpStatusCode.NotFound); 

                _courseService.RemoveCourse(new Services.Message.RemoveCourseRequest(){courseid = courseid});
                
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
