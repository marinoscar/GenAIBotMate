using Luval.AuthMate.Core.Interfaces;
using Luval.AuthMate.Infrastructure.Logging;
using Luval.GenAIBotMate.Core.Entities;
using Luval.GenAIBotMate.Core.Services;
using Luval.GenAIBotMate.Infrastructure.Data;
using Luval.GenAIBotMate.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Luval.GenAIBotMate.Tests
{
    public class ChatbotStorageServiceTests
    {

        private GenAIBotStorageService CreateService(Action<MemoryDataContext>? workOnContext)
        {
            var context = new MemoryDataContext();
            context.Initialize();
            var userResolverMock = new Mock<IUserResolver>();
            userResolverMock.Setup(x => x.GetUserEmail()).Returns("user@email.com");
            var service = new GenAIBotStorageService(context, new NullLogger<GenAIBotStorageService>(), userResolverMock.Object);

            workOnContext?.Invoke(context);

            return service;
        }

        #region Chatbot Test Cases

        [Fact]
        public async Task CreateChatbotAsync_ShouldCreateChatbot()
        {
            // Arrange
            var service = CreateService(null);
            var chatbot = new GenAIBot
            {
                Name = "New Test Chatbot",
                AccountId = 1
            };

            Action<MemoryDataContext> contextSetup = (c) =>
            {
                c.GenAIBots.Add(chatbot);
                c.SaveChanges();
            };

            // Act
            var result = await service.CreateChatbotAsync(chatbot);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Test Chatbot", result.Name);
            Assert.NotNull(result.CreatedBy);
            Assert.NotNull(result.UpdatedBy);
            Assert.True(result.Id > 0);
            Assert.True(result.UtcCreatedOn > DateTime.UtcNow.AddHours(-1));
        }
        [Fact]
        public async Task CreateChatbotAsync_ShouldThrowArgumentNullException_WhenChatbotIsNull()
        {
            // Arrange
            var service = CreateService(null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateChatbotAsync(null));
        }
        [Fact]
        public async Task GetChatbotAsync_ShouldReturnChatbot_WhenChatbotExists()
        {
            // Arrange
            var newBot = new GenAIBot
            {
                Name = "New Test Chatbot",
                AccountId = 1
            };

            var service = CreateService(context =>
            {
                context.GenAIBots.Add(newBot);
                context.SaveChanges();
                var chatSession = new ChatSession
                {
                    GenAIBotId = newBot.Id,
                    Title = "Test Chat Session",
                    Version = 1
                };
                context.ChatSessions.Add(chatSession);
                context.SaveChanges();
                var message = new ChatMessage
                {
                    ChatSessionId = chatSession.Id,
                    UserMessage = "Test Message",
                    AgentResponse = "Test Response",
                    Version = 1
                };
                context.ChatMessages.Add(message);
                context.SaveChanges();
                var media = new ChatMessageMedia()
                {
                    ChatMessageId = message.Id,
                    ProviderName = "Test Provider",
                    MediaUrl = "http://test.com",
                    Version = 1
                };
                context.ChatMessageMedia.Add(media);
                context.SaveChanges();
            });

            // Act
            var result = await service.GetChatbotAsync(newBot.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newBot.Name, result.Name);
            Assert.Equal(newBot.AccountId, result.AccountId);
            Assert.NotEmpty(result.ChatSessions);
        }
        [Fact]
        public async Task UpdateChatbotAsync_ShouldUpdateChatbot()
        {
            // Arrange
            var chatbotId = 0ul;
            var service = CreateService(context =>
            {
                var chatbot = new GenAIBot
                {
                    Name = "Old Chatbot",
                    AccountId = 1
                };
                context.GenAIBots.Add(chatbot);
                context.SaveChanges();
                chatbotId = chatbot.Id;
            });

            var updatedChatbot = new GenAIBot
            {
                Id = chatbotId,
                Name = "Updated Chatbot",
                AccountId = 1
            };

            // Act
            var result = await service.UpdateChatbotAsync(updatedChatbot);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Chatbot", result.Name);
            Assert.Equal(1ul, result.AccountId);
            Assert.NotNull(result.UpdatedBy);
            Assert.True(result.UtcUpdatedOn > DateTime.UtcNow.AddMinutes(-10));
        }

        [Fact]
        public async Task UpdateChatbotAsync_ShouldThrowArgumentNullException_WhenChatbotIsNull()
        {
            // Arrange
            var service = CreateService(null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateChatbotAsync(null));
        }

        [Fact]
        public async Task UpdateChatbotAsync_ShouldThrowInvalidOperationException_WhenChatbotDoesNotExist()
        {
            // Arrange
            var service = CreateService(null);
            var nonExistentChatbot = new GenAIBot
            {
                Id = 999,
                Name = "Non-existent Chatbot",
                AccountId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateChatbotAsync(nonExistentChatbot));
        }
        [Fact]
        public async Task DeleteChatbotAsync_ShouldDeleteChatbot()
        {
            // Arrange
            var chatbotId = 0ul;
            var service = CreateService(context =>
            {
                var chatbot = new GenAIBot
                {
                    Name = "Chatbot to Delete",
                    AccountId = 1
                };
                context.GenAIBots.Add(chatbot);
                context.SaveChanges();
                chatbotId = chatbot.Id;
            });

            // Act
            await service.DeleteChatbotAsync(chatbotId);

            // Assert
            var deletedChatbot = await service.GetChatbotAsync(chatbotId);
            Assert.Null(deletedChatbot);
        }

        [Fact]
        public async Task DeleteChatbotAsync_ShouldThrowInvalidOperationException_WhenChatbotDoesNotExist()
        {
            // Arrange
            var service = CreateService(null);
            var nonExistentChatbotId = 999ul;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteChatbotAsync(nonExistentChatbotId));
        }

        #endregion

        #region ChatSession Test Cases

        [Fact]
        public async Task CreateChatSessionAsync_ShouldCreateChatSession()
        {
            // Arrange
            var chatbotId = 0ul;
            var service = CreateService(context =>
            {
                var chatbot = new GenAIBot
                {
                    Name = "New Test Chatbot",
                    AccountId = 1
                };
                context.GenAIBots.Add(chatbot);
                context.SaveChanges();
                chatbotId = chatbot.Id;
            });

            var chatSession = new ChatSession
            {
                GenAIBotId = chatbotId,
                Title = "Test Chat Session"
            };

            // Act
            var result = await service.CreateChatSessionAsync(chatSession);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Chat Session", result.Title);
            Assert.Equal(chatbotId, result.GenAIBotId);
            Assert.NotNull(result.CreatedBy);
            Assert.NotNull(result.UpdatedBy);
            Assert.Equal(1u, result.Version);
            Assert.True(result.Id > 0);
            Assert.True(result.UtcCreatedOn > DateTime.UtcNow.AddHours(-1));
        }

        [Fact]
        public async Task CreateChatSessionAsync_ShouldThrowArgumentNullException_WhenChatSessionIsNull()
        {
            // Arrange
            var service = CreateService(null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateChatSessionAsync(null));
        }

        [Fact]
        public async Task CreateChatSessionAsync_ShouldThrowArgumentException_WhenChatbotIdIsInvalid()
        {
            // Arrange
            var service = CreateService(null);
            var chatSession = new ChatSession
            {
                GenAIBotId = 999,
                Title = "Test Chat Session"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateChatSessionAsync(chatSession));
        }

        [Fact]
        public async Task UpdateChatSessionAsync_ShouldUpdateChatSession()
        {
            // Arrange
            var chatSessionId = 0ul;
            var service = CreateService(context =>
            {
                var chatbot = new GenAIBot
                {
                    Name = "New Test Chatbot",
                    AccountId = 1
                };
                context.GenAIBots.Add(chatbot);
                context.SaveChanges();
                var chatSession = new ChatSession
                {
                    GenAIBotId = chatbot.Id,
                    Title = "Old Chat Session",
                    Version = 1
                };
                context.ChatSessions.Add(chatSession);
                context.SaveChanges();
                chatSessionId = chatSession.Id;
            });

            var updatedChatSession = new ChatSession
            {
                Id = chatSessionId,
                GenAIBotId = 1,
                Title = "Updated Chat Session"
            };

            // Act
            var result = await service.UpdateChatSessionAsync(updatedChatSession);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Chat Session", result.Title);
            Assert.Equal(1ul, result.GenAIBotId);
            Assert.NotNull(result.UpdatedBy);
            Assert.True(result.UtcUpdatedOn > DateTime.UtcNow.AddMinutes(-10));
        }

        [Fact]
        public async Task UpdateChatSessionAsync_ShouldThrowArgumentNullException_WhenChatSessionIsNull()
        {
            // Arrange
            var service = CreateService(null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateChatSessionAsync(null));
        }

        [Fact]
        public async Task UpdateChatSessionAsync_ShouldThrowInvalidOperationException_WhenChatSessionDoesNotExist()
        {
            // Arrange
            var service = CreateService(null);
            var nonExistentChatSession = new ChatSession
            {
                Id = 999,
                GenAIBotId = 1,
                Title = "Non-existent Chat Session"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateChatSessionAsync(nonExistentChatSession));
        }

        [Fact]
        public async Task DeleteChatSessionAsync_ShouldDeleteChatSession()
        {
            // Arrange
            var chatSessionId = 0ul;
            var service = CreateService(context =>
            {
                var chatbot = new GenAIBot
                {
                    Name = "New Test Chatbot",
                    AccountId = 1
                };
                context.GenAIBots.Add(chatbot);
                context.SaveChanges();
                var chatSession = new ChatSession
                {
                    GenAIBotId = chatbot.Id,
                    Title = "Chat Session to Delete",
                    Version = 1
                };
                context.ChatSessions.Add(chatSession);
                context.SaveChanges();
                chatSessionId = chatSession.Id;
            });

            // Act
            await service.DeleteChatSessionAsync(chatSessionId);

            // Assert
            var deletedChatSession = await service.GetChatSessionAsync(chatSessionId);
            Assert.Null(deletedChatSession);
        }

        [Fact]
        public async Task DeleteChatSessionAsync_ShouldThrowInvalidOperationException_WhenChatSessionDoesNotExist()
        {
            // Arrange
            var service = CreateService(null);
            var nonExistentChatSessionId = 999ul;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteChatSessionAsync(nonExistentChatSessionId));
        }

        #endregion

        #region ChatMessage Test Cases

        [Fact]
        public async Task AddChatMessageAsync_ShouldAddChatMessage()
        {
            // Arrange
            var chatSessionId = 0ul;
            var service = CreateService(context =>
            {
                var chatbot = new GenAIBot
                {
                    Name = "New Test Chatbot",
                    AccountId = 1,
                };
                context.GenAIBots.Add(chatbot);
                context.SaveChanges();
                var chatSession = new ChatSession
                {
                    GenAIBotId = chatbot.Id,
                    Title = "Test Chat Session",
                    Version = 1
                };
                context.ChatSessions.Add(chatSession);
                context.SaveChanges();
                chatSessionId = chatSession.Id;
            });

            var chatMessage = new ChatMessage
            {
                UserMessage = "Test User Message",
                AgentResponse = "Test Agent Response",
                InputTokens = 10,
                OutputTokens = 20
            };

            // Act
            var result = await service.AddChatMessageAsync(chatSessionId, chatMessage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test User Message", result.UserMessage);
            Assert.Equal("Test Agent Response", result.AgentResponse);
            Assert.Equal(chatSessionId, result.ChatSessionId);
            Assert.NotNull(result.CreatedBy);
            Assert.NotNull(result.UpdatedBy);
            Assert.Equal(1u, result.Version);
            Assert.True(result.Id > 0);
            Assert.True(result.UtcCreatedOn > DateTime.UtcNow.AddHours(-1));
        }

        [Fact]
        public async Task AddChatMessageAsync_ShouldThrowArgumentNullException_WhenChatMessageIsNull()
        {
            // Arrange
            var service = CreateService(null);
            var chatSessionId = 1ul;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddChatMessageAsync(chatSessionId, null));
        }

        [Fact]
        public async Task AddChatMessageAsync_ShouldThrowArgumentException_WhenChatSessionIdIsInvalid()
        {
            // Arrange
            var service = CreateService(null);
            var chatMessage = new ChatMessage
            {
                UserMessage = "Test User Message",
                AgentResponse = "Test Agent Response",
                InputTokens = 10,
                OutputTokens = 20
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddChatMessageAsync(0, chatMessage));
        }

        [Fact]
        public async Task AddChatMessageAsync_ShouldAddChatMessageWithMedia()
        {
            // Arrange
            var chatSessionId = 0ul;
            var service = CreateService(context =>
            {
                var chatbot = new GenAIBot
                {
                    Name = "New Test Chatbot",
                    AccountId = 1
                };
                context.GenAIBots.Add(chatbot);
                context.SaveChanges();
                var chatSession = new ChatSession
                {
                    GenAIBotId = chatbot.Id,
                    Title = "Test Chat Session",
                    Version = 1
                };
                context.ChatSessions.Add(chatSession);
                context.SaveChanges();
                chatSessionId = chatSession.Id;
            });

            var chatMessage = new ChatMessage
            {
                UserMessage = "Test User Message",
                AgentResponse = "Test Agent Response",
                InputTokens = 10,
                OutputTokens = 20
            };

            var media = new List<ChatMessageMedia>
            {
                new ChatMessageMedia
                {
                    MediaUrl = "http://test.com/media1",
                    ProviderName = "Test Provider",
                    Version = 1
                },
                new ChatMessageMedia
                {
                    MediaUrl = "http://test.com/media2",
                    ProviderName = "Test Provider",
                    Version = 1
                }
            };

            // Act
            var result = await service.AddChatMessageAsync(chatSessionId, chatMessage, media);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test User Message", result.UserMessage);
            Assert.Equal("Test Agent Response", result.AgentResponse);
            Assert.Equal(chatSessionId, result.ChatSessionId);
            Assert.True(result.ChatSession.HasMedia); // Chat session should have media
            Assert.NotNull(result.CreatedBy);
            Assert.NotNull(result.UpdatedBy);
            Assert.Equal(1u, result.Version);
            Assert.True(result.Id > 0);
            Assert.True(result.UtcCreatedOn > DateTime.UtcNow.AddHours(-1));
            Assert.Equal(2, result.Media.Count);
            Assert.Contains(result.Media, m => m.MediaUrl == "http://test.com/media1");
            Assert.Contains(result.Media, m => m.MediaUrl == "http://test.com/media2");
        }

        [Fact]
        public async Task AddMessageMediaAsync_ShouldAddMessageMedia()
        {
            // Arrange
            var chatMessageId = 0ul;
            var service = CreateService(context =>
            {
                var chatbot = new GenAIBot
                {
                    Name = "New Test Chatbot",
                    AccountId = 1
                };
                context.GenAIBots.Add(chatbot);
                context.SaveChanges();
                var chatSession = new ChatSession
                {
                    GenAIBotId = chatbot.Id,
                    Title = "Test Chat Session",
                    Version = 1
                };
                context.ChatSessions.Add(chatSession);
                context.SaveChanges();
                var chatMessage = new ChatMessage
                {
                    ChatSessionId = chatSession.Id,
                    UserMessage = "Test User Message",
                    AgentResponse = "Test Agent Response",
                    InputTokens = 10,
                    OutputTokens = 20,
                    Version = 1
                };
                context.ChatMessages.Add(chatMessage);
                context.SaveChanges();
                chatMessageId = chatMessage.Id;
            });

            var media = new ChatMessageMedia
            {
                MediaUrl = "http://test.com/media",
                ProviderName = "Test Provider",
                Version = 1
            };

            // Act
            var result = await service.AddMessageMediaAsync(chatMessageId, media);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("http://test.com/media", result.MediaUrl);
            Assert.Equal("Test Provider", result.ProviderName);
            Assert.Equal(chatMessageId, result.ChatMessageId);
            Assert.NotNull(result.CreatedBy);
            Assert.NotNull(result.UpdatedBy);
            Assert.Equal(1u, result.Version);
            Assert.True(result.Id > 0);
            Assert.True(result.UtcCreatedOn > DateTime.UtcNow.AddHours(-1));
        }

        [Fact]
        public async Task AddMessageMediaAsync_ShouldThrowArgumentNullException_WhenMediaIsNull()
        {
            // Arrange
            var service = CreateService(null);
            var chatMessageId = 1ul;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddMessageMediaAsync(chatMessageId, null));
        }

        [Fact]
        public async Task AddMessageMediaAsync_ShouldThrowArgumentException_WhenChatMessageIdIsInvalid()
        {
            // Arrange
            var service = CreateService(null);
            var media = new ChatMessageMedia
            {
                MediaUrl = "http://test.com/media",
                ProviderName = "Test Provider",
                Version = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddMessageMediaAsync(0, media));
        }

        #endregion

    }

}

