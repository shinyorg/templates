using NJsonSchema.Generation;

namespace ShinyAspNet;

public class GuidToUuidSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        if (context.Schema.Format == "guid")
            context.Schema.Format = "uuid";
    }
} 