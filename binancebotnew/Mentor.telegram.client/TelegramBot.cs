using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OpenTl.ClientApi;
using OpenTl.Schema;
using OpenTl.Schema.Channels;
using Core.Common.Extensions;
using OpenTl.Schema.Contacts;
using OpenTl.Schema.Messages;

namespace Mentor.telegram.client
{
    public class TelegramBot
    {
        private readonly IClientApi _client;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(TelegramBot));

        public TelegramBot(IClientApi client)
        {
            _client = client;
        }

        public void Subscribe()
        {
            //_client.UpdatesService.AutoReceiveUpdates += UpdatesServiceOnRecieveUpdates;
        }

        private Task UpdatesServiceOnRecieveUpdates(IUpdates update)
        {
            switch (update)
            {
                case TUpdates updates:
                    break;
                case TUpdatesCombined updatesCombined:
                    break;
                case TUpdateShort updateShort:
                    break;
                case TUpdateShortChatMessage updateShortChatMessage:
                    break;
                case TUpdateShortMessage updateShortMessage:
                    break;
                case TUpdateShortSentMessage updateShortSentMessage:
                    break;
                case TUpdatesTooLong updatesTooLong:
                    break;
            }

            return null;
        }

        public async Task<IEnumerable<TChannel>> GetChannels()
        {
            var result = new List<TChannel>();
            var responseObject = (await _client.MessagesService.GetUserDialogsAsync(1000));
            TVector<IChat> chats;

            if (!(responseObject is TDialogs))
            {
                chats = ((TDialogsSlice) responseObject).Chats;
            }
            else
            {
                chats = ((TDialogs) responseObject).Chats;
            }

            foreach (var chat in chats)
            {
                switch (chat)
                {
                    case TChannel channel:
                        result.Add(channel);
                        break;
                }
            }

            return result;
        }

        public async Task<IEnumerable<TChat>> GetChats()
        {
            var result = new List<TChat>();
            var responseObject = (await _client.MessagesService.GetUserDialogsAsync(1000));
            TVector<IChat> chats;

            if (!(responseObject is TDialogs))
            {
                chats = ((TDialogsSlice) responseObject).Chats;
            }
            else
            {
                chats = ((TDialogs) responseObject).Chats;
            }

            foreach (var chat in chats)
            {
                switch (chat)
                {
                    case TChannel channel:
                        break;
                    case TChat myChat:
                        result.Add(myChat);
                        break;
                }
            }

            return result;
        }

        public async Task<ChatData> FindChatData(string chatTitle)
        {
            var result = new ChatData();
            var responseObject = (await _client.MessagesService.GetUserDialogsAsync(1000));
            TVector<IChat> chats;

            if (!(responseObject is TDialogs))
            {
                chats = ((TDialogsSlice) responseObject).Chats;
            }
            else
            {
                chats = ((TDialogs) responseObject).Chats;
            }

            foreach (var chat in chats)
            {
                switch (chat)
                {
                    case TChannel channel:
                        if (channel.Title == chatTitle)
                        {
                            result.ChannelType = true;
                            result.AccessHash = channel.AccessHash;
                            result.ChatId = channel.Id;
                            return result;
                        }

                        break;
                    case TChat myChat:
                        if (myChat.Title == chatTitle)
                        {
                            if (myChat.MigratedTo == null)
                            {
                                result.ChatId = myChat.Id;
                                return result;
                            }

                            result.ChannelType = true;
                            return result;
                        }

                        break;
                    case TChatForbidden myChatF:
                        if (myChatF.Title == chatTitle)
                        {
                            result.ChatId = myChatF.Id;
                            return result;
                        }

                        break;
                }
            }

            return result;
        }

        public async Task<IEnumerable<TContact>> Contacts()
        {
            var result = await _client.ContactsService.GetContactsAsync();
            return result.Contacts.Select(x => (OpenTl.Schema.TContact) x);
        }

        public async Task<TUserFull> FindUser(int userId, long accessHash = 0)
        {
            return await _client.UsersService.GetUserFullAsync(
                new TInputUser()
                {
                    UserId = userId,
                    AccessHash = accessHash
                });
        }

        public async Task<TUser> FindUserByUsername(string username)
        {
            var result = await _client.CustomRequestsService.SendRequestAsync(new RequestResolveUsername()
            {
                Username = username
            });

            return (TUser) result.Users.First();
        }

