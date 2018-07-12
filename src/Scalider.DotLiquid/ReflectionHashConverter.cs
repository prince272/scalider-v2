using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DotLiquid;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

namespace Scalider.DotLiquid
{

    /// <summary>
    /// Provides an implementation of the <see cref="IHashConverter"/> interface that uses reflection
    /// to convert models.
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class ReflectionHashConverter : IHashConverter, IDisposable
    {

        private readonly ILogger<ReflectionHashConverter> _logger;
        private readonly ConcurrentDictionary<Type, MemberInfo[]> _modelMembersCache =
            new ConcurrentDictionary<Type, MemberInfo[]>();
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionHashConverter"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public ReflectionHashConverter(ILogger<ReflectionHashConverter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">A value indicating whether the dispose method was called.</param>
        [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _modelMembersCache.Clear();
            }

            _disposed = true;
        }

        #region ModelToDictionary

        private IDictionary<string, object> ModelToDictionary(object model)
        {
            if (_disposed)
            {
                // The object has been disposed
                return null;
            }

            // Try to retrieve the members for the model type
            var modelType = model.GetType();
            if (!_modelMembersCache.TryGetValue(modelType, out var modelMembers))
            {
                // The model type hasn't been cached before
                _logger.LogDebug("Trying to retrieve all the public fields and properties for {@ModelType}",
                    TypeNameHelper.GetTypeDisplayName(modelType));

                var modelFields = modelType
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                var modelProperties = modelType
                                      .GetProperties(BindingFlags.Instance | BindingFlags.Public |
                                                     BindingFlags.FlattenHierarchy)
                                      .Where(t => t.CanRead);

                modelMembers = modelFields.Concat(modelProperties.Cast<MemberInfo>()).ToArray();
                _modelMembersCache.TryAdd(modelType, modelMembers);
            }

            // Populate a dictionary using the properties
            var result = new Dictionary<string, object>();
            foreach (var mInfo in modelMembers)
            {
                _logger.LogDebug(
                    "Trying to resolve value for {@Member}",
                    $"{TypeNameHelper.GetTypeDisplayName(mInfo.DeclaringType, true, true)}.{mInfo.Name} " +
                    $"({mInfo.DeclaringType?.Assembly.GetName().Name})"
                );

                if (result.ContainsKey(mInfo.Name))
                {
                    // We already added a member with the same name to the result, this one will be skipped
                    _logger.LogDebug("Another member with the same name ({@MemberName}) has already been added",
                        mInfo.Name);

                    continue;
                }

                // Try to resolve the value for the field or property
                object memberValue = null;
                switch (mInfo)
                {
                    case FieldInfo fInfo:
                        memberValue = fInfo.GetValue(model);
                        break;
                    case PropertyInfo pInfo:
                        try
                        {
                            memberValue = pInfo.GetValue(model);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(
                                e,
                                "There was an unexpected exception while trying to retrieve the value for " +
                                "the property {@PropertyName} of type {@PropertyType}",
                                $"{TypeNameHelper.GetTypeDisplayName(pInfo.DeclaringType, true, true)}.{pInfo.Name} " +
                                $"({pInfo.DeclaringType?.Assembly.GetName().Name})",
                                TypeNameHelper.GetTypeDisplayName(pInfo.PropertyType)
                            );

                            // Add the default value for the property, that way it is still populated
                            result.Add(
                                pInfo.Name,
                                pInfo.PropertyType.IsValueType
                                    ? Activator.CreateInstance(pInfo.PropertyType)
                                    : null
                            );

                            continue;
                        }

                        break;
                }

                // Determine if we need to do any additional handling for the member value
                var valueType = memberValue?.GetType();
                if (memberValue == null || valueType.IsValueType)
                {
                    // We don't need to do any additional handling for the member value
                    result.Add(mInfo.Name, memberValue);
                    continue;
                }

                // Add the actual model value to the result
                result.Add(
                    mInfo.Name,
                    valueType != typeof(object)
                        ? ModelToDictionary(memberValue)
                        : memberValue
                );
            }

            // Done
            return result;
        }

        #endregion

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IDropConverter Members

        /// <inheritdoc />
        public Hash ToHash(object model)
        {
            Check.NotNull(model, nameof(model));

            return Hash.FromDictionary(ModelToDictionary(model));
        }

        #endregion

    }

}