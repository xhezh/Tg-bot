using static kdz3_3.Program;

namespace kdz3_3;
internal static class Sort {
    internal static FilterAndSortFunc sortDelegate = null; // Делегат для удобства.
    /// <summary>
    /// Прямая сортировка по AdmArea.
    /// </summary>
    /// <returns></returns>
    internal static async Task AdmAreaSortAscending() {
        var selected = from p in Program.electrocarPowers // Сортируем через LINQ.
                       orderby p.AdmArea ascending
                       select p;
        Program.electrocarPowers = selected.ToArray();
    }
    /// <summary>
    /// Обратная сортировка по AdmArea.
    /// </summary>
    /// <returns></returns>
    internal static async Task AdmAreaSortDescending() {
        var selected = from p in Program.electrocarPowers // Сортируем через LINQ.
                       orderby p.AdmArea descending
                       select p;
        Program.electrocarPowers = selected.ToArray();
    }
}

