using TestProject.Common.Enums;

namespace TestProject.Common.Models
{
    public record MeteoriteByYearSort
    (
        MeteoritesByYearSortParam Property,
        SortType Type
    );
}
