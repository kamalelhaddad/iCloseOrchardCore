using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using OrchardCore.ResourceManagement;

namespace iCloseTheme {
    public class Startup : StartupBase {
        public override void ConfigureServices(IServiceCollection serviceCollection) {
            //serviceCollection.AddScoped<IResourceManifestProvider, ResourceManifest>();
        }
    }
}
