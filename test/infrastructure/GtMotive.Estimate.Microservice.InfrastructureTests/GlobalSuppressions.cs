// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "For avoid xUnit1027.", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure.TestServerCollectionFixture")]
[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Test classes need to be public for xUnit.")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test method names use underscores for readability.")]
[assembly: SuppressMessage("Documentation", "SA1615:Element return value should be documented", Justification = "Test methods don't need return value documentation.")]
[assembly: SuppressMessage("Reliability", "CA1062:Validate arguments of public methods", Justification = "Fixture arguments in tests are guaranteed by xUnit.")]
[assembly: SuppressMessage("Minor Code Smell", "S6603:The collection-specific method should be used", Justification = "FluentAssertions style preference.")]
[assembly: SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1210:Using directives should be ordered alphabetically", Justification = "Project using directives preference.")]
