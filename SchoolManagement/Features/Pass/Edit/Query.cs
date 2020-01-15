using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Dto;
using Newtonsoft.Json;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.Pass.Edit
{
    public class Query : IRequest<DataResult<ViewModel>>
    {
        public int Id { get; set; }
        public int RedirectMemberId { get; set; }
    }

    public class QueryHandler : IRequestHandler<Query, DataResult<ViewModel>>
    {
        private readonly SchoolManagementContext _context;

        public QueryHandler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<DataResult<ViewModel>> Handle(Query request, CancellationToken cancellationToken)
        {
            Model.Domain.Pass pass = await _context.Passes.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (pass is null)
            {
                return DataResult<ViewModel>.Error(PolishReadableMessage.Pass.NotFound);
            }
            var serializedParent = JsonConvert.SerializeObject(PassDto.FromPass(pass));
            ViewModel viewModel = JsonConvert.DeserializeObject<ViewModel>(serializedParent);
            viewModel.MemberId = request.RedirectMemberId;
            return DataResult<ViewModel>.Success(viewModel);
        }
    }
}
