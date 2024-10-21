using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Spottarr.Web.Helpers;

internal sealed class CommaSeparatedEnumBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var key = bindingContext.FieldName;
        var value = ValueProviderResult.None;
        
        try
        {
            value = bindingContext.ValueProvider.GetValue(key);
        }
        catch (KeyNotFoundException)
        {
            return Task.CompletedTask;
        }
        
        var elementType = bindingContext.ModelType.GetElementType();
        var metadata = bindingContext.ModelMetadata;
        
        if (value == ValueProviderResult.None
            || string.IsNullOrEmpty(value.FirstValue)
            || elementType == null
            || !metadata.IsEnumerableType
            || !elementType.IsEnum)
        {
            return Task.CompletedTask;
        }
        
        var stringValues = value
            .SelectMany(v => v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .ToArray();

        var enumValues = Array.CreateInstance(elementType, stringValues.Length);

        for (var i = 0; i < stringValues.Length; i++)
        {
            var converter = TypeDescriptor.GetConverter(elementType);
            enumValues.SetValue(converter.ConvertFromString(stringValues[i]), i);
        }

        bindingContext.Result = ModelBindingResult.Success(enumValues);
        
        return Task.CompletedTask;
    }
}