using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UserService.Testing.ArchitectureTests;
public class PersistenceArchitectureTests : BaseArchitectureTest
{
  private Assembly[] LoadAssembliesWithPattern(string assemblyPattern)
  {
    var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
    var matchingAssemblies = loadedAssemblies
        .Where(assembly => Regex.IsMatch(assembly.FullName, assemblyPattern))
        .ToArray();

    return matchingAssemblies;
  }

  [Fact]
  public void PersistenceAssemblyExistence()
  {
    Assert.NotNull(PersistenceAssembly);
  }

  [Fact]
  public void PersistenceAssemblyDependencies()
  {
    var persistenceTypes = LoadAssembliesWithPattern(@".*\.Persistence\..*")
        .SelectMany(assembly => assembly.GetTypes());

    foreach (var type in persistenceTypes)
    {
      var dependencies = type.GetTypeInfo().ImplementedInterfaces;
      Assert.True(dependencies.Any(dep => dep.FullName.EndsWith("Domain.dll") || dep.FullName.EndsWith("Contracts.dll")));
    }
  }

  [Fact]
  public void PersistenceLayerDependencies_AreCorrect()
  {
    ReportArchitectureContext();
    var repositories = PersistenceLayer.GetObjects(BaseArchitecture)
        .Where(t => t.Name.EndsWith("Repository"))
        .ToList();

    if (!repositories.Any())
    {
      Console.WriteLine("ðŸŸ¡ No CommandHandlers found in Application layer â€” skipping rule.");
      return;
    }

    ArchRuleDefinition
        .Classes()
        .That()
        .ResideInAssembly(PersistenceAssembly)
        .Should()
        .OnlyDependOnTypesThat()
        // âœ… Allow dependencies on UserService core domain abstractions
        .ResideInNamespaceMatching("Franz.Common.Business.Domain")
        .OrShould().ResideInNamespaceMatching("Franz.Common.Business.Events")

        // âœ… Allow dependencies on UserService persistence and repository tooling
        .OrShould().ResideInNamespaceMatching("Franz.Common.EntityFramework")
        .OrShould().ResideInNamespaceMatching("Franz.Common.EntityFramework.Repositories")
        .OrShould().ResideInNamespaceMatching("Franz.Common.EntityFramework.Extensions")
        .OrShould().ResideInNamespaceMatching("Franz.Common.EntityFramework.Configuration")
        .OrShould().ResideInNamespaceMatching("Franz.Common.EntityFramework.Behaviors")
        .OrShould().ResideInNamespaceMatching("Franz.Common.EntityFramework.Properties")
        .OrShould().ResideInNamespaceMatching("Franz.Common.MongoDB")
        .OrShould().ResideInNamespaceMatching("Franz.Common.MongoDB.Config") 

        // âœ… Allow DI, Mediator, and core utilities
        .OrShould().ResideInNamespaceMatching("Microsoft.Extensions.DependencyInjection")
        .OrShould().ResideInNamespaceMatching("Franz.Common.Mediator")
        .OrShould().ResideInNamespaceMatching("Franz.Common.Errors")

        // âœ… Allow standard BCL namespaces
        .OrShould().ResideInNamespaceMatching("System")
        .OrShould().ResideInNamespaceMatching("System.Collections")
        .OrShould().ResideInNamespaceMatching("System.Collections.Generic")
        .OrShould().ResideInNamespaceMatching("System.Linq")
        .OrShould().ResideInNamespaceMatching("System.Threading")
        .OrShould().ResideInNamespaceMatching("System.Threading.Tasks")
        .OrShould().ResideInNamespaceMatching("System.Runtime.CompilerServices")

        // âœ… Allow itself (internal persistence classes)
        .OrShould().ResideInNamespaceMatching("UserService.Persistence")

        .Because("The Persistence layer should depend only on UserService persistence abstractions, core business types, and system libraries.")
        .WithoutRequiringPositiveResults()
        .Check(BaseArchitecture);

    Console.WriteLine("âœ… Verified persistence dependency isolation (UserService + System + self).");
  }




  [Fact]
  public void RepositoryImplementationsExist()
  {
    var persistenceTypes = LoadAssembliesWithPattern(@".*\.Persistence\..*")
        .SelectMany(assembly => assembly.GetTypes());

    foreach (var type in persistenceTypes)
    {
      if (type.Name.EndsWith("Repository") && type.IsClass)
      {
        // Check for implementation of a repository interface
        var repositoryInterface = type.GetInterfaces()
            .FirstOrDefault(interf => interf.Name.EndsWith("Repository"));

        Assert.NotNull(repositoryInterface);
      }
    }
  }
}

