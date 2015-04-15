using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TextPostApi.Models;
using TextUtils;
using PersistenceManager;

namespace TextPostApi.Controllers
{
    /// <summary>
    /// Controlador encargado de procesar los post.
    /// </summary>
    public class TextPostController : ApiController
    {

        // GET: api/TextPost
        public PostText Get()
        {
            return new PostText() { Content = "Hello" };
        }

        //POST: api/TextPost Manejador de las solicitudes de envio de textos de post
        public  HttpResponseMessage Post(PostText ptext)
        {
            if (ModelState.IsValid)
            {

                string statistics = TextProcesor.ProcesTextSingleRun(ptext.Content);
                PersistenceMgr.Instance.ProcessPostData(statistics);
                return Request.CreateResponse();
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
    }
}
