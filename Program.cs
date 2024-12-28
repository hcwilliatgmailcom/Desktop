using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Desktop.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

public class PhpCodeGenerator
{

    public static void Main(string[] args)
    {

        string links = "";
        var types = Gettypes(typeof(Desktop.Context.MyCmdbContext));
        foreach (var type in types)
        {
            links += $"<a href=\"/{type.Name}\">{type.Name}</a><br/>";
        }
        File.WriteAllText($"C:\\Users\\hcwilli\\Desktop\\php\\index.php", links);

        foreach (var type in types)
        {
            links += $"<a href=\"/{type.Name}\">{type.Name}</a><br/>";
        }


        using (var context = new MyCmdbContext())
        {
            // Using the recommended approach:
            var tableNames = context.GetTableNames();

            // OR using the reflective approach:
            //var tableNames = context.GetTableNamesReflectively();
            foreach (var type in types)
            {
                if (type.Name == "City")
                {
                    //System.Diagnostics.Debugger.Break();
                }

                var tableName = tableNames[type];

                var refAttributes = type.GetProperties()
                    .Where(p => p.GetCustomAttributes(typeof(ReferenceAttribute), true).Length > 0)
                    .ToList();

                GeneratePhpCode(type, tableName, refAttributes,context);
            }



        }


    }

    public static string GetColumnName(PropertyInfo property, DbContext context)
    {
        var entityType = context.Model.FindEntityType(property.DeclaringType);
        var propertyType = entityType.FindProperty(property.Name);
        return propertyType.GetColumnName(StoreObjectIdentifier.Table(entityType.GetTableName(), null));
    }

    public static Type[] Gettypes(Type dbContextType)
    {
        return dbContextType.GetProperties()
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .Select(p => p.PropertyType.GetGenericArguments()[0])
            .ToArray();
    }

    public static void GeneratePhpCode(Type modelClass, String tableName, List<PropertyInfo> refAttributes, DbContext context)  
    {

        string directoryPath = $"C:\\Users\\hcwilli\\Desktop\\php\\{modelClass.Name}";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        string indexCode = GeneratePhpIndex(modelClass, tableName);
        File.WriteAllText(Path.Combine(directoryPath, "index.php"), indexCode);

        string detailCode = GeneratePhpDetail(modelClass, tableName, refAttributes,context);
        File.WriteAllText(Path.Combine(directoryPath, "detail.php"), detailCode);

    }

    private static string GeneratePhpDetail(Type modelClass, String tableName, List<PropertyInfo> refAttributes, DbContext context)
    {

        StringBuilder sb = new StringBuilder();
        GenerateDbConnection(sb);
        sb.AppendLine("echo $_SERVER['REQUEST_URI'];");
        sb.AppendLine($"$stmt = $db->prepare(\"SELECT * FROM {tableName} LIMIT 1,1\");");
        sb.AppendLine("$stmt->execute();");
        sb.AppendLine("$items=$stmt->fetchAll(PDO::FETCH_ASSOC);");
        sb.AppendLine("?>");
        sb.AppendLine("<dl>");
        sb.AppendLine("    <?php foreach ($items as $item): ?>");
        sb.AppendLine("             <?php foreach ($item as $key => $value): ?>");
        sb.AppendLine("                <dt><?php echo htmlspecialchars($key); ?></dt><dd><?php echo htmlspecialchars($value); ?></dd>");
        sb.AppendLine("            <?php endforeach; ?>");
        sb.AppendLine("    <?php endforeach; ?>");
        sb.AppendLine("</dl>");

        foreach (var property in refAttributes)
        {
            var refAttribute = (ReferenceAttribute)property.GetCustomAttributes(typeof(ReferenceAttribute), true)[0];
            var refType = refAttribute.ReferenceType;
            var refProperty = refAttribute.ReferenceProperty;

            var columnName = GetColumnName(property, context);

            sb.AppendLine($"Property: {property.Name}, Reference Type: {refType.Name}, Reference Property: {refProperty}, Column Name: {columnName}");
            sb.AppendLine($"<iframe src='/{refType.Name}'>");
        
        }



        return sb.ToString();
    }

