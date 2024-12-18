using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Typeracer.Context;
using Typeracer.Models;

namespace TyperacerIntegrationTests.ControllersIT;

public class ParagraphsControllerIT : IClassFixture<CustomWebApplicationFactory<Program>>
{
   private readonly HttpClient _client;
   private readonly AppDbContext _context;

   public ParagraphsControllerIT(CustomWebApplicationFactory<Program> factory)
   {
      _client = factory.CreateClient();
      var serviceProvider = factory.Services;
      _context = serviceProvider.GetRequiredService<AppDbContext>();
   }

   [Fact]
   public async Task GetParagraph_ReturnsNotFound_WhenParagraphDoesNotExist()
   {
      var paragraphId = Guid.NewGuid().ToString();

      var response = await _client.GetAsync($"/api/Paragraphs/{paragraphId}");
      
      Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
   }

   [Fact]
   public async Task GetParagraph_ReturnsParagraph_WhenParagraphExists()
   {
      var id = Guid.NewGuid();
      var paragraph = new Paragraph("This is a test paragraph", new List<Gamemode>());
      paragraph.Id = id;
      _context.Paragraphs.Add(paragraph);
      _context.SaveChanges();
      
      var response = await _client.GetAsync($"/api/Paragraphs/{id}");
      var returnedParagraph = await response.Content.ReadFromJsonAsync<Paragraph>();
      
      response.EnsureSuccessStatusCode();
      Assert.NotNull(returnedParagraph);
      Assert.Equal(id, returnedParagraph.Id);
      Assert.Equal("This is a test paragraph", returnedParagraph.Text);
      Assert.Empty(returnedParagraph.AllowedGamemodes);

   }
}