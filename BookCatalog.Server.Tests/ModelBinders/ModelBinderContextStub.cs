using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace BookCatalog.Server.Tests.ModelBinders
{
    internal class ModelBinderContextStub : ModelBinderProviderContext
    {
        private readonly Type modelType;

        public override BindingInfo BindingInfo { get; }

        public override ModelMetadata Metadata
        {
            get { return new ModelMetadataStub(ModelMetadataIdentity.ForType(modelType)); }
        }

        public override IModelMetadataProvider MetadataProvider => throw new NotImplementedException();

        public ModelBinderContextStub(Type modelType)
        {
            this.modelType = modelType;
        }

        public override IModelBinder CreateBinder(ModelMetadata metadata)
        {
            return null;
        }
    }
}
