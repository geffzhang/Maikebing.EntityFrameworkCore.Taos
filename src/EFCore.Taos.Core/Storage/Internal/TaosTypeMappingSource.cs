// Copyright (c)  Maikebing. All rights reserved.
// Licensed under the MIT License, See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Maikebing.EntityFrameworkCore.Taos.Storage.Internal
{
    /// <summary>
    ///     <para>
    ///         This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///         the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///         any release. You should only use it directly in your code with extreme caution and knowing that
    ///         doing so can result in application failures when updating to a new Entity Framework Core release.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Singleton" />. This means a single instance
    ///         is used by many <see cref="DbContext" /> instances. The implementation must be thread-safe.
    ///         This service cannot depend on services registered as <see cref="ServiceLifetime.Scoped" />.
    ///     </para>
    /// </summary>
    public class TaosTypeMappingSource : RelationalTypeMappingSource
    {
        private static readonly HashSet<string> _spatialiteTypes
            = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "GEOMETRY",
                "GEOMETRYCOLLECTION",
                "LINESTRING",
                "MULTILINESTRING",
                "MULTIPOINT",
                "MULTIPOLYGON",
                "POINT",
                "POLYGON"
            };

        private const string IntegerTypeName = "INT";
        private const string BIGINTTypeName = "BIGINT";
        
        private const string DOUBLETypeName = "DOUBLE";
        private const string BINARYTypeName = "BINARY";
        private const string TextTypeName = "NCHAR";
        private const string SMALLINTTypeName = "SMALLINT";
        
        private const string TIMESTAMPTypeName = "TIMESTAMP";
        private const string FLOATTypeName = "FLOAT";
        private const string TINYINTTypeName = "TINYINT";
        private const string BOOLTypeName = "BOOL";

        private static readonly LongTypeMapping _integer = new LongTypeMapping(BIGINTTypeName);
        private static readonly DoubleTypeMapping _real = new DoubleTypeMapping(DOUBLETypeName);
        private static readonly ByteArrayTypeMapping _blob = new ByteArrayTypeMapping(BINARYTypeName);
        private static readonly StringTypeMapping _text = new StringTypeMapping(TextTypeName);

        private readonly Dictionary<Type, RelationalTypeMapping> _clrTypeMappings
            = new Dictionary<Type, RelationalTypeMapping>
            {
                { typeof(string), _text },
                { typeof(byte[]), _blob },
                { typeof(bool), new BoolTypeMapping(BOOLTypeName) },
                { typeof(byte), new ByteTypeMapping(TINYINTTypeName) },
                { typeof(int), new IntTypeMapping(IntegerTypeName) },
                { typeof(long), _integer },
                { typeof(sbyte), new SByteTypeMapping(IntegerTypeName) },
                { typeof(short), new ShortTypeMapping(SMALLINTTypeName) },
                { typeof(uint), new UIntTypeMapping(IntegerTypeName) },
                { typeof(ulong), new TaosULongTypeMapping(BIGINTTypeName) },
                { typeof(ushort), new UShortTypeMapping(SMALLINTTypeName) },
                { typeof(DateTime), new TaosDateTimeTypeMapping(TIMESTAMPTypeName) },
                { typeof(DateTimeOffset), new TaosDateTimeOffsetTypeMapping(TIMESTAMPTypeName) },
                { typeof(TimeSpan), new TimeSpanTypeMapping(TIMESTAMPTypeName) },
                { typeof(decimal), new TaosDecimalTypeMapping(BIGINTTypeName) },
                { typeof(double), _real },
                { typeof(float), new FloatTypeMapping(FLOATTypeName) },
                { typeof(Guid), new TaosGuidTypeMapping(BINARYTypeName) }
            };

        private readonly Dictionary<string, RelationalTypeMapping> _storeTypeMappings
            = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
            {
                { IntegerTypeName, _integer },
                { DOUBLETypeName, _real },
                { BINARYTypeName, _blob },
                { TextTypeName, _text }
            };

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public TaosTypeMappingSource(
            [NotNull] TypeMappingSourceDependencies dependencies,
            [NotNull] RelationalTypeMappingSourceDependencies relationalDependencies)
            : base(dependencies, relationalDependencies)
        {
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static bool IsSpatialiteType(string columnType)
            => _spatialiteTypes.Contains(columnType);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            if (clrType != null
                && _clrTypeMappings.TryGetValue(clrType, out var mapping))
            {
                return mapping;
            }

            var storeTypeName = mappingInfo.StoreTypeName;
            if (storeTypeName != null
                && _storeTypeMappings.TryGetValue(storeTypeName, out mapping))
            {
                return mapping;
            }

            mapping = base.FindMapping(mappingInfo);

            if (mapping == null
                && storeTypeName != null)
            {
                var affinityTypeMapping = _typeRules.Select(r => r(storeTypeName)).FirstOrDefault(r => r != null);

                if (affinityTypeMapping == null)
                {
                    return _blob;
                }

                if (clrType == null
                    || affinityTypeMapping.ClrType.UnwrapNullableType() == clrType)
                {
                    return affinityTypeMapping;
                }
            }

            return mapping;
        }
        /// <summary>
        /// TODO:���������Ҫ�޸�
        /// </summary>
        private readonly Func<string, RelationalTypeMapping>[] _typeRules =
        {
            name => Contains(name, "INT")
                ? _integer
                : null,
            name => Contains(name, "CHAR")
                || Contains(name, "CLOB")
                || Contains(name, "TEXT")
                    ? _text
                    : null,
            name => Contains(name, "BLOB")
                || Contains(name, "BIN")
                    ? _blob
                    : null,
            name => Contains(name, "REAL")
                || Contains(name, "FLOA")
                || Contains(name, "DOUB")
                    ? _real
                    : null
        };

        private static bool Contains(string haystack, string needle)
            => haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
