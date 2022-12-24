using AutoMapper;
using BusinnesLogicLayer.DTO;
using BusinnesLogicLayer.Interfaces;
using DataLayer.Data.Entities;
using DataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinnesLogicLayer.Services
{
    public class MessageUserService : IService<MessageUserDTO>
    {
        private readonly IWorkMessageUser _userDB;
        public MessageUserService(IWorkMessageUser database)
        {
            this._userDB = database;
        }
        public async Task AddItemAsync(object item)
        {
            if (item is MessageUserDTO)
                await AddUser((MessageUserDTO)item);
        }

        private async Task AddUser(MessageUserDTO item)
        {
            var contact = MappingModels(item);

            await _userDB.MessageUsers.Create(contact);
            await _userDB.Save();
        }

        public async Task AddList(IEnumerable<MessageUserDTO> templist)
        {
            if (templist != null)
            {
                foreach (var item in templist)
                {
                    await AddUser(item);
                }
            }
        }

        public async Task DeleteDTO(MessageUserDTO id)
        {
            _userDB.MessageUsers.Delete(MappingModels(id));
            await _userDB.Save();
        }

        public async Task<MessageUserDTO> FindDTO(MessageUserDTO id)
        {
            return MappingModels(await _userDB.MessageUsers.Find(MappingModels(id)));
        }

        public IEnumerable<MessageUserDTO> GetAll()
        {
            List<MessageUserDTO> list = new List<MessageUserDTO>();
            foreach (var item in _userDB.MessageUsers.GetAll())
                list.Add(MappingModels(item));
            return list;
        }

        public int GetIdDTO(MessageUserDTO item)
        {
            throw new InvalidOperationException();
        }

        public Task UpdateDTO(MessageUserDTO item)
        {
            throw new InvalidOperationException();
        }

        private MessageUserDTO MappingModels(MessageUserEntity user)
        {
            //var firstobj = new MapperConfiguration(map => map.CreateMap<UserEntity, UserDTO>()).CreateMapper();
            //var secondobj = new MapperConfiguration(map => map.CreateMap<MessageEntity, MessageDTO>()).CreateMapper();

            return new MessageUserDTO()
            {
                FromUser = new UserDTO()
                {
                    Email = user.FromUser.Email,
                    EmailConfirmed = user.FromUser.EmailConfirmed,
                    Login = user.FromUser.Login,
                    Password = user.FromUser.Password,
                    Photo = user.FromUser.Photo,
                    Id = user.FromUser.Id
                },
                ToUser = new UserDTO()
                {
                    Email = user.ToUser.Email,
                    EmailConfirmed = user.ToUser.EmailConfirmed,
                    Login = user.ToUser.Login,
                    Password = user.ToUser.Password,
                    Photo = user.ToUser.Photo,
                    Id = user.ToUser.Id
                },
                Message = new MessageDTO()
                {
                    Message = user.Message.Message,
                    Time = user.Message.Time,
                    User = new UserDTO()
                    {
                        Email = user.Message.User.Email,
                        EmailConfirmed = user.Message.User.EmailConfirmed,
                        Login = user.Message.User.Login,
                        Password = user.Message.User.Password,
                        Photo = user.Message.User.Photo,
                        Id = user.Message.User.Id
                    },
                    Id = user.Message.Id
                }
            };
        }
        private MessageUserEntity MappingModels(MessageUserDTO user)
        {
            //var firstobj = new MapperConfiguration(map => map.CreateMap<UserDTO, UserEntity>()).CreateMapper();
            //var secondobj = new MapperConfiguration(map => map.CreateMap<MessageDTO, MessageEntity>()).CreateMapper();

            return new MessageUserEntity()
            {
                FromUser = new UserEntity()
                {
                    Email = user.FromUser.Email,
                    EmailConfirmed= user.FromUser.EmailConfirmed,
                    Login= user.FromUser.Login,
                    Password= user.FromUser.Password,
                    Photo= user.FromUser.Photo,
                    Id = user.FromUser.Id
                },
                ToUser = new UserEntity()
                {
                    Email= user.ToUser.Email,
                    EmailConfirmed= user.ToUser.EmailConfirmed,
                    Login= user.ToUser.Login,
                    Password= user.ToUser.Password,
                    Photo= user.ToUser.Photo, 
                    Id= user.ToUser.Id
                },
                Message = new MessageEntity()
                {
                    Message=user.Message.Message,
                    Time = user.Message.Time,
                    User = new UserEntity()
                    {
                        Email= user.Message.User.Email,
                        EmailConfirmed= user.Message.User.EmailConfirmed,
                        Login= user.Message.User.Login,
                        Password= user.Message.User.Password,
                        Photo= user.Message.User.Photo,
                        Id = user.Message.User.Id
                    }
                }
            };
        }
    }
}
