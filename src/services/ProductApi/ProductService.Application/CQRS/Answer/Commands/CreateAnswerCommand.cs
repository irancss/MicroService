using AutoMapper;
using MediatR;
using ProductService.Application.Interfaces;

namespace ProductService.Application.CQRS.Answer.Commands
{
    public class CreateAnswerCommand : IRequest<string>
    {
        public string QuestionId { get; set; }
        public string UserId { get; set; }
        public string AnswerText { get; set; }
        public bool IsAdminAnswer { get; set; }

    }

    public class CreateAnswerCommandHandler : IRequestHandler<CreateAnswerCommand, string>
    {
        private IAnswerService _answerService;
        private IMapper _mapper;

        public CreateAnswerCommandHandler(IAnswerService answerService, IMapper mapper)
        {
            _answerService = answerService;
            _mapper = mapper;
        }
        public async Task<string> Handle(CreateAnswerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
