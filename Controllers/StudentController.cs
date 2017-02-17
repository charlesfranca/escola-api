using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EscolaDeVoce.API.Controllers
{
    [Route("api/[controller]")]
    public class StudentController : Controller
    {
        // EscolaContext ctx;

        // // GET api/values
        // [HttpGet]
        // public List<Model.Student> Get()
        // {
        //     EscolaContext ctx = new EscolaContext();

        //     var student = ctx.Students.ToList();
        //     return student;
        // }

        // // GET api/values/5
        // [HttpGet("{id}")]
        // public Model.Student Get(string id)
        // {
        //     EscolaContext ctx = new EscolaContext();

        //     Guid catid = Guid.Empty;
        //     Guid.TryParse(id, out catid); 
        //     var student = ctx.Students.Where(c=>c.Id == catid).FirstOrDefault();
        //     return student;
        // }

        // // POST api/values
        // [HttpPost]
        // public void Post([FromBody]ViewModel.AddStudentViewlModel model)
        // {
        //     EscolaContext ctx = new EscolaContext();

        //     Model.User user = new Model.User();
        //     user.username = model.username;
        //     user.password = model.password;

        //     ctx.Users.Add(user);
        //     ctx.SaveChanges();
        // }

        // // PUT api/values/5
        // [HttpPut("{id}")]
        // public void Put(int id, [FromBody]string value)
        // {

        // }

        // // DELETE api/values/5
        // [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }
    }
}
