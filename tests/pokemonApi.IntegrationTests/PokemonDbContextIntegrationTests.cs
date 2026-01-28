// Learning Point: This is an integration test that tests EF Core and the database together
// using Testcontainers to spin up a real PostgreSQL database for testing.

using System;
using Microsoft.EntityFrameworkCore;
using pokemonApi.Data;
using pokemonApi.Models;

namespace pokemonApi.IntegrationTests;

/// <summary>
/// Learning Point: For now, these tests demonstrate the database schema.
/// We'll implement testcontainers in Lesson 2 when we have more complexity.
/// The test infrastructure is in place and ready to expand.
/// </summary>
public class PokemonDbContextIntegrationTests
{
    // Placeholder class - actual tests will be added in Lesson 2
}
