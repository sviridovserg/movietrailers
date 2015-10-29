using System.Web.Http;
using Microsoft.Practices.Unity;
using MovieTrailers.Interfaces;
using MovieTrailers.Services;

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

            container.RegisterType<IMovieTrailerService, YoutubeService>();     

            return container;
        }
    }
}