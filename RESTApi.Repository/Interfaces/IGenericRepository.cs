using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RESTApi.Models.DTOs;
using System.Linq.Expressions;

namespace RESTApi.Repository.Interfaces
{
    public interface IGenericRepository<T, in TKey> where T : class
    {
        /// <summary>
        /// Wyszukiwanie w kolekcji obiektów
        /// </summary>
        /// <param name="expression">Wyrażenie wyszukiwania</param>
        /// <returns>Wyszukana kolekcja obiektów</returns>
        IQueryable<T> Find(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Wyszukiwanie w kolekcji obiektów
        /// </summary>
        /// <param name="context">Kontekst bazy danych</param>
        /// <param name="expression">Wyrażenie wyszukiwania</param>
        /// <returns>Wyszukana kolekcja obiektów</returns>
        IQueryable<T> Find(DbContext context, Expression<Func<T, bool>> expression);

        /// <summary>
        /// Pobranie kolekcji obiektów
        /// </summary>
        /// <returns>Kolekcja obiektów</returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// Wyszukiwanie
        /// </summary>
        /// <param name="queryParams">Parametry wyszukiwania</param>
        /// <param name="filteredCount">Liczba wyszukanych rekordów</param>
        /// <param name="totalCount">Liczba wszystkich rekordów</param>
        /// <returns>Lista wyszukanych obiektów</returns>
        IQueryable<T> Search(ListRequestDTO model, out int filteredCount, out int totalCount);

        /// <summary>
        /// Dodanie rekordu
        /// </summary>
        /// <param name="entity">Obiekt do dodania</param>
        /// <returns>Dodany obiekt</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Dodanie rekordu
        /// </summary>
        /// <param name="context">Kontekst bazy danych</param>
        /// <param name="entity">Obiekt do dodania</param>
        /// <returns>Dodany obiekt</returns>
        Task<T> AddAsync(DbContext context, T entity);

        /// <summary>
        /// Dodanie wielu rekordów
        /// </summary>
        /// <param name="entities">Obiekty do dodania</param>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Pobranie rekordu po identyfikatorze
        /// </summary>
        /// <param name="id">Identyfikator</param>
        /// <returns>Znaleziony obiekt</returns>
        T GetById(TKey id);
        /// <summary>
        /// Pobranie rekordu po identyfikatorze - asynchronicznie
        /// </summary>
        /// <param name="id">Identyfikator</param>
        /// <returns>Znaleziony obiekt</returns>

        Task<T> GetByIdAsync(TKey id);
        /// <summary>
        /// Usunięcie rekordu
        /// </summary>
        /// <param name="entity">Obiekt do usunięcia</param>
        Task RemoveAsync(T entity);

        /// <summary>
        /// Usunięcie wielu rekordów
        /// </summary>
        /// <param name="entities">Obiekty do usunięcia</param>
        Task RemoveRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Aktualizacja rekordu
        /// </summary>
        /// <param name="entity">Obiekt aktualizowany</param>
        /// <returns>Zaktualizowany obiekt</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Aktualizacja rekordu
        /// </summary>
        /// <param name="context">Kontekst bazy danych</param>
        /// <param name="entity">Obiekt aktualizowany</param>
        /// <returns>Zaktualizowany obiekt</returns>
        Task<T> UpdateAsync(DbContext context, T entity);

        Task<int> SaveChangesAsync();

        /// <summary>
        /// Rozpoczęcie transakcji
        /// </summary>
        /// <returns>Kontekst transakcji</returns>
        IDbContextTransaction BeginTransaction();

        /// <summary>
        /// Rozpoczęcie transakcji asynchronicznie
        /// </summary>
        /// <returns>Kontekst transakcji</returns>
        Task<IDbContextTransaction> BeginTransactionAsync();

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression);

        Task<T> FirstOrDefaultAsyncOrdered(Expression<Func<T, bool>> expression, string orderColumn, bool orderDirection);

        T FirstOrDefault(Expression<Func<T, bool>> expression);
    }
}