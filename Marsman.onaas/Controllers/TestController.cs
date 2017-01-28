using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Marsman.onaas.Controllers
{
    public class TestController : ApiController
    {
		[HttpGet]
		public string BreakOhNo()
		{
			int activityLength = 1, errorLength = 1, maxLines = 1;
			List<string> failureSets = new List<string>();

			try
			{
				while (activityLength < 1000)
				{
					new OhNoController(true).Get(RandomString(activityLength++), "test");
				}
			}
			catch
			{

			}

			try
			{
				while (errorLength < 1000)
				{
					new OhNoController(true).Get(RandomString(errorLength++), "test");
				}
			}
			catch
			{

			}

			try
			{
				var lines = RandomString(10);
				while (maxLines < 1000)
				{
					new OhNoController(true).Get(lines, "test");
					lines = $"{lines}{Environment.NewLine}{RandomString(10)}";
					maxLines++;
				}
			}
			catch
			{

			}

			try
			{
				new OhNoController(true).Get("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToLower());
			}
			catch
			{
				failureSets.Add("alphanumeric");
			}

			try
			{
				new OhNoController(true).Get("!\"£$%^&*()-_=+{}[]:@;'~#<>?,./¬`", "!\"£$%^&*()-_=+{}[]:@;'~#<>?,./¬`");
			}
			catch
			{
				failureSets.Add("punctuation");
			}

			try
			{
				new OhNoController(true).Get("ΤΥΦΧΨεζλ℗©™ΤΥΦΧΨεζλ℗©™", "ΤΥΦΧΨεζλ℗©™ΤΥΦΧΨεζλ℗©™");
			}
			catch
			{
				failureSets.Add("unicode");
			}

			return $"activityLength:{activityLength}{(activityLength == 1000 ? "(max)" : "")}, errorLength:{errorLength}{(errorLength == 1000 ? "(max)" : "")}, maxLines:{maxLines}{(maxLines == 1000 ? "(max)" : "")}, failures: {string.Join(", ", failureSets)}";
		}

		private static Random random = new Random();
		public static string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
}