        public async Task<bool> AddUserToChannel(int userId, int channelId, long accessHash, long accessHashChannel
            , Guid correlationId)
        {
            var requestInvite = new RequestInviteToChannel();
            requestInvite.Channel = new TInputChannel()
            {
                ChannelId = channelId,
                AccessHash = accessHashChannel
            };
            requestInvite.Users = new TVector<IInputUser>(new TInputUser()
            {
                UserId = userId,
                AccessHash = accessHash
            });

            try
            {
                await _client.CustomRequestsService.SendRequestAsync(requestInvite);
            }
            catch (Exception ex)
            {
                Log.LogErr($"TelegramBot: AddUserToChannel {ex.Message}", correlationId);
                return false;
            }

            return true;
        }


        public async Task<bool> AddUserToChat(int userId, int chatId, long accessHash
            , Guid correlationId)
        {
            IInputUser user = new TInputUser()
            {
                UserId = userId,
                AccessHash = accessHash
            };

            try
            {
                await _client.MessagesService.AddChatUserAsync(chatId, user, 0);
            }
            catch (Exception ex)
            {
                Log.LogErr($"TelegramBot: AddUserToChat {ex.Message}", correlationId);
                return false;
            }

            return true;
        }

        public async Task<TChannelParticipants> GetParticipants(int channelId, long accessHashChannel
            , Guid correlationId, int limit = 100, int offset = 0, int lastUserId = 0)
        {
            var requestGetParticipants = new RequestGetParticipants();
            requestGetParticipants.Channel = new TInputChannel()
            {
                ChannelId = channelId,
                AccessHash = accessHashChannel
            };
            var filter = new TChannelParticipantsSearch();
            filter.Q = "";
            requestGetParticipants.Hash = lastUserId;
            requestGetParticipants.Limit = limit;
            requestGetParticipants.Offset = offset;
            requestGetParticipants.Filter = filter;

            try
            {
                return (TChannelParticipants) await _client.CustomRequestsService.SendRequestAsync(
                    requestGetParticipants);
            }
            catch (Exception ex)
            {
                Log.LogErr($"TelegramBot: GetParticipants {ex.Message}", correlationId);
                return null;
            }
        }

