using BookHub.Shared.DTOs;

namespace BookHub.BlazorClient.Services
{
    public class LoanService : ILoanService
    {
        private readonly HttpClient _httpClient;

        public LoanService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LoanDto>> GetOverdueLoansAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LoanDto>> GetUserLoansAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<LoanDto?> ReturnLoanAsync(Guid loanId)
        {
            throw new NotImplementedException();
        }
    }
}
