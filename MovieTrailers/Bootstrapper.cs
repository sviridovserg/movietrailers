using System.Web.Http;
using Microsoft.Practices.Unity;
using MovieTrailers.Interfaces;
using MovieTrailers.Services;
using MovieTrailers.DataAccess.Interfaces;
using MovieTrailers.DataAccess.Youtube;
using MovieTrailers.DataAccess.OMDB;

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

            container.RegisterType<IMovieDataAccess, YoutubeService>("YoutubeService");
            container.RegisterType<IMovieDataAccess, OMDBService>("OMDBService");     
            container.RegisterInstance<IMovieTrailerService>(new MovieTrailerService(container.ResolveAll<IMovieDataAccess>()));
            return container;
        }
    }
}