using System.Web.Http;
using Microsoft.Practices.Unity;
using MovieTrailers.Interfaces;
using MovieTrailers.Services;
using MovieTrailers.DataAccess.Interfaces;
using MovieTrailers.DataAccess.Youtube;
using MovieTrailers.DataAccess.OMDB;
using MovieTrailers.DataAccess;

namespace MovieTrailers
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<IIdGenerator, IdGenerator>();
            container.RegisterType<IMovieDataAccess, YoutubeService>("YoutubeService");
            container.RegisterType<IMovieDataAccess, OMDBService>("OMDBService");
            container.RegisterInstance<IAppCache>(MovieTrailers.DataAccess.AppCache.Instance);
            container.RegisterInstance<IMovieTrailerService>(new MovieTrailerService(container.ResolveAll<IMovieDataAccess>(), container.Resolve<IAppCache>()));
            return container;
        }
    }
}