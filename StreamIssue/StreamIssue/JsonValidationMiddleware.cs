using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Newtonsoft.Json.Linq;

namespace StreamIssue {
	public class JsonValidatorMiddleware {
		private readonly RequestDelegate next;
		private static readonly HttpMethod[] RequiredVerbs = { HttpMethod.Post, new HttpMethod("PATCH") };

		public JsonValidatorMiddleware(RequestDelegate next) {
			this.next = next;
		}

		public async Task Invoke(HttpContext context) {
			var httpVerb = new HttpMethod(context.Request.Method);
			if(!RequiredVerbs.Contains(httpVerb)) {
				await this.next.Invoke(context);
				return;
			}

			var initalBody = context.Request.Body;
			var requestBody = new StreamReader(context.Request.Body).ReadToEnd();

			bool success;
			try {
				JToken.Parse(requestBody);
				success = true;
			} catch(Exception) {
				success = false;
			}
			
			if(success) {
				initalBody.Seek(0, SeekOrigin.Begin);
				context.Request.Body = initalBody;
				await this.next.Invoke(context);
			} else {
				context.Response.StatusCode = 400;
				await context.Response.WriteAsync("Not valid!");
			}
		}
	}

	public static class MiddlewareApplicationBuilderExtensions {
		public static IApplicationBuilder UseJsonValidator(this IApplicationBuilder builder) {
			return builder.UseMiddleware<JsonValidatorMiddleware>();
		}
	}
}
