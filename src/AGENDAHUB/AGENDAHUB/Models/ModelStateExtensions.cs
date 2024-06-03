using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AGENDAHUB.Extensions
{
    public static class ModelStateExtensions
    {
        public static bool IsValidExcept(this ModelStateDictionary modelState, object model, params string[] propertyNamesToExclude)
        {
            foreach (var propertyName in propertyNamesToExclude)
            {
                modelState.Remove(propertyName);
            }

            return modelState.IsValid;
        }
    }
}
