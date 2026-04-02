
// (c) 2026 Kazuki Kohzuki

namespace EnumSerializer.TestGenerator.Verify;

internal record TestCaseInfo(string SourceName, bool SourceIsStatic, string? TestName, string FilePath)
{
    internal string GetTestMethodName()
        => this.TestName ?? ToPascalCase(this.SourceName);

    private static string ToPascalCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        var buffer = new char[name.Length];
        var writeIndex = 0;

        var capitalizeNext = true;

        for (var i = 0; i < name.Length; i++)
        {
            var c = name[i];

            if (c == '_')
            {
                capitalizeNext = true;
                continue;
            }

            if (capitalizeNext)
            {
                buffer[writeIndex++] = char.ToUpperInvariant(c);
                capitalizeNext = false;
            }
            else
            {
                buffer[writeIndex++] = c;
            }
        }

        return new(buffer, 0, writeIndex);
    } // private static string ToPascalCase (string)
} // private record FieldInfo (string, bool, string?)
