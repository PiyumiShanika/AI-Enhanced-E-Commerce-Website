using EcomApi.Application.DTO;
using EcomApi.Application.Interfaces;
using EcomApi.Domain.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Application.Services
{
    public class AddressService : IAddressService
    {

        private readonly ITokenService _tokenService;
        private readonly ITransaction _transaction;


        // Constructor to initialize dependencies
        public AddressService(ITokenService tokenService, ITransaction transaction)
        {

            _tokenService = tokenService;
            _transaction = transaction;
        }

        // Method to create a new address
        public async Task<Address> CreateAsync(CreateAddressDTO dto)
        {
            try
            {
                await _transaction.BeginTransactionAsync();

                // Retrieve the current logged-in user's ID
                var curreLogedUser = _tokenService.GetUserId();

                var myEntity = new Address
                {

                    Country = dto.Country,
                    City = dto.City,
                    Street = dto.Street,
                    Postal_Code = dto.Postal_Code,
                    User_Id = curreLogedUser,
                    isPrimary = false

                };

                // Add the entity to the context
                await _transaction.GetRepository<Address>().AddAsync(myEntity);
                await _transaction.SaveChangesAsync();
                await _transaction.CommitAsync();

                Log.Information("Address created successfully for User ID: {UserID}, Address ID: {AddressID}", curreLogedUser);
                return myEntity;
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                Log.Error(ex, "Error creating address for User ID: {UserID}", _tokenService.GetUserId());
                throw new ApplicationException("Error creating entity", ex);
            }

        }

        // Method to retrieve all addresses for the current logged-in user
        public async Task<List<Address>> GetAddressAsync()
        {

            var curreLogedUser = _tokenService.GetUserId();
            Log.Information("Retrieving addresses for User ID: {UserID}", curreLogedUser);

            // Query the database for addresses associated with the current use

            var addresses = await _transaction.GetRepository<Address>().GetByIdAsync(curreLogedUser, a => a.User_Id == curreLogedUser);
            return addresses;

        }

        // Method to retrieve a specific address by its ID
        public async Task<Address> getAddress(int id)
        {
            // var address = await _baseRepo.GetById(id);
            var address = await _transaction.GetRepository<Address>().GetById(id);
            Log.Information("Retrieved address with ID: {AddressID}", id);
            return address;

        }

    }
}
