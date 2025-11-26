using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Repository;

namespace V_Quiz_Backend.Services
{
    public class CustomerService
    {
        private readonly CustomerRepository _repo;
        public CustomerService(CustomerRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _repo.GetAllCustomersAsync();
        }
    }
}
