using EcomApi.Application.DTO;
using EcomApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Application.Interfaces
{
    public interface IAddressService
    {
        Task<Address> CreateAsync(CreateAddressDTO dto);
        Task<List<Address>> GetAddressAsync();

        Task<Address> getAddress(int id);
    }
}
