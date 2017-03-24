using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

using Shouldly;

using Xunit;

namespace StreamIssue
{
    public class Tests
    {
		[Fact]
	    public async Task GivenInvalidJson_WhenPost_ThenShouldReturnError() {
		    var testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
		    var client = testServer.CreateClient();
			var postMessage = new HttpRequestMessage(HttpMethod.Post, "/");
			postMessage.Content = new StringContent("{");
		    var response = await client.SendAsync(postMessage);

		    response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
	    }

	    [Fact]
	    public async Task GivenValidJson_WhenPost_ThenShouldReturnOk() {
		    var testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
		    var client = testServer.CreateClient();
		    var postMessage = new HttpRequestMessage(HttpMethod.Post, "/");
		    postMessage.Content = new StringContent("{}");
		    var response = await client.SendAsync(postMessage);

		    response.StatusCode.ShouldBe(HttpStatusCode.OK);
	    }
	}
}