        public async Task<List<TUser>> GetAllParticipants(int channelId, long accessHashChannel
            , Guid correlationId)
        {
            try
            {
                var users = await SearchParticipants(channelId, accessHashChannel, "", Guid.NewGuid());

                if (users.Users.Count < 200)
                {
                    return users.Users.Select(x => x as TUser).ToList();
                }

                var result = new List<TUser>();
                string alphabet = "abcdefghijklmnopqrstuvwxyz";

                foreach (char c in alphabet)
                {
                    users = await SearchParticipants(channelId, accessHashChannel, c.ToString(), Guid.NewGuid());

                    foreach (var usersUser in users.Users)
                    {
                        var user2 = usersUser as TUser;
                        if (string.IsNullOrEmpty(user2.Username))
                        {
                            if (!(result.Any(x => x.FirstName == user2.FirstName)))
                            {
                                result.Add(user2);
                            }
                        }
                        else
                        {
                            if (!(result.Any(x => x.Username == user2.Username)))
                            {
                                result.Add(user2);
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.LogErr($"TelegramBot: GetAllParticipants {ex.Message}", correlationId);
                return null;
            }
        }

        public async Task<TChannelParticipants> SearchParticipants(int channelId, long accessHashChannel, string query
            , Guid correlationId, int limit = 200, int offset = 0, int lastUserId = 0)
        {
            var requestGetParticipants = new RequestGetParticipants();
            requestGetParticipants.Channel = new TInputChannel()
            {
                ChannelId = channelId,
                AccessHash = accessHashChannel
            };
            var filter = new TChannelParticipantsSearch();
            filter.Q = query;
            requestGetParticipants.Hash = lastUserId;
            requestGetParticipants.Limit = limit;
            requestGetParticipants.Offset = offset;
            requestGetParticipants.Filter = filter;

            try
            {
                return (TChannelParticipants) await _client.CustomRequestsService.SendRequestAsync(
                    requestGetParticipants);
            }
            catch (Exception ex)
            {
                Log.LogErr($"TelegramBot: GetParticipants {ex.Message}", correlationId);
                return null;
            }
        }

        public async Task<TChannelParticipants> GetRecentUsers(int numUsers, int channelId, long accessHashChannel
            , Guid correlationId, int offset)
        {
            var request = new RequestGetParticipants()
            {
                Channel = new TInputChannel()
                {
                    AccessHash = accessHashChannel,
                    ChannelId = channelId
                },
                Filter = new TChannelParticipantsRecent(),
                Limit = numUsers,
                Offset = offset
            };
            try
            {
                return (TChannelParticipants) await _client.CustomRequestsService.SendRequestAsync(request);
            }
            catch (Exception ex)
            {
                Log.LogErr($"TelegramBot: GetParticipants {ex.Message}", correlationId);
                return null;
            }
        }

        public async Task<bool> RemoveUserFromChat(int userId, int chatId, long accessHash,
            Guid correlationId)
        {
            IInputUser user = new TInputUser()
            {
                UserId = userId,
                AccessHash = accessHash
            };

            try
            {
                var result = await _client.MessagesService.DeleteChatUser(chatId, user);
            }
            catch (Exception ex)
            {
                Log.LogErr($"TelegramBot: RemoveUserFromChat {ex.Message}", correlationId);
                return false;
            }

            return true;
        }

        public async Task<bool> SendMessage(int userId, long accessHash, string message,
            Guid correlationId)
        {
            TInputPeerUser user = new TInputPeerUser()
            {
                UserId = userId,
                AccessHash = accessHash
            };
            try
            {
                var result = await _client.MessagesService.SendMessageAsync(user, message);
            }
            catch (Exception ex)
            {
                Log.LogErr($"TelegramBot: SendMessage {ex.Message}", correlationId);
                return false;
            }

            return true;
        }

        public async Task<bool> BanUserFromChannel(int userId, int channelId, long accessHash, long accessHashChannel
            , Guid correlationId)
        {
            var leaveChannel = new RequestEditBanned
            {
                Channel = new TInputChannel() {ChannelId = channelId, AccessHash = accessHashChannel},
                UserId = new TInputUser() {UserId = userId, AccessHash = accessHash},
                BannedRights = new TChannelBannedRights() {ViewMessages = true, UntilDate = 0}
            };

            try
            {
                await _client.CustomRequestsService.SendRequestAsync(leaveChannel);
            }
            catch (Exception ex)
            {
                Log.LogErr($"TelegramBot: BanUserFromChannel {ex.Message}", correlationId);
                return false;
            }

            return true;
        }


        public async Task<bool> UnBanUserFromChannel(int userId, int channelId, long accessHash, long accessHashChannel
            , Guid correlationId)
        {
            var leaveChannel = new RequestEditBanned
            {
                Channel = new TInputChannel() {ChannelId = channelId, AccessHash = accessHashChannel},
                UserId = new TInputUser() {UserId = userId, AccessHash = accessHash},
                BannedRights = new TChannelBannedRights() {ViewMessages = false, UntilDate = 0}
            };

            try
            {
                await _client.CustomRequestsService.SendRequestAsync(leaveChannel);
            }
            catch (Exception ex)
            {
                Log.LogErr($"TelegramBot: UnBanUserFromChannel {ex.Message}", correlationId);
                return false;
            }

            return true;
        }

        public async Task<bool> SendImage(int userId, long userAccessHash, string imageName, Guid correlationId, string message = "")
        {
            TInputPeerUser user = new TInputPeerUser()
            {
                UserId = userId,
                AccessHash = userAccessHash
            };
            try
            {
                IInputFile image;
                using (var stream = new MemoryStream(System.IO.File.ReadAllBytes($"wwwroot\\Menu\\{imageName}")))
                {
                    image = await _client.FileService.UploadFileAsync(imageName, stream);
                }
                var result = await _client.MessagesService.SendUploadedPhotoAsync(user,image,message);
            }
            catch (Exception ex)
            {
                Log.LogErr($"TelegramBot: SendImage {ex.Message}", correlationId);
                return false;
            }

            return true;
        }
    }

    public class ChatData
    {
        public bool ChannelType { get; set; }
        public int ChatId { get; set; }
        public long AccessHash { get; set; }
    }
}