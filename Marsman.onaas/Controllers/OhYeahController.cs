using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;

namespace Marsman.onaas.Controllers
{

	public class OhYeahController : ApiController
	{
		// GET api/values
		public async Task<HttpResponseMessage> Get(string imageUri, float scale = 0.8f, string scaleAxis = "y", float centreX = 0.5f, float centreY = 0.5f)
		{
			try
			{
				using (var cli = new HttpClient())
				using (var imageResponse = await cli.GetStreamAsync(imageUri))
				using (var targetBmp = (Bitmap)Image.FromStream(imageResponse))
				{
					using (var ohYeahBmp = (Bitmap)Image.FromFile(System.Web.Hosting.HostingEnvironment.MapPath("~/ohyeah.png")))
					using (var targetGfx = Graphics.FromImage(targetBmp))
					{
						targetGfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
						ohYeahBmp.MakeTransparent();
						var targetRectangle = GetTargetRectangle(scale, scaleAxis, targetBmp, ohYeahBmp, centreX, centreY);
						targetGfx.DrawImage(ohYeahBmp, targetRectangle);
					}

					using (var memoryStream = new MemoryStream())
					{
						targetBmp.Save(memoryStream, ImageFormat.Png);
						var response = new HttpResponseMessage(HttpStatusCode.OK);
						response.Content = new ByteArrayContent(memoryStream.ToArray());
						response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
						return response;
					}
				}
			}
			catch
			{
				return new OhNoController().Get("Making an oh yeah!", "OH NO!");
			}
		}

		private static Rectangle GetTargetRectangle(float scale, string centre, Bitmap targetBmp, Bitmap ohYeahBmp, float centreX, float centreY)
		{
			int x, y, overlayWidth, overlayHeight;
			float ratio;
			switch (centre)
			{
				case "x":
					overlayWidth = (int)(scale * (float)targetBmp.Width);
					ratio = (float)overlayWidth / (float)ohYeahBmp.Width;
					overlayHeight = (int)((float)ohYeahBmp.Height * ratio);
					
					break;

				case "y":
					overlayHeight = (int)(scale * (float)targetBmp.Height);
					ratio = (float)overlayHeight / (float)ohYeahBmp.Height;
					overlayWidth = (int)((float)ohYeahBmp.Width * ratio);
					break;

				default:
					throw new ArgumentException();
			}

			x = (int)((targetBmp.Width - overlayWidth) * centreX);
			y = (int)((targetBmp.Height - overlayHeight) * centreY);
			return new Rectangle(x, y, overlayWidth, overlayHeight);
		}
	}

	public class OhYeahWebController : ApiController
	{
		[System.Web.Http.HttpGet]
		public HttpResponseMessage Web(string uri)
		{
			var body = RenderViewToString("OhYeahWeb", "~/Views/OhYeahWeb.cshtml", uri);
			var c = Request.CreateResponse(HttpStatusCode.OK);
			c.Content = new StringContent(body);
			c.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
			return c;
		}

		private string RenderViewToString(string controllerName, string viewName, object viewData)
		{
			using (var writer = new StringWriter())
			{
				var routeData = new RouteData();
				routeData.Values.Add("controller", controllerName);
				var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new FakeController());
				var razorViewEngine = new RazorViewEngine();
				var razorViewResult = razorViewEngine.FindView(fakeControllerContext, viewName, "", false);

				var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(viewData), new TempDataDictionary(), writer);
				razorViewResult.View.Render(viewContext, writer);
				return writer.ToString();
			}
		}
	}

	public class FakeController : ControllerBase { protected override void ExecuteCore() { } }
}