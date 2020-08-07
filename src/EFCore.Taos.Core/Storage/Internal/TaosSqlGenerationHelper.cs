// Copyright (c)  Maikebing. All rights reserved.
// Licensed under the MIT License, See License.txt in the project root for license information.

using System.Text;
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
    public class TaosSqlGenerationHelper : RelationalSqlGenerationHelper
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public TaosSqlGenerationHelper([NotNull] RelationalSqlGenerationHelperDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override string DelimitIdentifier(string name, string schema)
            => base.DelimitIdentifier(name,schema);
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public  override     void DelimitIdentifier(StringBuilder builder, string name, string schema)
        {
            if (!string.IsNullOrEmpty(schema))
            {
                DelimitIdentifier(builder, schema);
                builder.Append(".");
            }
            DelimitIdentifier(builder, name);
        }
        public  override  void DelimitIdentifier(StringBuilder builder, string identifier)
        {
            Microsoft.EntityFrameworkCore.Utilities.Check.NotEmpty(identifier, "identifier");
            EscapeIdentifier(builder, identifier);
        }
        public override  void EscapeIdentifier( StringBuilder builder, string identifier)
        {
            Microsoft.EntityFrameworkCore.Utilities.Check.NotEmpty(identifier, "identifier");
            int length = builder.Length;
            builder.Append(identifier);
        }

        public override string DelimitIdentifier(string identifier) => EscapeIdentifier(Microsoft.EntityFrameworkCore.Utilities.Check.NotEmpty(identifier, "identifier"));
    }
}
