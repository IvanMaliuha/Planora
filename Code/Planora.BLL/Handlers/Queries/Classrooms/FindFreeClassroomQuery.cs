using MediatR;
using Planora.BLL.DTOs.Queries;
using System.Collections.Generic;

namespace Planora.BLL.Handlers.Queries.Classrooms
{
    public class FindFreeClassroomQuery : IRequest<List<FreeClassroomResultDto>>
    {
        public FindFreeClassroomQueryDto SearchData { get; }

        public FindFreeClassroomQuery(FindFreeClassroomQueryDto searchData)
        {
            SearchData = searchData;
        }
    }
}