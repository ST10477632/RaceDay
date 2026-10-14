using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Database;
using RaceDay.Models;
using System.Text.Json;

namespace RaceDay.Controllers
{//start of namespace
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {//start of class


        [HttpGet]
        public string get_categories()
        {//start of get_categories
            Manage_Categories category_data = new Manage_Categories();
            return JsonSerializer.Serialize(category_data.get_all());
        }//end of get_categories

        
        [HttpGet("{id:int}")]
        public string get_category(int id)
        {//start of get_category
            Manage_Categories category_data = new Manage_Categories();
            Category? category = category_data.get_category(id);

            if (category == null)
            {
                Response.StatusCode = 404;
                return "Category does not exist...";
            }
            return JsonSerializer.Serialize(category);
        }//end of get_category

        
        [HttpPost]
        public string create_category(string category_name, string? description = null)
        {//start of create_category
            string denied = check_access.check(HttpContext, "Organiser");
            if (denied != "") return denied;

            if (category_name.Trim() == "")
            {
                Response.StatusCode = 400;
                return "Category name is required...";
            }

            Manage_Categories category_data = new Manage_Categories();
            string message = category_data.add_category(category_name.Trim(), description);

            if (message.Contains("successfully")) Response.StatusCode = 201;
            else Response.StatusCode = 400;

            return message;
        }//end of create_category

        
        [HttpPut("{id:int}")]
        public string update_category(int id, string category_name, string? description = null)
        {//start of update_category
            string denied = check_access.check(HttpContext, "Organiser");
            if (denied != "") return denied;

            Manage_Categories category_data = new Manage_Categories();
            if (category_data.get_category(id) == null)
            {
                Response.StatusCode = 404;
                return "Category does not exist...";
            }

            if (category_name.Trim() == "")
            {
                Response.StatusCode = 400;
                return "Category name is required...";
            }

            string message = category_data.update_category(id, category_name.Trim(), description);

            if (message.Contains("successfully")) Response.StatusCode = 200;
            else Response.StatusCode = 400;

            return message;
        }//end of update_category

        
        [HttpDelete("{id:int}")]
        public string delete_category(int id)
        {//start of delete_category
            string denied = check_access.check(HttpContext, "Organiser");
            if (denied != "") return denied;

            Manage_Categories category_data = new Manage_Categories();
            if (category_data.get_category(id) == null)
            {
                Response.StatusCode = 404;
                return "Category does not exist...";
            }

            if (category_data.in_use(id) > 0)
            {
                Response.StatusCode = 409;
                return "Category is currently being used...";
            }

            string message = category_data.delete_category(id);

            if (message.Contains("successfully"))
            {
                Response.StatusCode = 204;
                return "";
            }
            Response.StatusCode = 400;
            return message;
        }//end of delete_category


    }//end of class
}//end of namespace
