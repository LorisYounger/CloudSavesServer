using LinePutScript;
using LinePutScript.Converter;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace SavesServer
{
    public class LPSAspModel : IModelBinder, IModelBinderProvider
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            // 从请求体中读取原始LPS字符串
            using (var reader = new StreamReader(bindingContext.HttpContext.Request.Body, Encoding.UTF8))
            {
                var lpsstr = await reader.ReadToEndAsync();
                if (bindingContext.ModelType == typeof(ILPS))
                {
                    ILPS r = (ILPS)bindingContext.ModelType.CreateInstanceGetDefaultValue();
                    r.Load(lpsstr);
                    bindingContext.Result = ModelBindingResult.Success(r);
                    return;
                }
                if (string.IsNullOrEmpty(lpsstr))
                {
                    return;
                }
                Line l = [.. new LPS(lpsstr)];

                object? o = LPSConvert.GetSubObject(l, bindingContext.ModelType, convertNoneLineAttribute: true);
                bindingContext.Result = ModelBindingResult.Success(o);
            }
        }
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                return null;
            }
            if (context.Metadata.IsComplexType)
            {
                return this;
            }
            return null;
        }
    }
}
