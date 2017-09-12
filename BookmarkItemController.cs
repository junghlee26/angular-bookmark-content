namespace Sabio.Web.Controllers.Api
{
    [RoutePrefix("api/bookmark")]
    public class BookmarkItemController : ApiController
    {
        readonly BookmarkService bookmarkService;
        readonly IAuthenticationService authService;

        public BookmarkItemController(BookmarkService bookmarkService, IAuthenticationService authService)
        {
            this.bookmarkService = bookmarkService;
            this.authService = authService;
        }


        // POST (api/bookmark)
        [Route(), HttpPost]
        public HttpResponseMessage Insert(BookmarkAddRequest model)
        {
            var user = authService.GetCurrentUser();

            if (user == null)
                return Request.CreateResponse(HttpStatusCode.Forbidden, "not logged in");

            model.UserId = user.Id;

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = bookmarkService.Insert(model);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        // DELETE (api/bookmark/{content-id})
        [Route("{contentId:int}"), HttpDelete]
        public HttpResponseMessage DeleteByContentId(int contentId)
        {
            SuccessResponse response = new SuccessResponse();
            var user = authService.GetCurrentUser();
            if (user == null)
                return Request.CreateResponse(HttpStatusCode.Forbidden, "not logged in");


            int userId = user.Id;

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            bookmarkService.DeleteByContentId(userId, contentId);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        //GET(api/bookmark/user)
        [Route("user"), HttpGet]
        public HttpResponseMessage GetByUserId(int itemNum, int pageNum)
        {
            ItemsResponse<Bookmark> response = new ItemsResponse<Bookmark>();
            var user = authService.GetCurrentUser();
            if (user == null)
                return Request.CreateResponse(HttpStatusCode.Forbidden, "not logged in");

            int userId = user.Id;

            response.Items = bookmarkService.GetByUserId(userId, itemNum, pageNum);

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        // GET (api/bookmark/check) MULTIPLEXING 
        [Route("check"), HttpPost]
        public HttpResponseMessage CheckBookmarkRequest(CheckBookmark model)
        {
            ItemResponse<Dictionary<int, Bookmark>> response = new ItemResponse<Dictionary<int, Bookmark>>();

            var user = authService.GetCurrentUser();
            if (user == null)
                return Request.CreateResponse(HttpStatusCode.Forbidden, "not logged in");

            int userId = user.Id;

            response.Item = bookmarkService.GetBookmarkRequest(userId, model.ContentIds.Distinct().ToArray());

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

    }
}
