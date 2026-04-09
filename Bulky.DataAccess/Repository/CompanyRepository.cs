using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly AppDbContext _DB;
        public CompanyRepository(AppDbContext DB) : base(DB)
        {
            _DB = DB;
        }

        public void Update(Company company)
        {
            _DB.Companies.Update(company);
        }
    }
}
