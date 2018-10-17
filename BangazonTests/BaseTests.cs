using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Collections.Generic;
using System.Net.Http;

namespace BangazonTests
{
    public class BaseTests : IClassFixture<WebApplicationFactory<Bangazon.Startup>>
    {
        private readonly WebApplicationFactory<Bangazon.Startup> _factory;

        public BaseTests(WebApplicationFactory<Bangazon.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Home")]
        [InlineData("/Home/About")]
        [InlineData("/Home/Privacy")]
        [InlineData("/Home/Contact")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Get_Home_200Status_HasCopywrite()
        {
            // Arrange
            var _client = _factory.CreateClient();

            //Act
            var response = await _client.GetAsync("/");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Matches("&copy; 2018 - Bangazon", content);
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Post_Product_Get_ListContainsProduct()
        {
            // Arrange
            var _client = _factory.CreateClient();

            var formData = new Dictionary<string, string>
              {
                {"Description", "It tastes yummy"},
                {"Title", "Lollipop"},
                {"Price", "1.99"},
                {"Quantity", "149"},
                {"ProductType", "1"}
              };

            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "/Products/Create")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            //Act
            var post = await _client.SendAsync(postRequest);
            var response = await _client.GetAsync("/Products");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Matches("Lollipop", content);
            Assert.Matches("It tastes yummy", content);
            Assert.Matches("149", content);
            Assert.Matches("1.99", content);
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

    }
}