using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using RESTApi.Models;
using RESTApi.Extensions;
using RESTApi.Models.DTOs;
using RESTApi.Constants;

namespace RESTApi.Repository
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : class
    {
        /// <summary>
        /// Kontekst bazy danych
        /// </summary>
        protected readonly ApplicationDbContext _context;
        protected readonly IHttpContextAccessor _httpContext;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">Context bazy danych</param>
        /// <param name="logger">Logger - Windsor</param>
        public GenericRepository(ApplicationDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        // Metody synchroniczne
        /// <summary>
        /// Pobranie wszystkich rekordów
        /// </summary>
        /// <returns>Lista obiektów</returns>
        public IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsQueryable();
        }

        /// <summary>
        /// Odfiltrowanie rekordów na podstawie podanego wyrażenia
        /// </summary>
        /// <param name="expression">Wyrażenie</param>
        /// <returns></returns>
        public IQueryable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }

        /// <summary>
        /// Odfiltrowanie rekordów na podstawie podanego wyrażenia
        /// </summary>
        /// <param name="expression">Wyrażenie</param>
        /// <returns></returns>
        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(expression);
        }

        /// <summary>
        /// Odfiltrowanie rekordów na podstawie podanego wyrażenia wraz z OrderBy
        /// </summary>
        /// <param name="expression">Wyrażenie</param
        /// <param name="orderColumn">Kolumna do OrderBy</param>
        /// <param name="orderDirection">Kolejność do OrderBy</param>
        /// <returns></returns>
        public virtual async Task<T> FirstOrDefaultAsyncOrdered(Expression<Func<T, bool>> expression, string orderColumn, bool orderDirection)
        {
            return await _context.Set<T>().OrderBy(orderColumn, orderDirection).FirstOrDefaultAsync(expression);
        }

        /// <summary>
        /// Odfiltrowanie rekordów na podstawie podanego wyrażenia
        /// </summary>
        /// <param name="expression">Wyrażenie</param>
        /// <returns></returns>
        public T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().FirstOrDefault(expression);
        }

        /// <summary>
        /// Odfiltrowanie rekordów na podstawie podanego wyrażenia
        /// </summary>
        /// <param name="context">Kontekst bazy danych</param>
        /// <param name="expression">Wyrażenie</param>
        /// <returns></returns>
        public virtual IQueryable<T> Find(DbContext context, Expression<Func<T, bool>> expression)
        {
            return context.Set<T>().Where(expression);
        }

        /// <summary>
        /// Pobieranie rekordów
        /// </summary>
        /// <param name="filteredCount">Ilość rekordów po przefiltrowaniu</param>
        /// <param name="totalCount">Ilość wszystkich rekordów</param>
        /// <returns>Przefiltrowana kolekcja</returns>
        public IQueryable<T> Search(ListRequestDTO model, out int filteredCount, out int totalCount)
        {
            var entities = _context.Set<T>().AsQueryable();
            totalCount = entities.Count();

            entities = Filter(entities, model.FilterObject);

            filteredCount = entities.Count();

            if (model.SortObject != null)
                entities = Sort(entities, model.SortObject.Value.Key, model.SortObject.Value.Value);
            if (!model.Page.HasValue || !model.PerPage.HasValue)
            {
                return entities;
            }

            return entities.Skip(model.Page.Value * model.PerPage.Value).Take(Math.Min(model.PerPage.Value, filteredCount == 0 ? 1 : filteredCount));
        }

        /// <summary>
        /// Sortowanie listy wyników
        /// </summary>
        /// <param name="entities">Lista rekordów</param>
        /// <param name="sortCol">Kolumna, po której dane są sortowane</param>
        /// <param name="sortDir">Kierunek sortowania</param>
        /// <returns>Posortowana lista rekordów</returns>
        protected IQueryable<T> Sort(IQueryable<T> entities, string sortCol, string sortDir)
        {
            var sort = sortDir == SearchParams.SORT_ASC ?
                entities.OrderBy(sortCol) : entities.OrderBy(sortCol, false);
            return sort;
        }

        /// <summary>
        /// Filtrowanie wartości
        /// </summary>
        protected IQueryable<T> Filter(IQueryable<T> entities, Dictionary<string, string> filterParams)
        {
            entities = FilterDeleted(entities);
            if (filterParams == null)
                return entities;

            foreach (var filterParam in filterParams)
            {
                var colName = filterParam.Key;
                var colVal = filterParam.Value;
                if (colName == "q")
                    entities = entities.FilterString("AutocompleteSearch", colVal.ToLower());
                if (colName.StartsWith(SearchParams.SEARCH_STRING))
                    entities = entities.FilterString(colName.Replace(SearchParams.SEARCH_STRING, string.Empty), colVal.ToLower());

                else if (colName.StartsWith(SearchParams.SEARCH_DATE_FROM))
                    entities = entities.FilterDateFrom(colName.Replace(SearchParams.SEARCH_DATE_FROM, string.Empty), colVal);

                else if (colName.StartsWith(SearchParams.SEARCH_DATE_TO))
                    entities = entities.FilterDateTo(colName.Replace(SearchParams.SEARCH_DATE_TO, string.Empty), colVal);

                else if (colName.StartsWith(SearchParams.SEARCH_DATE_BETWEEN))
                {
                    var colNames = colName.Replace(SearchParams.SEARCH_DATE_BETWEEN, string.Empty).Split("%");
                    var colValue = bool.Parse(colVal);
                    entities = entities.FilterDateValid(colNames[0], colNames[1], colValue);

                }
                else if (colName.StartsWith(SearchParams.SEARCH_DATE))
                    entities = entities.FilterDateEqual(colName.Replace(SearchParams.SEARCH_DATE, string.Empty), colVal);

                else if (colName.StartsWith(SearchParams.SEARCH_EQUAL) && colName.ValidateSearchParam())
                    entities = entities.FilterEqualValue(colName.Replace(SearchParams.SEARCH_EQUAL, string.Empty), colVal);

                else if (colName.StartsWith(SearchParams.SEARCH_NOT_EQUAL) && colName.ValidateSearchParam())
                    entities = entities.FilterNotEqualValue(colName.Replace(SearchParams.SEARCH_NOT_EQUAL, string.Empty), colVal);

                else if (colName.StartsWith(SearchParams.SEARCH_NOT_NULL) && !string.IsNullOrEmpty(colVal))
                    entities = entities.FilterNull(colName.Replace(SearchParams.SEARCH_NOT_NULL, string.Empty), colVal, false);

                else if (colName.StartsWith(SearchParams.SEARCH_DATE_TIME_FROM))
                    entities = entities.FilterDateTimeFrom(colName.Replace(SearchParams.SEARCH_DATE_TIME_FROM, string.Empty), colVal);

                else if (colName.StartsWith(SearchParams.SEARCH_DATE_TIME_TO))
                    entities = entities.FilterDateTimeTo(colName.Replace(SearchParams.SEARCH_DATE_TIME_TO, string.Empty), colVal);

                else if (colName.StartsWith(SearchParams.SEARCH_IN))
                    entities = entities.FilterIn(colName.Replace(SearchParams.SEARCH_IN, string.Empty), colVal);

                else if (colName.StartsWith(SearchParams.SEARCH_NOT_IN))
                    entities = entities.FilterNotIn(colName.Replace(SearchParams.SEARCH_NOT_IN, string.Empty), colVal);
            }

            return entities;
        }

        private IQueryable<T> FilterDeleted(IQueryable<T> entities)
        {
            if (entities.Count() > 0)
            {
                var deletedColumn = entities.FirstOrDefault().GetDeletedColumn();
                if (deletedColumn != null)
                {
                    entities = entities.FilterEqualValue(deletedColumn.Name, "false");
                }
            }
            return entities;

        }

        /// <summary>
        /// Pobranie rekordu po identyfikatorze - asynchronicznie
        /// </summary>
        /// <param name="id">Identyfikator</param>
        /// <returns>Znaleziony obiekt</returns>
        public virtual async Task<T> GetByIdAsync(TKey id)
        {
            var obj = await _context.Set<T>().FindAsync(id);

            return obj;

        }

        // Metody asynchroniczne

        /// <summary>
        /// Dodanie rekordu
        /// </summary>
        /// <param name="entity">Obiekt do dodania</param>
        /// <returns>Dodany obiekt</returns>
        public async Task<T> AddAsync(T entity)
        {
            var entry = await _context.Set<T>().AddAsync(entity);
            await SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Dodanie rekordu
        /// </summary>
        /// <param name="context">Kontekst bazy danych</param>
        /// <param name="entity">Obiekt do dodania</param>
        /// <returns>Dodany obiekt</returns>
        public async Task<T> AddAsync(DbContext context, T entity)
        {
            var entry = await context.Set<T>().AddAsync(entity);
            await SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Dodanie wielu rekordów
        /// </summary>
        /// <param name="entities">Obiekty do dodania</param>
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Pobranie rekordu po identyfikatorze
        /// </summary>
        /// <param name="id">Identyfikator</param>
        /// <returns>Znaleziony obiekt</returns>
        public T GetById(TKey id)
        {
            return _context.Set<T>().Find(id);

        }

        /// <summary>
        /// Usunięcie rekordu
        /// </summary>
        /// <param name="entity">Obiekt do usunięcia</param>
        public async Task RemoveAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Usunięcie wielu rekordów
        /// </summary>
        /// <param name="entities">Obiekty do usunięcia</param>
        public async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Aktualizacja rekordu
        /// </summary>
        /// <param name="entity">Obiekt aktualizowany</param>
        /// <returns>Zaktualizowany obiekt</returns>
        public async Task<T> UpdateAsync(T entity)
        {
            var entry = _context.Entry<T>(entity);
            if (entry.State == EntityState.Detached)
                throw new Exception("EntityState.Detached, data not updated.");
            await SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Aktualizacja rekordu
        /// </summary>
        /// <param name="context">Kontekst bazy danych</param>
        /// <param name="entity">Obiekt aktualizowany</param>
        /// <returns>Zaktualizowany obiekt</returns>
        public async Task<T> UpdateAsync(DbContext context, T entity)
        {
            var entry = context.Entry(entity);
            if (entry.State == EntityState.Detached)
                throw new Exception("EntityState.Detached, data not updated.");
            await SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<int> SaveChangesAsync()
        {
            var userId = _httpContext.HttpContext.User.GetUserId();
            var userName = _httpContext.HttpContext.User.GetUserName();

            foreach (var entity in _context.ChangeTracker
                .Entries()
                .Where(x => x.Entity is BaseModel && x.State == EntityState.Modified)
                .Select(x => x.Entity)
                .Cast<BaseModel>())
            {
                entity.UpdatedDate = DateTime.Now;
                entity.UpdatedUserId = userId;
            }

            foreach (var entity in _context.ChangeTracker
                .Entries()
                .Where(x => x.Entity is BaseModel && x.State == EntityState.Added)
                .Select(x => x.Entity)
                .Cast<BaseModel>())
            {
                entity.CreatedDate = DateTime.Now;
                entity.CreatedUserId = userId;
            }

            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Rozpoczęcie transakcji
        /// </summary>
        /// <returns>Kontekst transakcji</returns>
        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        /// <summary>
        /// Rozpoczęcie transakcji asynchronicznie
        /// </summary>
        /// <returns>Kontekst transakcji</returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
