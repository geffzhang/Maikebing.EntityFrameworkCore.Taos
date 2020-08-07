// Copyright (c)  Maikebing. All rights reserved.
// Licensed under the MIT License, See License.txt in the project root for license information.

using JetBrains.Annotations;
using Maikebing.EntityFrameworkCore.Taos.Infrastructure.Internal;

namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    /// <summary>
    ///     <para>
    ///         Allows Taos specific configuration to be performed on <see cref="DbContextOptions" />.
    ///     </para>
    ///     <para>
    ///         Instances of this class are returned from a call to
    ///         <see
    ///             cref="TaosDbContextOptionsBuilderExtensions.UseTaos(DbContextOptionsBuilder, string, System.Action{TaosDbContextOptionsBuilder})" />
    ///         and it is not designed to be directly constructed in your application code.
    ///     </para>
    /// </summary>
    public class TaosDbContextOptionsBuilder : RelationalDbContextOptionsBuilder<TaosDbContextOptionsBuilder, TaosOptionsExtension>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TaosDbContextOptionsBuilder" /> class.
        /// </summary>
        /// <param name="optionsBuilder"> The options builder. </param>
        public TaosDbContextOptionsBuilder([NotNull] DbContextOptionsBuilder optionsBuilder)
            : base(optionsBuilder)
        {
        }
    }
}
