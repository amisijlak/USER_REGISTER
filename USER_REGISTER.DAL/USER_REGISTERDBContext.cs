using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using USER_REGISTER.DAL.Security;

namespace USER_REGISTER.DAL
{
    public class USER_REGISTERDBContext : IdentityDbContext<ApplicationUser, SecurityRole, string>
    {
        public string ConnectionString => _connectionString;
        private readonly string _connectionString;

        public USER_REGISTERDBContext(DbContextOptions<USER_REGISTERDBContext> options) : base(options)
        {
            if (options != null)
            {
                //extract connnection string
                var extension = options.FindExtension<SqlServerOptionsExtension>();
                _connectionString = extension.ConnectionString;
            }
        }

        private void CreateIndex<T>(ModelBuilder builder, Expression<Func<T, object>> KeyPropertyExpression, string IndexName = "IX_Unique", bool IsUnique = true)
            where T : class
        {
            var indexBuilder = builder.Entity<T>().HasIndex(KeyPropertyExpression);
            if (!string.IsNullOrEmpty(IndexName)) indexBuilder.HasName(IndexName);
            if (IsUnique) indexBuilder.IsUnique();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Call the base class first:
            base.OnModelCreating(builder);
        }
    }
}
