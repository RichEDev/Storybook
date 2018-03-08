namespace ManagementAPI.Repositories
{
    using ManagementAPI.Interface;

    public abstract class BaseRepository<T> : IRepository<T> 
    {
        private IContext _context;

        public IContext Context {
            get
            {
                _context = _context ?? new Context();
                return _context;
            }
            set
            {
                _context = value;
            }
        }
    }
}