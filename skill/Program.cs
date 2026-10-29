
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

Console.WriteLine("================================");
Console.WriteLine("       CodeFast Generator");
Console.WriteLine("================================");
Console.WriteLine("1. Generate Class");
Console.WriteLine("2. Generate CRUD Service");
Console.WriteLine("3. Generate DTO");
Console.WriteLine("4. Generate Interface");
Console.Write("Choose an option: ");

string? choice = Console.ReadLine();

Console.Write("Enter class name: ");
string? className = Console.ReadLine();

if (string.IsNullOrWhiteSpace(className) ||
    !Regex.IsMatch(className, @"^[A-Za-z_][A-Za-z0-9_]*$"))
{
    Console.WriteLine("Invalid class name.");
    return;
}

string outputDirectory = Path.Combine(
    Directory.GetCurrentDirectory(),
    "Generated"
);

Directory.CreateDirectory(outputDirectory);

string fileName = choice switch
{
    "1" => $"{className}.cs",
    "2" => $"{className}Service.cs",
    "3" => $"{className}Dto.cs",
    "4" => $"I{className}Service.cs",
    _ => ""
};

if (string.IsNullOrEmpty(fileName))
{
    Console.WriteLine("Invalid option.");
    return;
}

string filePath = Path.Combine(outputDirectory, fileName);

if (File.Exists(filePath))
{
    Console.WriteLine("File already exists. Nothing was overwritten.");
    return;
}

string code = choice switch
{
    "1" => GenerateClass(className),
    "2" => GenerateCrudService(className),
    "3" => GenerateDto(className),
    "4" => GenerateInterface(className),
    _ => ""
};

File.WriteAllText(filePath, code);

Console.WriteLine();
Console.WriteLine("Code generated successfully!");
Console.WriteLine($"File: {filePath}");

static string GenerateClass(string name)
{
    return $$"""
    namespace Generated;

    public class {{name}}
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    """;
}

static string GenerateCrudService(string name)
{
    return $$"""
    namespace Generated;

    public class {{name}}Service
    {
        private readonly List<{{name}}> _items = new();
        private int _nextId = 1;

        public IReadOnlyList<{{name}}> GetAll()
        {
            return _items.AsReadOnly();
        }

        public {{name}}? GetById(int id)
        {
            return _items.FirstOrDefault(item => item.Id == id);
        }

        public void Add({{name}} item)
        {
            ArgumentNullException.ThrowIfNull(item);

            item.Id = _nextId++;
            _items.Add(item);
        }

        public bool Delete(int id)
        {
            var item = GetById(id);

            if (item is null)
                return false;

            return _items.Remove(item);
        }
    }
    """;
}

static string GenerateDto(string name)
{
    return $$"""
    namespace Generated;

    public class {{name}}Dto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
    """;
}

static string GenerateInterface(string name)
{
    return $$"""
    namespace Generated;

    public interface I{{name}}Service
    {
        IReadOnlyList<{{name}}> GetAll();

        {{name}}? GetById(int id);

        void Add({{name}} item);

        bool Delete(int id);
    }
    """;
}
