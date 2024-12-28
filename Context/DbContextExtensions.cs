using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class DbContextExtensions
{
    public static Dictionary<Type, string> GetTableNames(this DbContext context)
    {
        var tableNames = new Dictionary<Type, string>();
        var entityTypes = context.Model.GetEntityTypes();

        foreach (var entityType in entityTypes)
        {
            var tableName = entityType.GetTableName(); 

            if(string.IsNullOrEmpty(tableName))
            {
                tableName = entityType.GetViewName();
            }
            
            //Handle scenarios where table name might be different due to fluent API configuration
            if(string.IsNullOrEmpty(tableName))
            {
                tableName = entityType.DisplayName(); //Fallback to entity type name if no table name is configured
            }

            tableNames.Add(entityType.ClrType, tableName);
        }

        return tableNames;
    }
    
    public static Dictionary<Type, string> GetTableNamesReflectively(this DbContext context)
    {
        var tableNames = new Dictionary<Type, string>();
        var dbSetProperties = context.GetType().GetProperties()
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        foreach (var dbSetProperty in dbSetProperties)
        {
            var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
            var tableName = context.Model.FindEntityType(entityType)?.GetTableName();

            //Handle scenarios where table name might be different due to fluent API configuration
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = entityType.Name; //Fallback to entity type name if no table name is configured
            }

            tableNames.Add(entityType, tableName);
        }
        return tableNames;
    }
}