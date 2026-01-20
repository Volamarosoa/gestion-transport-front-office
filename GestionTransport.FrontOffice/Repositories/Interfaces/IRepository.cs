namespace GestionTransport.FrontOffice.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();
        T GetById(int id);
        int Create(T entity);
        void Update(T entity);
        void Delete(int id);
        void Activate(int id);
        void Deactivate(int id);
    }
}