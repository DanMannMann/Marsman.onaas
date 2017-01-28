using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace Marsman.onaas.Controllers
{
	public class OhNoController : ApiController
	{
		public OhNoController() { }

		public OhNoController(bool test) { Test = test; }

		public bool Test { get; private set; }

		// GET api/values
		public HttpResponseMessage Get(string activity, string error)
		{
			//if (activity.Length > )

			try
			{
				using (var bitmap = (Bitmap)Image.FromFile(System.Web.Hosting.HostingEnvironment.MapPath("~/template.png")))
				{
					using (var collection = new PrivateFontCollection())
					{
						collection.AddFontFile(System.Web.Hosting.HostingEnvironment.MapPath("~/gapstown-b.ttf"));
						collection.AddFontFile(System.Web.Hosting.HostingEnvironment.MapPath("~/gapstown-r.ttf"));
						using (var graphics = Graphics.FromImage(bitmap))
						using (var font = new Font(collection.Families.First(), 26, FontStyle.Bold))
						using (var fontsm = new Font(collection.Families.First(), 22, FontStyle.Bold))
						{
							graphics.DrawString(activity, font, Brushes.Black, new PointF(40, 20));
							graphics.DrawString(error, fontsm, Brushes.Maroon, new PointF(430, 156));
						}
					}

					using (MemoryStream memoryStream = new MemoryStream())
					{
						bitmap.Save(memoryStream, ImageFormat.Png);
						var response = new HttpResponseMessage(HttpStatusCode.OK);
						response.Content = new ByteArrayContent(memoryStream.ToArray());
						response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
						return response;
					}
				}
			}
			catch
			{
				if (Test) throw;
				return Get("Making an oh no!", "OH NO!");
			}
		}
	}

}
