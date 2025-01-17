using BookCatalog.Shared.Request.Sorting;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BookCatalog.Server.ModelBinders
{
    /// <summary>
    /// Model binder for all properties used in sort that starts from "sort." prefix
    /// </summary>
    public class SortModelBinder : IModelBinder
    {
        private readonly string sortPrefix = "sort.";
        private readonly IModelBinder _modelBinder;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="modelBinder">Default model binder</param>
        public SortModelBinder(IModelBinder modelBinder)
        {
            _modelBinder = modelBinder;
        }

        /// <inheritdoc/>
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext is null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            await _modelBinder.BindModelAsync(bindingContext);

            var model = bindingContext.Model as ISortableRequest;
            List<ClientSort> sortCollection = null;
            foreach (var param in bindingContext.HttpContext.Request.Query.Keys)
            {
                if (!param.StartsWith(sortPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var paramName = param.Split(".")[1].ToUpperInvariant();
                if (sortCollection is null)
                {
                    sortCollection = new List<ClientSort>();
                }

                foreach (var paramOrder in bindingContext.ValueProvider.GetValue(param))
                {
                    sortCollection.Add(new ClientSort(paramName, paramOrder));
                }
            }

            model.Sort = sortCollection?.ToArray();
            bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}
