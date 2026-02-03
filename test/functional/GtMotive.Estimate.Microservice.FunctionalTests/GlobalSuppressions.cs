// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "For avoid xUnit1027.", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure.CompositionRootCollectionFixture")]
[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Test classes need to be public for xUnit.")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test method names use underscores for readability.")]
[assembly: SuppressMessage("Documentation", "SA1615:Element return value should be documented", Justification = "Test methods don't need return value documentation.")]
[assembly: SuppressMessage("Reliability", "CA1062:Validate arguments of public methods", Justification = "Fixture arguments in tests are guaranteed by xUnit.")]
[assembly: SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1210:Using directives should be ordered alphabetically", Justification = "Project using directives preference.")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1402:File may only contain a single type", Justification = "Test output ports are grouped together for maintainability.")]
[assembly: SuppressMessage("Design", "CA1822:Mark members as static", Justification = "Test output port methods need to be instance methods for interface implementation.")]
[assembly: SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameters required by interface contract even if not used in test implementation.")]
[assembly: SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Extension class logically appears after the class it extends.")]
[assembly: SuppressMessage("Performance", "CA1852:Seal internal types", Justification = "Test output ports are simple test helpers that don't need to be sealed.")]
