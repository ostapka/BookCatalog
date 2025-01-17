using BookCatalog.Shared.Request.Sorting;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BookCatalog.Server.ModelBinders
{
    /// <summary>
    /// ModelBinder provider for SortModelBinder
    /// </summary>
    public class SortModelBinderProvider : IModelBinderProvider
    {
        private IList<IModelBinderProvider> ModelBinderProviders { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="modelBinderProviders">Providers from application</param>
        public SortModelBinderProvider(IList<IModelBinderProvider> modelBinderProviders)
        {
            ModelBinderProviders = modelBinderProviders;
        }

        /// <inheritdoc/>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (typeof(ISortableRequest).IsAssignableFrom(context.Metadata.ModelType))
            {
                IModelBinder defaultBinder = ModelBinderProviders
                   .Where(x => x.GetType() != this.GetType())
                   .Select(x => x.GetBinder(context)).FirstOrDefault(x => x != null);

                if (defaultBinder != null)
                {
                    return new SortModelBinder(defaultBinder);
                }
            }

            return null;
        }
    }
}
