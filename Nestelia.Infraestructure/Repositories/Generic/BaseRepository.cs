using System.Data;
using System.Linq.Expressions;
using System.Security.Claims;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Nestelia.Domain.Entities.Audit;
using Nestelia.Infraestructure.Common;

namespace Nestelia.Infraestructure.Repositories.Generic
{
    public class BaseRepository<T> where T : class
    {
        /// <summary>
        /// The context
        /// </summary>
        protected readonly ApplicationDbContext Context;
        private readonly ClaimsPrincipal _user;
        private string tableName;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{T}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public BaseRepository(ApplicationDbContext context, ClaimsPrincipal user)
        {
            Context = context;
            var model = context.Model;

            var entityTypes = model.GetEntityTypes();

            try
            {
                var entityTypeOfFooBar = entityTypes.First(t => t.ClrType == typeof(T));
                var tableNameAnnotation = entityTypeOfFooBar.GetAnnotation("Relational:TableName");
                var tableNameOfFooBarSet = tableNameAnnotation.Value.ToString();
                tableName = tableNameOfFooBarSet;
            }
            catch (Exception ex)
            {
                //Una entidad que hereda de base entity que no tiene tabla
            }
            _user = user;

        }

        /// <summary>
        /// Inserts the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public virtual async Task<Guid> InsertAsync(T entity)
        {
            var createdAtProperty = typeof(T).GetProperty("CreatedAt");
            if (createdAtProperty != null && createdAtProperty.PropertyType == typeof(DateTime))
            {
                createdAtProperty.SetValue(entity, DateTime.UtcNow);
            }

            Context.Set<T>().Add(entity);
            await Context.SaveChangesAsync();

            var idRow = (Guid)typeof(T).GetProperty("Id")?.GetValue(entity);

            AuditChanges audit = new AuditChanges()
            {
                Id = Guid.NewGuid(),
                Action = "INSERT",
                TableName = tableName,
                OldValue = "",
                NewValue = JsonConvert.SerializeObject(entity),
                User = this.GetIdUser(),
                Role = this.GetRol(),
                IPAddress = "",
                RowVersion = DateTime.Now,
                IsDeleted = false,
                IdEntity = idRow,
                CreatedAt = createdAtProperty != null ? (DateTime)createdAtProperty.GetValue(entity) : DateTime.Now
            };

            await AuditTable(audit);

            return idRow;
        }


        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(T entity)
        {
            Context.Set<T>().Update(entity);

            var result = await Context.SaveChangesAsync();

            if (result != 0)
            {
                var idRow = (Guid)typeof(T).GetProperty("Id")?.GetValue(entity);
                AuditChanges audit = new AuditChanges()
                {
                    Id = Guid.NewGuid(),
                    Action = "UPDATE",
                    TableName = tableName,
                    OldValue = "",
                    NewValue = JsonConvert.SerializeObject(entity),
                    User = this.GetIdUser(),
                    Role = this.GetRol(),
                    IPAddress = "" +
                    "",
                    RowVersion = DateTime.Now,
                    IsDeleted = false,
                    IdEntity = idRow,
                    CreatedAt = DateTime.Now
                };
                await AuditTable(audit);
            }

            return result;
        }

        /// <summary>
        /// Removes the asynchronous (soft delete).
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public virtual async Task<int> RemoveAsync(T entity)
        {
            if (typeof(T).GetProperty("IsDeleted") != null)
            {
                typeof(T).GetProperty("IsDeleted")?.SetValue(entity, true);
                Context.Set<T>().Update(entity);
            }
            else
            {
                Context.Set<T>().Remove(entity);
            }

            var result = await Context.SaveChangesAsync();

            if (result != 0)
            {
                AuditChanges audit = new()
                {
                    Id = Guid.NewGuid(),
                    Action = "DELETE",
                    TableName = tableName,
                    OldValue = string.Empty,
                    NewValue = string.Empty,
                    User = this.GetIdUser(),
                    Role = this.GetRol(),
                    IPAddress = "",
                    RowVersion = DateTime.Now,
                    IsDeleted = false,
                    IdEntity = (Guid)entity.GetType().GetProperty("Id").GetValue(entity),
                    CreatedAt = DateTime.Now
                };

                await AuditTable(audit);
            }
            return result;
        }