    private static string GeneratePhpIndex(Type modelClass, String tableName)
    {

        StringBuilder sb = new StringBuilder();
        GenerateDbConnection(sb);
        sb.AppendLine("echo $_SERVER['REQUEST_URI'];");
        sb.AppendLine($"$stmt = $db->prepare(\"SELECT * FROM {tableName} LIMIT 1,10\");");
        sb.AppendLine("$stmt->execute();");
        sb.AppendLine("$items=$stmt->fetchAll(PDO::FETCH_ASSOC);");
        sb.AppendLine("?>");
        sb.AppendLine("<table>");
        sb.AppendLine("<tbody>");
        sb.AppendLine("    <?php foreach ($items as $item): ?>");
        sb.AppendLine("        <tr>");
        sb.AppendLine("             <?php foreach ($item as $key => $value): ?>");
        sb.AppendLine("                <td><?php echo htmlspecialchars($value); ?></td>");
        sb.AppendLine("            <?php endforeach; ?>");
        sb.AppendLine("            <td>");
        sb.AppendLine("                <a href=\"detail/<?php echo $item['id']; ?>\">Detail</a>");
        sb.AppendLine("            </td>");
        sb.AppendLine("        </tr>   ");
        sb.AppendLine("    <?php endforeach; ?>");
        sb.AppendLine("</tbody>");
        sb.AppendLine("</table>");
        return sb.ToString();


        // string lowerModelClassName = modelClassName.ToLower();
        // Type modelType = GetModelType(projectNamespace, modelClassName);

        // string searchLogic = GeneratePhpSearchLogic(modelType, lowerModelClassName);
        // string detailKeyName = GetDetailKeyName(modelType);
        // string detailKeyType = GetDetailKeyType(modelType);

        //         return $@"
        // <?php
        // namespace App\Controllers;

        // use App\Models\\{modelClassName};
        // use App\Helpers\Paginator;

        // class {modelClassName}Controller
        // {{
        //     public function index()
        //     {{
        //         \$searchString = \$_GET['searchString'] ?? '';
        //         \$pageNumber = \$_GET['page'] ?? 1;
        //         \$pageSize = 5; // Set your desired page size

        //         \${lowerModelClassName}Model = new {modelClassName}();

        //         if (!empty(\$searchString)) {{
        //             {searchLogic}
        //             \$pageNumber = 1; // Reset to page 1 if search term changes
        //         }} else {{
        //             \${lowerModelClassName}s = \${lowerModelClassName}Model->getAll();
        //         }}

        //         \$paginator = new Paginator(\${lowerModelClassName}s, \$pageNumber, \$pageSize);
        //         \${lowerModelClassName}s = \$paginator->getData();

        //         \$viewData = [
        //             '{lowerModelClassName}s' => \${lowerModelClassName}s,
        //             'searchString' => \$searchString,
        //             'paginator' => \$paginator
        //         ];

        //         // Load a view and pass the data
        //         require 'Views/{modelClassName}/index.php'; 
        //     }}

        //     public function details(\$id)
        //     {{
        //         \${lowerModelClassName}Model = new {modelClassName}();
        //         \${lowerModelClassName} = \${lowerModelClassName}Model->find(\$id);

        //         if (!\${lowerModelClassName}) {{
        //             // Handle not found error (e.g., redirect to 404 page)
        //             echo ""{modelClassName} not found."";
        //             return;
        //         }}

        //         // Load a view and pass the data
        //         require 'Views/{modelClassName}/details.php';
        //     }}
        // }}
        // ";
    }

    private static void GenerateDbConnection(StringBuilder sb)
    {
        sb.AppendLine("<?php");
        sb.AppendLine("try {");
        sb.AppendLine("    $db = new PDO(\"mysql:host=hcwilli.at;dbname=d0424dc5\", \"d0424dc5\", \"3QHu9nnDesLrDbKF44vN\");");
        sb.AppendLine("    $db->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);");
        sb.AppendLine("} catch (PDOException $e) {");
        sb.AppendLine("    die(\"Database connection failed: \" . $e->getMessage());");
        sb.AppendLine("}");
    }

    private static string GeneratePhpSearchLogic(Type modelType, string lowerModelClassName)
    {
        if (modelType == null)
        {
            return ""; // Handle the case where the model type is not found.
        }

        StringBuilder searchLogic = new StringBuilder();
        var searchableProperties = modelType.GetProperties()
            .Where(p => p.PropertyType == typeof(string));

        searchLogic.Append($"$${lowerModelClassName}s = array_filter($${lowerModelClassName}Model->getAll(), function($$item) use ($$searchString) {{\n");
        searchLogic.Append("    return ");

        if (searchableProperties.Any())
        {
            bool firstProperty = true;
            foreach (PropertyInfo prop in searchableProperties)
            {
                if (!firstProperty)
                {
                    searchLogic.Append(" || ");
                }
                searchLogic.Append($"strpos(strtolower($$item['{prop.Name}']), strtolower($$searchString)) !== false");
                firstProperty = false;
            }
        }
        else
        {
            searchLogic.Append("true"); // No searchable properties, return all items
        }

        searchLogic.Append(";\n");
        searchLogic.Append("});");

        return searchLogic.ToString();
    }

    private static string GetDetailKeyName(Type modelType)
    {
        if (modelType == null) return "id";

        var detailKeyProperty = modelType.GetProperties()
            .FirstOrDefault(p => p.GetCustomAttributes(typeof(DetailKeyAttribute), true).Length > 0);

        return detailKeyProperty?.Name ?? "id";
    }

    private static string GetDetailKeyType(Type modelType)
    {
        if (modelType == null) return "int";

        var detailKeyProperty = modelType.GetProperties()
            .FirstOrDefault(p => p.GetCustomAttributes(typeof(DetailKeyAttribute), true).Length > 0);

        return GetPHPTypeName(detailKeyProperty?.PropertyType) ?? "int";
    }

