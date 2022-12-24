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
            var firstobj = new MapperConfiguration(map => map.CreateMap<UserEntity, UserDTO>()).CreateMapper();
            var secondobj = new MapperConfiguration(map => map.CreateMap<MessageEntity, MessageDTO>()).CreateMapper();

            return new MessageUserDTO()
            {
                FromUser = firstobj.Map<UserDTO>(user.FromUser),
                UserFromId = user.UserFromId,
                ToUser = firstobj.Map<UserDTO>(user.ToUser),
                UserToId= user.UserToId,
                Message = secondobj.Map<MessageDTO>(user.Message),
                MessageId = user.MessageId
            };
        }
        private MessageUserEntity MappingModels(MessageUserDTO user)
        {
            var firstobj = new MapperConfiguration(map => map.CreateMap<UserDTO, UserEntity>()).CreateMapper();
            var secondobj = new MapperConfiguration(map => map.CreateMap<MessageDTO, MessageEntity>()).CreateMapper();

            return new MessageUserEntity()
            {
                FromUser = firstobj.Map<UserEntity>(user.FromUser),
                UserFromId = user.UserFromId,
                ToUser = firstobj.Map<UserEntity>(user.ToUser),
                UserToId = user.UserToId,
                Message = secondobj.Map<MessageEntity>(user.Message),
                MessageId = user.MessageId
            };
        }
    }
}
