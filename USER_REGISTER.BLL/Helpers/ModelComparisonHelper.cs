using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace USER_REGISTER.BLL.Helpers
{
    public static class ModelComparisonHelper
    {
        /// <summary>
        /// Compare a collection with another.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="GroupName"></param>
        /// <param name="previousModels"></param>
        /// <param name="updatedModels"></param>
        /// <param name="GetUpdateModelFtn"></param>
        /// <param name="CompareChildItemsFtn"></param>
        /// <returns></returns>
        public static ModelComparisonResultGroup CompareCollections<T>(this IEnumerable<T> previousModels, IEnumerable<T> updatedModels, string GroupName
            , Func<T, object> GetRecordIdFtn, Func<T, T, IEnumerable<ModelComparisonResultGroup>> CompareChildItemsFtn = null) where T : class
        {
            ModelComparisonResultGroup resultGroup = new ModelComparisonResultGroup
            {
                GroupName = GroupName
            };

            var previousModelList = previousModels?.ToList() ?? new List<T>();
            var updatedModelList = updatedModels?.ToList() ?? new List<T>();

            foreach (var item in previousModelList)//check existing
            {
                var updatedItem = updatedModelList.Where(r => GetRecordIdFtn(r).Equals(GetRecordIdFtn(item))).SingleOrDefault();

                if (updatedItem != null)//modified
                {
                    resultGroup.ComparisonResults.Add(item.CompareWith(updatedItem, GetRecordIdFtn, CompareChildItemsFtn));
                    updatedModelList.Remove(updatedItem);
                }
                else//deleted
                {
                    resultGroup.ComparisonResults.Add(CompareWith(item, null, GetRecordIdFtn, CompareChildItemsFtn));
                }
            }

            //add new
            foreach (var item in updatedModelList)
            {
                resultGroup.ComparisonResults.Add(CompareWith(null, item, GetRecordIdFtn, CompareChildItemsFtn));
            }

            //remove empty results for no change
            resultGroup.ComparisonResults.RemoveAll(r => r == null);

            return resultGroup;
        }

        private static bool IsComparableType(this PropertyInfo property)
        {
            //ignore NotMapped fields
            if (property.GetCustomAttributes<NotMappedAttribute>(false).Any()) return false;

            Type[] acceptedTypes = new[] { typeof(Decimal), typeof(int), typeof(string), typeof(DateTime), typeof(bool), typeof(double)
            ,typeof(float),typeof(Guid)};

            Type actualPropertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            return actualPropertyType.IsEnum || acceptedTypes.Contains(actualPropertyType);
        }

        /// <summary>
        /// Looks for [DisplayName] or [Display] attributes to get a name, else it returns the PropertyName
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetDisplayNameFromAttributes(this PropertyInfo property)
        {
            return property.GetCustomAttribute<DisplayNameAttribute>(false)?.DisplayName
                ?? property.GetCustomAttribute<DisplayAttribute>(false)?.Name
                ?? property.Name;
        }

        /// <summary>
        /// Compare a model with another. At least one of the models (previousModel or updatedModel) MUST be specified.
        /// </summary>
        /// <param name="previousModel"></param>
        /// <param name="updatedModel"></param>
        /// <param name="CompareChildItemsFtn">A function to compare subitems of the specified models</param>
        /// <returns></returns>
        public static ModelComparisonResult CompareWith<T>(this T previousModel, T updatedModel
            , Func<T, object> GetRecordIdFtn
            , Func<T, T, IEnumerable<ModelComparisonResultGroup>> CompareChildItemsFtn = null) where T : class
        {
            if (previousModel == null && updatedModel == null)
            {
                throw new Exception("One of the Models for Comparison MUST be specified!");
            }

            var result = new ModelComparisonResult();

            Func<Type, IEnumerable<PropertyInfo>> GetTypeProperties = type => type.GetProperties().Where(r => r.GetCustomAttribute<JsonIgnoreAttribute>() == null);

            if (updatedModel == null)//deleted
            {
                result.ResultType = ModelComparisonResultType.Deleted;
                result.RecordId = GetRecordIdFtn(previousModel);

                foreach (var property in GetTypeProperties(previousModel.GetType()))
                {
                    if (property.IsComparableType())
                    {
                        result.FieldInfos.Add(new ModelComparisonFieldInfo
                        {
                            PropertyName = property.GetDisplayNameFromAttributes(),
                            ValueBefore = property.GetValue(previousModel, null)
                        });
                    }
                }
            }
            else if (previousModel == null)//added
            {
                result.ResultType = ModelComparisonResultType.Added;
                result.RecordId = GetRecordIdFtn(updatedModel);

                foreach (var property in GetTypeProperties(updatedModel.GetType()))
                {
                    if (property.IsComparableType())
                    {
                        result.FieldInfos.Add(new ModelComparisonFieldInfo
                        {
                            PropertyName = property.GetDisplayNameFromAttributes(),
                            ValueAfter = property.GetValue(updatedModel, null)
                        });
                    }
                }
            }
            else//possibly modified
            {
                result.ResultType = ModelComparisonResultType.Modified;
                result.RecordId = GetRecordIdFtn(previousModel);

                List<ModelComparisonFieldInfo> UpdateFieldInfos = new List<ModelComparisonFieldInfo>();

                foreach (var property in GetTypeProperties(previousModel.GetType()))
                {
                    if (property.IsComparableType())
                    {
                        var fieldInfo = new ModelComparisonFieldInfo
                        {
                            PropertyName = property.GetDisplayNameFromAttributes(),
                            ValueBefore = property.GetValue(previousModel, null),
                            ValueAfter = property.GetValue(updatedModel, null)
                        };

                        if (fieldInfo.ValueBefore == null && fieldInfo.ValueAfter != null
                            || fieldInfo.ValueBefore != null && fieldInfo.ValueAfter == null
                            || (fieldInfo.ValueBefore != null && fieldInfo.ValueAfter != null
                            && !fieldInfo.ValueBefore.Equals(fieldInfo.ValueAfter)))//track only modified fields
                        {
                            UpdateFieldInfos.Add(fieldInfo);
                        }
                    }
                }

                if (UpdateFieldInfos.Count == 0)//No Change Found
                {
                    result.ResultType = ModelComparisonResultType.No_Change;
                }
                else
                {
                    result.FieldInfos = UpdateFieldInfos.ToList();
                }
            }

            if (CompareChildItemsFtn != null)
            {
                result.ComparisonResultGroups = CompareChildItemsFtn(previousModel, updatedModel).ToList();

                //remove empty Groups
                result.ComparisonResultGroups.RemoveAll(r => r.ComparisonResults.Count == 0);
            }

            if (result.ResultType == ModelComparisonResultType.No_Change && !result.ComparisonResultGroups.Any())
            {
                return null;
            }

            return result;
        }
    }

    public class ModelComparisonResult
    {
        public object RecordId { get; set; }
        public ModelComparisonResultType ResultType { get; set; }
        public List<ModelComparisonFieldInfo> FieldInfos { get; set; }
        public List<ModelComparisonResultGroup> ComparisonResultGroups { get; set; }

        public ModelComparisonResult()
        {
            FieldInfos = new List<ModelComparisonFieldInfo>();
            ComparisonResultGroups = new List<ModelComparisonResultGroup>();
        }
    }

    public class ModelComparisonResultGroup
    {
        public string GroupName { get; set; }
        public List<ModelComparisonResult> ComparisonResults { get; set; }

        public ModelComparisonResultGroup()
        {
            ComparisonResults = new List<ModelComparisonResult>();
        }

        public ModelComparisonResult PackIntoResult(object RecordId, ModelComparisonResultType ResultType = ModelComparisonResultType.No_Change)
        {
            return new ModelComparisonResult
            {
                RecordId = RecordId,
                ResultType = ResultType,
                ComparisonResultGroups = new List<ModelComparisonResultGroup> { this }
            };
        }
    }

    public enum ModelComparisonResultType
    {
        Added,
        Deleted,
        Modified,
        No_Change
    }

    public class ModelComparisonFieldInfo
    {
        public string PropertyName { get; set; }
        public object ValueBefore { get; set; }
        public object ValueAfter { get; set; }
    }
}
