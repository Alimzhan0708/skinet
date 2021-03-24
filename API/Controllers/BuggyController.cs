using API.Errors;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly StoreContext _context;
        public BuggyController(StoreContext context)
        {
            _context = context;

        }

        [HttpGet("notfound")]
        public ActionResult GetNotFoundRequest()
        {
            var item =  _context.ProductBrands.Find(42);

            if(item is null)
            {
                return NotFound(new ApiResponse(404));
            }

            return Ok();
        }

        [Authorize]
        [HttpGet("testauth")]
        public ActionResult Authorize()
        {
            return Ok("Super secret");
        }

        [HttpGet("servererror")]
        public ActionResult GetServerError()
        {
            var item =  _context.ProductBrands.Find(42);
            var itemToReturn = item.ToString();

            return Ok();
        }

        [HttpGet("badrequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("badrequest/{id}")]
        public ActionResult GetNotFoundRequest(int id)
        {
            return Ok();
        }
    }
}