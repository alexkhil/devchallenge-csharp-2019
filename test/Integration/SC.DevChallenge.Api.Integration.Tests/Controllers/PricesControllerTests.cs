﻿using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SC.DevChallenge.Domain.Constants;
using SC.DevChallenge.Dto;
using SC.DevChallenge.Dto.Prices.GetAveragePrice;
using Xunit;

namespace SC.DevChallenge.Api.Integration.Tests.Controllers
{
    public class PricesControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;

        public PricesControllerTests(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task GetAveragePrice_ValidRequest_HttpStatusCodeOK()
        {
            // Arrange
            var client = factory.CreateClient();
            var uriBuilder = new UriBuilder(client.BaseAddress)
            {
                Query = "portfolio=Fannie Mae&owner=Google&instrument=CDS&date=15/03/2018 17:34:50",
                Path = "api/prices/average"
            };

            // Act
            var response = await client.GetAsync(uriBuilder.Uri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAveragePrice_NotExistingPortfolio_HttpStatusCodeNotFound()
        {
            // Arrange
            var client = factory.CreateClient();
            var uriBuilder = new UriBuilder(client.BaseAddress)
            {
                Query = "portfolio=qwerty&owner=Microsoft&instrument=Deposit&date=15/03/2018 17:34:50",
                Path = "api/prices/average"
            };

            // Act
            var response = await client.GetAsync(uriBuilder.Uri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAveragePrice_ValidRequest_CorrectContent()
        {
            // Arrange
            var expected = new AveragePriceDto
            {
                Date = new DateTime(2018, 3, 15, 17, 26, 40),
                Price = 133.71
            };

            var client = factory.CreateClient();
            var uriBuilder = new UriBuilder(client.BaseAddress)
            {
                Query = "portfolio=Fannie Mae&owner=Google&instrument=CDS&date=15/03/2018 17:34:50",
                Path = "api/prices/average"
            };

            // Act
            var response = await client.GetAsync(uriBuilder.Uri);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<AveragePriceDto>(jsonContent, new IsoDateTimeConverter() { DateTimeFormat = DateTimeFormat.Default });

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetBenchmark_ValidRequest_HttpStatusCodeOK()
        {
            // Arrange
            var client = factory.CreateClient();
            var uriBuilder = new UriBuilder(client.BaseAddress)
            {
                Query = "portfolio=Fannie Mae&date=15/03/2018 17:34:50",
                Path = "api/prices/benchmark"
            };

            // Act
            var response = await client.GetAsync(uriBuilder.Uri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetBenchmark_NotExistingPortfolio_HttpStatusCodeNotFound()
        {
            // Arrange
            var client = factory.CreateClient();
            var uriBuilder = new UriBuilder(client.BaseAddress)
            {
                Query = "portfolio=qwerty&date=15/03/2018 17:34:50",
                Path = "api/prices/benchmark"
            };

            // Act
            var response = await client.GetAsync(uriBuilder.Uri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetBenchmark_ValidRequest_CorrectContent()
        {
            // Arranges
            var expected = new BenchmarkPriceDto
            {
                Date = new DateTime(2018, 3, 15, 17, 26, 40),
                Price = 133.71
            };

            var client = factory.CreateClient();
            var uriBuilder = new UriBuilder(client.BaseAddress)
            {
                Query = "portfolio=Fannie Mae&date=15/03/2018 17:34:50",
                Path = "api/prices/benchmark"
            };

            // Act
            var response = await client.GetAsync(uriBuilder.Uri);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<BenchmarkPriceDto>(jsonContent, new IsoDateTimeConverter() { DateTimeFormat = DateTimeFormat.Default });

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}