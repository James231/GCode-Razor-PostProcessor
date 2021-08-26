using RazorLight;

namespace GCodeRazor.EngineBuilder
{
    public static class Builder
    {
        public static RazorLightEngine GetBuilder()
        {
            var engine = new RazorLightEngineBuilder()
                // required to have a default RazorLightProject type,
                // but not required to create a template from string.
                .UseEmbeddedResourcesProject(typeof(EmptyModel))
                .SetOperatingAssembly(typeof(EmptyModel).Assembly)
                .UseMemoryCachingProvider()
                .Build();

            return engine;
        }
    }
}
