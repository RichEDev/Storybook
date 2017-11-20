namespace SpendManagementApi.Controllers
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Interfaces;
    using Models.Common;
    using Models.Types;
    using Utilities;
    using Spend_Management;

    /// <summary>
    /// Handles the selection and creation of Controllers for testing.
    /// </summary>
    /// <typeparam name="T">The type, which is a subclass of BaseExternalType</typeparam>
    public class ControllerFactory<T> where T : BaseExternalType
    {
        private IRepository<T> _repository;
        private ICurrentUser _user;

        private BaseApiController _controller;

        /// <summary>
        /// Creates a ControllerFactory that creates controllers using the specified repository.
        /// </summary>
        /// <param name="repository">The repository to pass to the controller</param>
        public ControllerFactory(IRepository<T> repository)
        {
            _repository = repository;
            _user = _repository.User;
        }

        /// <summary>
        /// Creates a ControllerFactory that creates controllers using the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        public ControllerFactory(ICurrentUser user)
        {
            _user = user;
        }

        /// <summary>
        /// Getter for the current controller.
        /// </summary>
        public BaseApiController Controller
        {
            get
            {
                return _controller;
            }
        }

        /// <summary>
        /// Gets a controller 
        /// </summary>
        /// <returns></returns>
        public BaseApiController<T> GetController()
        {
            var list = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(c => c.IsSubclassOf(typeof(BaseApiController<T>)))
                    .ToList();

            var controller = (BaseApiController<T>)Activator.CreateInstance(list.First(), _repository);
            _controller = controller;
            return controller;
        }

        /// <summary>
        /// Gets a controller 
        /// </summary>
        /// <returns></returns>
        public BaseApiController<T> GetInitialisedController()
        {
            var list = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(c => c.IsSubclassOf(typeof(BaseApiController<T>)))
                    .ToList();

            var controller = (BaseApiController<T>)Activator.CreateInstance(list.First());
            _controller = controller;
            controller.Initialise(_user, new ApiAuditLogCallResult
            {
                Allowed = true,
                FriendlySummary = "Test"
            });

            return controller;
        }

        /// <summary>
        /// Calls put (add) on the selected controller.
        /// </summary>
        /// <param name="request">The type of the object</param>
        /// <param name="useInited">Determines whether to call the Init() on the controller, 
        /// overwriting it's ActionContext.</param>
        /// <typeparam name="TResponse">The type of the response</typeparam>
        /// <returns>An instance of type TResponse</returns>
        public TResponse Put<TResponse>(T request, bool useInited = false) where TResponse : ApiResponse<T>, new()
        {
            using (var controller = useInited ? GetInitialisedController() : GetController())
            {
                return controller.Put<TResponse>(request);
            }
        }

        /// <summary>
        /// Calls post (edit) on the selected controller.
        /// </summary>
        /// <param name="request">The type of the object</param>
        /// <param name="useInited">Determines whether to call the Init() on the controller, 
        /// overwriting it's ActionContext.</param>
        /// <typeparam name="TResponse">The type of the response</typeparam>
        /// <returns>An instance of type TResponse</returns>
        public TResponse Post<TResponse>(T request, bool useInited = false) where TResponse : ApiResponse<T>, new()
        {
            using (var controller = useInited ? GetInitialisedController() : GetController())
            {
                return controller.Post<TResponse>(request);
            }
        }

        /// <summary>
        /// Calls Get on the selected controller.
        /// </summary>
        /// <param name="id">The id of the object to get</param>
        /// <param name="useInited">Determines whether to call the Init() on the controller, 
        /// overwriting it's ActionContext.</param>
        /// <typeparam name="TResponse">The type of the response</typeparam>
        /// <returns>An instance of type TResponse</returns>
        public TResponse Get<TResponse>(int id, bool useInited = false) where TResponse : ApiResponse<T>, new()
        {
            using (var controller = useInited ? GetInitialisedController() : GetController())
            {
                return controller.Get<TResponse>(id);
            }
        }

        /// <summary>
        /// Calls GetAll on the selected controller.
        /// </summary>
        /// <param name="useInited">Determines whether to call the Init() on the controller, 
        /// overwriting it's ActionContext.</param>
        /// <typeparam name="TResponse">The type of the response</typeparam>
        /// <returns>An instance of type TResponse</returns>
        public TResponse GetAll<TResponse>(bool useInited = false) where TResponse : GetApiResponse<T>, new()
        {
            using (var controller = useInited ? GetInitialisedController() : GetController())
            {
                return controller.GetAll<TResponse>();
            }
        }

        /// <summary>
        /// Calls Delete on the selected controller.
        /// </summary>
        /// <param name="id">The id of the object to delete</param>
        /// <param name="useInited">Determines whether to call the Init() on the controller, 
        /// overwriting it's ActionContext.</param>
        /// <typeparam name="TResponse">The type of the response</typeparam>
        /// <returns>An instance of type TResponse</returns>
        public TResponse Delete<TResponse>(int id, bool useInited = false) where TResponse : ApiResponse<T>, new()
        {
            using (var controller = useInited ? GetInitialisedController() : GetController())
            {
                return controller.Delete<TResponse>(id);
            }
        }
    }
}