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
    public class UserService : IService<UserDTO>
    {
        private readonly IWorkUser _userDB;
        public UserService(IWorkUser database)
        {
            this._userDB = database;
        }
        public async Task AddItemAsync(object item)
        {
            if (item is UserDTO)
                await AddUser((UserDTO)item);
        }

        private async Task AddUser(UserDTO item)
        {
            var contact = MappingModels(item);

            await _userDB.Users.Create(contact);
            await _userDB.Save();
        }

        public async Task AddList(IEnumerable<UserDTO> templist)
        {
            if (templist != null)
            {
                foreach (var item in templist)
                {
                    await AddUser(item);
                }
            }
        }

        public async Task DeleteDTO(UserDTO id)
        {
            _userDB.Users.Delete(MappingModels(id));
            await _userDB.Save();
        }

        public async Task<UserDTO> FindDTO(UserDTO id)
        {
            return MappingModels(await _userDB.Users.Find(MappingModels(id)));
        }

        public IEnumerable<UserDTO> GetAll()
        {
            List<UserDTO> list = new List<UserDTO>();
            foreach (var item in _userDB.Users.GetAll())
                list.Add(MappingModels(item));
            return list;
        }

        public int GetIdDTO(UserDTO item)
        {
            return _userDB.Users.GetId(MappingModels(item));
        }

        public async Task UpdateDTO(UserDTO item)
        {
            _userDB.Users.Update(MappingModels(item));
            await _userDB.Save();
        }

        private UserDTO MappingModels(UserEntity user)
        {
            var firstobj = new MapperConfiguration(map => map.CreateMap<UserEntity, UserDTO>()).CreateMapper();
            return firstobj.Map<UserDTO>(user);
        }
        private UserEntity MappingModels(UserDTO user)
        {
            var firstobj = new MapperConfiguration(map => map.CreateMap<UserDTO, UserEntity>()).CreateMapper();
            return firstobj.Map<UserEntity>(user);
        }
    }
}
