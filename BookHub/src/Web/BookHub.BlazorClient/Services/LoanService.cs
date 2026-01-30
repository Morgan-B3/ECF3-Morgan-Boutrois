using BookHub.Shared.DTOs;
using System.Net.Http.Json;

namespace BookHub.BlazorClient.Services
{
    public class LoanService : ILoanService
    {
        private readonly HttpClient _httpClient;

        public LoanService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/loans", createLoanDto);


            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Loan creation failed: {error}");
            }


            return (await response.Content.ReadFromJsonAsync<LoanDto>())!;
        }

        public async Task<IEnumerable<LoanDto>> GetAllLoansAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<LoanDto>>("api/loans")
                ?? Enumerable.Empty<LoanDto>();
        }

        public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<LoanDto>>("api/loans/overdue")
                ?? Enumerable.Empty<LoanDto>();
        }

        public async Task<IEnumerable<LoanDto>> GetUserLoansAsync(Guid userId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<LoanDto>>($"api/loans/user/{userId}") 
                ?? Enumerable.Empty<LoanDto>();
        }

        public async Task<LoanDto?> ReturnLoanAsync(Guid loanId)
        {
            var response = await _httpClient.PutAsync($"api/loans/{loanId}/return", null);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Return loan failed: {error}");
            }

            return await response.Content.ReadFromJsonAsync<LoanDto>();
        }
    }
}
