// Planora.BLL/Handlers/Queries/Teachers/FindTeacherLocationQuery.cs
using MediatR;
using Planora.BLL.DTOs.Queries;

namespace Planora.BLL.Handlers.Queries.Teachers
{
    public class FindTeacherLocationQuery : IRequest<TeacherLocationResultDto>
    {
        public FindTeacherLocationQueryDto SearchData { get; }

        public FindTeacherLocationQuery(FindTeacherLocationQueryDto searchData)
        {
            SearchData = searchData;
        }
    }
}