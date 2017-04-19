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
	public class LabQuoteController : ApiController
	{
		public LabQuoteController() { }

		public LabQuoteController(bool test) { Test = test; }

		public bool Test { get; private set; }

		// GET api/values
		public HttpResponseMessage Get(string source, string quote)
		{
			try
			{
				using (var bitmap = (Bitmap)Image.FromFile(System.Web.Hosting.HostingEnvironment.MapPath("~/labquotetemplate.png")))
				{
					using (var collection = new PrivateFontCollection())
					{
						collection.AddFontFile(System.Web.Hosting.HostingEnvironment.MapPath("~/arial.ttf"));
						collection.AddFontFile(System.Web.Hosting.HostingEnvironment.MapPath("~/ariali.ttf"));
						using (var graphics = Graphics.FromImage(bitmap))
						{
							var size = 56;
							var scaled = false;
							while (!scaled)
							{
								using (var font = new Font(collection.Families.First(), size, FontStyle.Bold))
								{
									var measurement = graphics.MeasureString(quote, font, 875);
									if (measurement.Height > 629 && size > 16) //16 = minimum size
									{
										size -= 4;
									}
									else
									{
										scaled = true;
										graphics.DrawString(quote, font, Brushes.White, new RectangleF(40, 20, 875, 629));

										var top = measurement.Height + 30;
										var fontsm = new Font(collection.Families.First(), 22, FontStyle.Italic);
										var sigMeasure = graphics.MeasureString(source, fontsm, 875);
										graphics.DrawString(source, fontsm, Brushes.LightGray, new PointF(875 - sigMeasure.Width, top));
									}
								}
							}							
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