    //get type in php
    private static string GetPHPTypeName(Type type)
    {
        if (type == null) return "int";

        if (type == typeof(int) || type == typeof(int?)) return "int";
        if (type == typeof(string)) return "string";
        if (type == typeof(Guid) || type == typeof(Guid?)) return "string"; // Assuming GUIDs are stored as strings in PHP
        if (type == typeof(DateTime) || type == typeof(DateTime?)) return "string"; // Assuming DateTime is stored as a formatted string in PHP
        if (type == typeof(bool) || type == typeof(bool?)) return "bool";
        if (type == typeof(double) || type == typeof(double?)) return "float";
        if (type == typeof(float) || type == typeof(float?)) return "float";
        if (type == typeof(decimal) || type == typeof(decimal?)) return "float";

        return "mixed"; // Fallback for other types
    }

    private static string GeneratePhpIndexView(string modelClassName, string projectNamespace)
    {
        string lowerModelClassName = modelClassName.ToLower();
        Type modelType = GetModelType(projectNamespace, modelClassName);
        string detailKeyName = GetDetailKeyName(modelType);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<h2><?php echo \"{$modelClassName} List\"; ?></h2>");
        sb.AppendLine("<form method=\"GET\">");
        sb.AppendLine("    <p>");
        sb.AppendLine("        Title: <input type=\"text\" name=\"searchString\" value=\"<?php echo $searchString; ?>\" />");
        sb.AppendLine("        <input type=\"submit\" value=\"Filter\" />");
        sb.AppendLine("    </p>");
        sb.AppendLine("</form>");
        sb.AppendLine("<table class=\"table\">");
        sb.AppendLine("    <thead>");
        sb.AppendLine("        <tr>");

        if (modelType != null)
        {
            foreach (PropertyInfo prop in modelType.GetProperties())
            {
                sb.AppendLine($"            <th><?php echo \"{prop.Name}\"; ?></th>");
            }
        }

        sb.AppendLine("            <th>Actions</th>");
        sb.AppendLine("        </tr>");
        sb.AppendLine("    </thead>");
        sb.AppendLine("    <tbody>");
        sb.AppendLine($"        <?php foreach ($${lowerModelClassName}s as $${lowerModelClassName}): ?>");
        sb.AppendLine("            <tr>");

        if (modelType != null)
        {
            foreach (PropertyInfo prop in modelType.GetProperties())
            {
                sb.AppendLine($"                <td><?php echo $${lowerModelClassName}['{prop.Name}']; ?></td>");
            }
        }

        sb.AppendLine($"                <td><a href=\"/{lowerModelClassName}/details/<?php echo $${lowerModelClassName}['{detailKeyName}']; ?>\">Details</a></td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("        <?php endforeach; ?>");
        sb.AppendLine("    </tbody>");
        sb.AppendLine("</table>");
        sb.AppendLine("<div class=\"pagination\">");
        sb.AppendLine("    <?php if ($$paginator->hasPreviousPage()): ?>");
        sb.AppendLine($"        <a href=\"/{lowerModelClassName}?page=<?php echo $$paginator->getPreviousPage(); ?>&searchString=<?php echo $$searchString; ?>\">Previous</a>");
        sb.AppendLine("    <?php endif; ?>");
        sb.AppendLine("    <?php echo \" Page \" . $$paginator->getCurrentPage() . \" of \" . $$paginator->getTotalPages() . \" \"; ?>");
        sb.AppendLine("    <?php if ($$paginator->hasNextPage()): ?>");
        sb.AppendLine($"        <a href=\"/{lowerModelClassName}?page=<?php echo $$paginator->getNextPage(); ?>&searchString=<?php echo $$searchString; ?>\">Next</a>");
        sb.AppendLine("    <?php endif; ?>");
        sb.AppendLine("</div>");

        return sb.ToString();
    }

    private static Type GetModelType(string projectNamespace, string modelClassName)
    {
        var assembly = Assembly.Load(projectNamespace);
        return assembly.GetType($"{projectNamespace}.Models.{modelClassName}");
    }

    private static string GeneratePhpDetailsView(string modelClassName, string projectNamespace)
    {
        string lowerModelClassName = modelClassName.ToLower();
        Type modelType = GetModelType(projectNamespace, modelClassName);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"<h2>{modelClassName} Details</h2>");
        sb.AppendLine("<div>");
        sb.AppendLine("    <dl class=\"row\">");

        if (modelType != null)
        {
            foreach (PropertyInfo prop in modelType.GetProperties())
            {
                sb.AppendLine($"        <dt class=\"col-sm-2\">");
                sb.AppendLine($"            <?php echo \"{prop.Name}\"; ?>");
                sb.AppendLine($"        </dt>");
                sb.AppendLine($"        <dd class=\"col-sm-10\">");
                sb.AppendLine($"            <?php echo $${lowerModelClassName}['{prop.Name}']; ?>");
                sb.AppendLine($"        </dd>");
            }
        }

        sb.AppendLine("    </dl>");
        sb.AppendLine("</div>");
        sb.AppendLine("<div>");
        sb.AppendLine($"    <a href=\"/{lowerModelClassName}\">Back to List</a>");
        sb.AppendLine("</div>");

        return sb.ToString();
    }



}

// Define the DetailKeyAttribute in your C# project
public class DetailKeyAttribute : Attribute
{
}