using System;
using System.Linq.Expressions;

namespace Parsimony.Options
{
    /// <summary>
    /// An interface for building rules for an option set with respect to a specified target option within the set.
    /// </summary>
    /// <remarks>
    /// A rule builder <c>builder</c> for the option "foo" in a set would use
    /// <c>builder.Requires("bar").Precludes("baz")</c> to indicate that "bar" must be supplied when "foo" is supplied
    /// and that "baz" may not be supplied when "foo" is supplied.
    /// </remarks>
    public interface IRuleBuilder<TOptionSet> where TOptionSet : class
    {
        /// <summary>
        /// Requires the option associated with <paramref name="property"/> when the target option is supplied.
        /// </summary>
        /// <typeparam name="TProperty">The type of the required option.</typeparam>
        /// <param name="property">The property associated with the required option.</param>
        /// <returns>The <see cref="IRuleBuilder{TOptions}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="property"/> is not a writable property of <typeparamref name="TOptionSet"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="property"/> refers to the target option's property.
        /// </exception>
        public IRuleBuilder<TOptionSet> Requires<TProperty>(Expression<Func<TOptionSet, TProperty>> property);


        /// <summary>
        /// Precludes the option associated with <paramref name="property"/> when the target option is supplied.
        /// </summary>
        /// <typeparam name="TProperty">The type of the precluded option.</typeparam>
        /// <param name="property">The property associated with the precluded option.</param>
        /// <returns>The <see cref="IRuleBuilder{TOptions}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="property"/> is not a writable property of <typeparamref name="TOptionSet"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="property"/> refers to the target option's property.
        /// </exception>
        /// <remarks>
        /// Note that if some option A precludes some option B then the option B effectively precludes A as well.
        /// </remarks>
        public IRuleBuilder<TOptionSet> Precludes<TProperty>(Expression<Func<TOptionSet, TProperty>> property);
    }
}