        /// <summary>
        /// Removes the asynchronous by id (soft delete).
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public virtual async Task<int> RemoveAsync(Guid id)
        {
            var entity = await Context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                if (typeof(T).GetProperty("IsDeleted") != null)
                {
                    typeof(T).GetProperty("IsDeleted")?.SetValue(entity, true);
                    Context.Set<T>().Update(entity);
                }
                else
                {
                    Context.Set<T>().Remove(entity);
                }

                var result = await Context.SaveChangesAsync();

                if (result != 0)
                {
                    AuditChanges audit = new()
                    {
                        Id = Guid.NewGuid(),
                        Action = "DELETE",
                        TableName = tableName,
                        OldValue = string.Empty,
                        NewValue = string.Empty,
                        User = this.GetIdUser(),
                        Role = this.GetRol(),
                        IPAddress = "",
                        RowVersion = DateTime.Now,
                        IsDeleted = false,
                        IdEntity = id,
                        CreatedAt = DateTime.Now
                    };

                    await AuditTable(audit);
                }
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = Context.Set<T>();

            var isDeletable = typeof(T).GetProperty("IsDeleted") != null;

            if (isDeletable)
            {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }


        /// <summary>
        /// Gets the single asynchronous.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public virtual async Task<T?> GetSingleAsync(Expression<Func<T, bool>>? filter = null)
        {
            var query = Context.Set<T>().AsQueryable();

            if (typeof(T).GetProperty("IsDeleted") != null)
            {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }
        
        public virtual async Task<T?> GetSingleWithRelationsAsync(
            Expression<Func<T, bool>>? filter = null,
            params Expression<Func<T, object>>[] includes)
        {
            var query = Context.Set<T>().AsQueryable();

            if (includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            else
            {
                var navigations = Context.Model.FindEntityType(typeof(T))?
                    .GetNavigations()
                    .Select(n => n.Name)
                    .ToList();

                if (navigations != null)
                {
                    foreach (var navigation in navigations)
                    {
                        query = query.Include(navigation);
                    }
                }
            }
            
            if (typeof(T).GetProperty("IsDeleted") != null)
            {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            } ;

            return query.FirstOrDefault();
        }

        public string GetIdUser()
        {
            if (this._user == null || !this._user.Identity.IsAuthenticated)
            {
                return string.Empty;
            }
            return this._user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }

        public string GetRol()
        {
            if (this._user == null || !this._user.Identity.IsAuthenticated)
            {
                return string.Empty;
            }
            return this._user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value;
        }

        public async Task<int> AuditTable(AuditChanges audit)
        {
            var sql = @"INSERT INTO [dbo].[Tbl_AuditChanges]
                                   ([Id]
                                   ,[TableName]
                                   ,[OldValue]
                                   ,[NewValue]
                                   ,[User]
                                   ,[Role]
                                   ,[IPAddress]
                                   ,[RowVersion]
                                   ,[IsDeleted]
                                   ,[IdEntity]
                                   ,[Action]
                                   ,[CreatedAt])
                             VALUES
                                   (@Id
                                   ,@TableName
                                   ,@OldValue
                                   ,@NewValue
                                   ,@User
                                   ,@Role
                                   ,@IPAddress
                                   ,@RowVersion
                                   ,@IsDeleted
                                   ,@IdEntity
                                   ,@Action
                                   ,@CreatedAt)";


            var result = await Context.Database.GetDbConnection().QueryAsync<int>(sql, audit);
            return result.FirstOrDefault();
        }

        public async Task<int> AuditTable(AuditChanges audit, IDbTransaction transaction = null)
        {
            var sql = @"INSERT INTO [dbo].[Tbl_AuditChanges]
                                   ([Id]
                                   ,[TableName]
                                   ,[OldValue]
                                   ,[NewValue]
                                   ,[User]
                                   ,[Role]
                                   ,[IPAddress]
                                   ,[RowVersion]
                                   ,[IsDeleted]
                                   ,[IdEntity]
                                   ,[Action]
                                   ,[CreatedAt])
                             VALUES
                                   (@Id
                                   ,@TableName
                                   ,@OldValue
                                   ,@NewValue
                                   ,@User
                                   ,@Role
                                   ,@IPAddress
                                   ,@RowVersion
                                   ,@IsDeleted
                                   ,@IdEntity
                                   ,@Action
                                   ,@CreatedAt)";

            return await transaction.Connection.ExecuteAsync(sql, audit, transaction);

        }


    }
}
