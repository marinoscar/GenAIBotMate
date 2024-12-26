using Luval.AuthMate.Core.Interfaces;
using Luval.AuthMate.Infrastructure.Logging;
using Luval.GenAIBotMate.Core.Entities;
using Luval.GenAIBotMate.Core.Services;
using Luval.GenAIBotMate.Infrastructure.Data;
using Luval.GenAIBotMate.Tests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Tests
{
    public class ChatbotServiceTests
    {

        private GenAIBotService CreateService(Action<MemoryDataContext>? workOnContext, Action<CompletionServiceStub>? workOnCompletion)
        {
            var userResolverMock = new Mock<IUserResolver>();
            userResolverMock.Setup(x => x.GetUserEmail()).Returns("user@email.com");
            var mediaService = new MediaServiceStub();

            var context = new MemoryDataContext();
            var completionService = new CompletionServiceStub();
            var storageService = new GenAIBotStorageService(context, new NullLogger<GenAIBotStorageService>(), userResolverMock.Object);
            var service = new GenAIBotService(completionService, storageService, mediaService, new NullLogger<GenAIBotService>());

            context.Initialize();
            completionService.Initialize();
            workOnContext?.Invoke(context);
            workOnCompletion?.Invoke(completionService);

            return service;
        }


        [Fact]
        public async Task SubmitMessageToNewSession_ShouldCreateChatSessionAndReturnChatMessage()
        {
            // Arrange
            ulong chatbotId = 1;
            string message = "Hello, world!";
            string sessionTitle = "Test Session";
            IEnumerable<UploadFile>? files = null;

            var service = CreateService((c) =>
            {
                var cb = new GenAIBot()
                {
                    AccountId = 1,
                    Name = "Test Chatbot"
                };
                c!.GenAIBots.Add(cb);
                c!.SaveChanges();
                chatbotId = cb.Id;
            }, null);

            // Act
            var result = await service.SubmitMessageToNewSession(chatbotId, message, sessionTitle, files);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(message, result.UserMessage);
            Assert.NotNull(result.AgentResponse);
            Assert.NotNull(result.ChatSession);
            Assert.Equal(chatbotId, result.ChatSession.GenAIBotId);
            Assert.True(result.OutputTokens > 0);
        }

        [Fact]
        public async Task SubmitMessageToNewSession_ShouldCreateChatSessionAndReturnChatMessageAndCompletedEventIsCalled()
        {
            // Arrange
            ulong chatbotId = 1;
            var service = CreateService((c) =>
            {
                var cb = new GenAIBot()
                {
                    AccountId = 1,
                    Name = "Test Chatbot"
                };
                c!.GenAIBots.Add(cb);
                c!.SaveChanges();
                chatbotId = cb.Id;
            }, null);

            var eventRaised = false;

            service.ChatMessageCompleted += (s, e) =>
            {
                Assert.NotNull(e);
                Assert.NotNull(e.Content);
                Assert.Equal("Stop", e.FinishReason);
                Assert.True(e.InputTokenCount > 0);
                Assert.True(e.OutputTokenCount > 0);
                eventRaised = true;
            };

            // Act
            var result = await service.SubmitMessageToNewSession(chatbotId, "Hello, world!", "Title", null);

            // Assert
            Assert.NotNull(result);
            Assert.True(eventRaised);
        }

        [Fact]
        public async Task SubmitMessageToNewSession_ShouldCreateChatSessionAndReturnChatMessageAndStreamEventIsCalled()
        {
            // Arrange
            ulong chatbotId = 1;
            var service = CreateService((c) =>
            {
                var cb = new GenAIBot()
                {
                    AccountId = 1,
                    Name = "Test Chatbot"
                };
                c!.GenAIBots.Add(cb);
                c!.SaveChanges();
                chatbotId = cb.Id;
            }, null);

            var eventRaised = false;

            service.ChatMessageStream += (s, e) =>
            {
                Assert.NotNull(e);
                eventRaised = true;
            };

            // Act
            var result = await service.SubmitMessageToNewSession(chatbotId, "Hello, world!", "Title", null);

            // Assert
            Assert.NotNull(result);
            Assert.True(eventRaised);
        }

        [Fact]
        public async Task SubmitMessageToNewSession_ShouldHandleFilesCorrectly()
        {
            // Arrange
            ulong chatbotId = 1;
            string message = "Hello, world!";
            string sessionTitle = "Test Session";
            var files = new List<UploadFile>
            {
                new() { Name = "file1.txt", Content = new MemoryStream(Encoding.UTF8.GetBytes("File content 1")) },
                new() { Name = "file2.txt", Content = new MemoryStream(Encoding.UTF8.GetBytes("File content 2")) }
            };

            var service = CreateService((c) =>
            {
                var cb = new GenAIBot()
                {
                    AccountId = 1,
                    Name = "Test Chatbot"
                };
                c!.GenAIBots.Add(cb);
                c!.SaveChanges();
                chatbotId = cb.Id;
            }, null);

            // Act
            var result = await service.SubmitMessageToNewSession(chatbotId, message, sessionTitle, files);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(message, result.UserMessage);
            Assert.NotNull(result.AgentResponse);
            Assert.NotNull(result.ChatSession);
            Assert.Equal(chatbotId, result.ChatSession.GenAIBotId);
            Assert.True(result.OutputTokens > 0);
            Assert.Equal(2, result.Media.Count);
        }

        [Fact]
        public async Task AppendMessageToSession_ShouldStartNewSessionAndAddAnotherMessage()
        {
            // Arrange
            ulong chatbotId = 1;
            string initialMessage = "Hello, world!";
            string followUpMessage = "How are you?";
            string sessionTitle = "Test Session";
            IEnumerable<UploadFile>? files = null;

            var service = CreateService((c) =>
            {
                var cb = new GenAIBot()
                {
                    AccountId = 1,
                    Name = "Test Chatbot"
                };
                c!.GenAIBots.Add(cb);
                c!.SaveChanges();
                chatbotId = cb.Id;
            }, null);

            // Act
            var initialResult = await service.SubmitMessageToNewSession(chatbotId, initialMessage, sessionTitle, files);
            var followUpResult = await service.AppendMessageToSession(followUpMessage, initialResult.ChatSession.Id, files);

            // Assert
            Assert.NotNull(initialResult);
            Assert.Equal(initialMessage, initialResult.UserMessage);
            Assert.NotNull(initialResult.AgentResponse);
            Assert.NotNull(initialResult.ChatSession);
            Assert.Equal(chatbotId, initialResult.ChatSession.GenAIBotId);
            Assert.True(initialResult.OutputTokens > 0);

            Assert.NotNull(followUpResult);
            Assert.Equal(followUpMessage, followUpResult.UserMessage);
            Assert.NotNull(followUpResult.AgentResponse);
            Assert.Equal(initialResult.ChatSession.Id, followUpResult.ChatSessionId);
            Assert.True(followUpResult.OutputTokens > 0);

            Assert.Equal(2, followUpResult.ChatSession.ChatMessages.Count);

        }

        [Fact]
        public async Task AppendMessageToSession_ShouldStartNewSessionWithoutFilesAndAddFollowUpMessageWithFiles()
        {
            // Arrange
            ulong chatbotId = 1;
            string initialMessage = "Hello, world!";
            string followUpMessage = "Here are some files.";
            string sessionTitle = "Test Session";
            IEnumerable<UploadFile>? initialFiles = null;
            var followUpFiles = new List<UploadFile>
            {
                new() { Name = "file1.txt", Content = new MemoryStream(Encoding.UTF8.GetBytes("File content 1")) },
                new() { Name = "file2.txt", Content = new MemoryStream(Encoding.UTF8.GetBytes("File content 2")) }
            };

            var service = CreateService((c) =>
            {
                var cb = new GenAIBot()
                {
                    AccountId = 1,
                    Name = "Test Chatbot"
                };
                c!.GenAIBots.Add(cb);
                c!.SaveChanges();
                chatbotId = cb.Id;
            }, null);

            // Act
            var initialResult = await service.SubmitMessageToNewSession(chatbotId, initialMessage, sessionTitle, initialFiles);
            var followUpResult = await service.AppendMessageToSession(followUpMessage, initialResult.ChatSession.Id, followUpFiles);

            // Assert
            Assert.NotNull(initialResult);
            Assert.Equal(initialMessage, initialResult.UserMessage);
            Assert.NotNull(initialResult.AgentResponse);
            Assert.NotNull(initialResult.ChatSession);
            Assert.Equal(chatbotId, initialResult.ChatSession.GenAIBotId);
            Assert.True(initialResult.OutputTokens > 0);

            Assert.NotNull(followUpResult);
            Assert.Equal(followUpMessage, followUpResult.UserMessage);
            Assert.NotNull(followUpResult.AgentResponse);
            Assert.Equal(initialResult.ChatSession.Id, followUpResult.ChatSessionId);
            Assert.True(followUpResult.OutputTokens > 0);
            Assert.Equal(2, followUpResult.Media.Count);

            Assert.Equal(2, followUpResult.ChatSession.ChatMessages.Count);
        }

    }
}
